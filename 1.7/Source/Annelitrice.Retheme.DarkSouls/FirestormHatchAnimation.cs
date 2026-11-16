using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Annelitrice.Retheme.DarkSouls
{
    // A dense wave of short-lived flames and embers hides the immediate spawn.
    // The storm thins after the first second to reveal the undead pawn.
    public sealed class FirestormHatchAnimation : MapComponent
    {
        private const int DurationTicks = 165;

        private sealed class Instance
        {
            internal IntVec3 cell;
            internal int age;
            internal float rotation;
        }

        private readonly List<Instance> active = new List<Instance>();
        private static FleckDef columnDef;
        private static FleckDef ringDef;
        private static FleckDef flameDef;
        private static FleckDef smokeDef;
        private static FleckDef emberDef;
        private static FleckDef glowDef;

        public FirestormHatchAnimation(Map map) : base(map) { }

        internal static void Start(PupationSite site)
        {
            if (!site.Valid)
                return;
            FirestormHatchAnimation animation = site.map.GetComponent<FirestormHatchAnimation>();
            animation.ResolveDefs();
            Instance instance = new Instance
            {
                cell = site.cell,
                age = 0,
                rotation = Rand.Range(-20f, 20f)
            };
            animation.active.Add(instance);
            animation.SpawnColumn(instance);
            animation.SpawnRing(instance, 0);
            animation.SpawnGlow(instance, 0.36f);
            animation.SpawnFlames(instance, 10, 0.88f);
            animation.SpawnEmbers(instance, 8, 0.95f);
        }

        private void ResolveDefs()
        {
            if (flameDef != null)
                return;
            columnDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchFirestormColumn");
            ringDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchFirestormRing");
            flameDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchFlameTongue");
            smokeDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchAfterSmoke");
            emberDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchEmber");
            glowDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchGlow");
        }

        private void SpawnColumn(Instance instance)
        {
            FleckCreationData column = FleckMaker.GetDataStatic(
                instance.cell.ToVector3Shifted() + new Vector3(0f, 0f, 0.08f),
                map, columnDef, 1.14f);
            column.rotation = instance.rotation * 0.35f;
            column.instanceColor = new Color(1f, 1f, 1f, 0.49f);
            map.flecks.CreateFleck(column);
        }

        private void SpawnRing(Instance instance, int stage)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            FleckCreationData ring = FleckMaker.GetDataStatic(
                center + new Vector3(0f, 0f, -0.04f), map, ringDef,
                stage == 0 ? 1.58f : 1.95f);
            ring.rotation = instance.rotation + stage * 38f;
            ring.rotationRate = stage == 0 ? -22f : 16f;
            ring.instanceColor = new Color(1f, 1f, 1f,
                stage == 0 ? 0.54f : 0.30f);
            map.flecks.CreateFleck(ring);
        }

        private void SpawnGlow(Instance instance, float alpha)
        {
            FleckCreationData glow = FleckMaker.GetDataStatic(
                instance.cell.ToVector3Shifted() + new Vector3(0f, 0f, -0.10f),
                map, glowDef, 1.22f);
            glow.instanceColor = new Color(1f, 0.32f, 0.08f, alpha);
            map.flecks.CreateFleck(glow);
        }

        public override void MapComponentTick()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                int age = ++instance.age;

                if (age == 20) SpawnRing(instance, 1);
                if (age == 35) SpawnGlow(instance, 0.23f);

                if (age <= 96 && age % 4 == 0)
                    SpawnFlames(instance, age < 60 ? 5 : 3,
                        age < 60 ? 0.75f : 0.48f);
                if (age == 16 || age == 34 || age == 52)
                    SpawnFlames(instance, 7, 0.95f);

                if (age <= 126 && age % 6 == 0)
                    SpawnEmbers(instance, age < 72 ? 7 : 4,
                        age < 72 ? 0.90f : 0.55f);

                if (age == 61 || age == 78 || age == 96 || age == 118)
                    SpawnAfterSmoke(instance, age);

                if (age >= DurationTicks)
                    active.RemoveAt(i);
            }
        }

        private void SpawnFlames(Instance instance, int count, float strength)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            for (int i = 0; i < count; i++)
            {
                float angle = Rand.Range(0f, Mathf.PI * 2f);
                float radius = Rand.Range(0.15f, 0.53f);
                float side = Mathf.Cos(angle);
                float depth = Mathf.Sin(angle);
                FleckCreationData flame = FleckMaker.GetDataStatic(
                    center + new Vector3(side * radius, 0f,
                        depth * radius * 0.58f + Rand.Range(-0.10f, 0.10f)),
                    map, flameDef, Rand.Range(0.34f, 0.67f) * strength);
                flame.rotation = Rand.Range(-32f, 32f);
                flame.rotationRate = Rand.Range(-17f, 17f);
                flame.velocity = new Vector3(
                    -depth * Rand.Range(0.12f, 0.30f) + side * 0.09f, 0f,
                    side * Rand.Range(0.06f, 0.16f) + Rand.Range(0.17f, 0.38f));
                flame.solidTimeOverride = Rand.Range(0.09f, 0.22f);
                flame.instanceColor = new Color(1f, 1f, 1f,
                    Rand.Range(0.57f, 0.86f) * strength);
                map.flecks.CreateFleck(flame);
            }
        }

        private void SpawnEmbers(Instance instance, int count, float strength)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            for (int i = 0; i < count; i++)
            {
                float angle = Rand.Range(0f, Mathf.PI * 2f);
                float speed = Rand.Range(0.47f, 1.18f) * strength;
                FleckCreationData ember = FleckMaker.GetDataStatic(
                    center + new Vector3(Rand.Range(-0.31f, 0.31f), 0f,
                        Rand.Range(-0.19f, 0.27f)), map, emberDef,
                    Rand.Range(0.06f, 0.14f));
                ember.velocity = new Vector3(Mathf.Cos(angle) * speed, 0f,
                    Mathf.Sin(angle) * speed * 0.64f + Rand.Range(0.18f, 0.43f));
                ember.solidTimeOverride = Rand.Range(0.13f, 0.30f);
                ember.instanceColor = new Color(1f, Rand.Range(0.29f, 0.63f),
                    Rand.Range(0.05f, 0.17f), Rand.Range(0.61f, 0.91f));
                map.flecks.CreateFleck(ember);
            }
        }

        private void SpawnAfterSmoke(Instance instance, int age)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            for (int i = 0; i < 2; i++)
            {
                float side = i == 0 ? -1f : 1f;
                FleckCreationData smoke = FleckMaker.GetDataStatic(
                    center + new Vector3(side * Rand.Range(0.18f, 0.46f), 0f,
                        Rand.Range(-0.10f, 0.15f)), map, smokeDef,
                    Rand.Range(0.69f, 1.02f));
                smoke.rotation = side * Rand.Range(15f, 35f);
                smoke.rotationRate = side * Rand.Range(4f, 11f);
                smoke.velocity = new Vector3(side * Rand.Range(0.08f, 0.21f),
                    0f, Rand.Range(0.10f, 0.23f));
                smoke.instanceColor = new Color(0.53f, 0.42f, 0.37f,
                    age >= 96 ? 0.17f : 0.25f);
                map.flecks.CreateFleck(smoke);
            }
        }
    }
}

