using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Annelitrice.Retheme.DarkSouls
{
    // The base mod spawns the adult immediately. A short-lived fire column
    // conceals that spawn, then clears to reveal the actual pawn underneath.
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
            animation.SpawnGlow(instance);
        }

        private void ResolveDefs()
        {
            if (columnDef != null)
                return;
            columnDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchFirestormColumn");
            ringDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchFirestormRing");
            smokeDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchAfterSmoke");
            emberDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchEmber");
            glowDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HatchGlow");
        }

        private void SpawnColumn(Instance instance)
        {
            FleckCreationData column = FleckMaker.GetDataStatic(
                instance.cell.ToVector3Shifted() + new Vector3(0f, 0f, 0.08f),
                map, columnDef, 1.65f);
            column.rotation = instance.rotation * 0.35f;
            column.instanceColor = new Color(1f, 1f, 1f, 0.82f);
            map.flecks.CreateFleck(column);
        }

        private void SpawnRing(Instance instance, int stage)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            FleckCreationData ring = FleckMaker.GetDataStatic(
                center + new Vector3(0f, 0f, -0.04f), map, ringDef,
                stage == 0 ? 1.84f : stage == 1 ? 2.08f : 2.29f);
            ring.rotation = instance.rotation + stage * 42f;
            ring.rotationRate = -(stage == 0 ? 31f : stage == 1 ? 24f : 18f);
            ring.instanceColor = new Color(1f, 1f, 1f,
                stage == 0 ? 0.72f : stage == 1 ? 0.58f : 0.38f);
            map.flecks.CreateFleck(ring);
        }

        private void SpawnGlow(Instance instance)
        {
            FleckCreationData glow = FleckMaker.GetDataStatic(
                instance.cell.ToVector3Shifted() + new Vector3(0f, 0f, -0.10f),
                map, glowDef, 1.55f);
            glow.instanceColor = new Color(1f, 0.29f, 0.07f, 0.30f);
            map.flecks.CreateFleck(glow);
        }

        public override void MapComponentTick()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                int age = ++instance.age;

                if (age == 18) SpawnRing(instance, 1);
                if (age == 42) SpawnRing(instance, 2);

                if (age == 7 || age == 20 || age == 31 || age == 45 ||
                    age == 60 || age == 78 || age == 98 || age == 115)
                    SpawnEmberBurst(instance, age);

                if (age == 32 || age == 59 || age == 89)
                    SpawnAfterSmoke(instance, age);

                if (age >= DurationTicks)
                    active.RemoveAt(i);
            }
        }

        private void SpawnEmberBurst(Instance instance, int age)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            int count = age == 31 ? 12 : age >= 98 ? 5 : age == 78 ? 6 : 9;
            for (int i = 0; i < count; i++)
            {
                float angle = Rand.Range(0f, Mathf.PI * 2f);
                float speed = Rand.Range(0.45f, 1.08f);
                FleckCreationData ember = FleckMaker.GetDataStatic(
                    center + new Vector3(Rand.Range(-0.27f, 0.27f), 0f,
                        Rand.Range(-0.19f, 0.29f)), map, emberDef,
                    Rand.Range(0.075f, 0.15f));
                ember.velocity = new Vector3(Mathf.Cos(angle) * speed, 0f,
                    Mathf.Sin(angle) * speed * 0.62f + Rand.Range(0.20f, 0.43f));
                ember.solidTimeOverride = Rand.Range(0.16f, 0.35f);
                ember.instanceColor = new Color(1f, Rand.Range(0.28f, 0.61f),
                    Rand.Range(0.06f, 0.15f), Rand.Range(0.64f, 0.90f));
                map.flecks.CreateFleck(ember);
            }
        }

        private void SpawnAfterSmoke(Instance instance, int age)
        {
            float side = age == 59 ? 1f : -1f;
            Vector3 center = instance.cell.ToVector3Shifted();
            FleckCreationData smoke = FleckMaker.GetDataStatic(
                center + new Vector3(side * Rand.Range(0.27f, 0.42f), 0f,
                    Rand.Range(-0.08f, 0.18f)), map, smokeDef,
                Rand.Range(0.82f, 1.09f));
            smoke.rotation = side * Rand.Range(18f, 36f);
            smoke.rotationRate = side * Rand.Range(5f, 12f);
            smoke.velocity = new Vector3(side * Rand.Range(0.13f, 0.24f), 0f,
                Rand.Range(0.11f, 0.20f));
            smoke.instanceColor = new Color(0.58f, 0.44f, 0.39f,
                age == 89 ? 0.20f : 0.29f);
            map.flecks.CreateFleck(smoke);
        }
    }
}

