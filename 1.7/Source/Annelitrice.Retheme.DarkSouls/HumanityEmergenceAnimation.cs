using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Annelitrice.Retheme.DarkSouls
{
    // The new larva already exists when Anneli_Hatch fires. Briefly veil it,
    // burst outward from the ash, then let the real pawn show through.
    public sealed class HumanityEmergenceAnimation : MapComponent
    {
        private const int DurationTicks = 90;

        private sealed class Instance
        {
            internal IntVec3 cell;
            internal int age;
            internal float rotation;
        }

        private readonly List<Instance> active = new List<Instance>();
        private static FleckDef veilDef;
        private static FleckDef bloomDef;
        private static FleckDef wispDef;
        private static FleckDef ashDef;

        public HumanityEmergenceAnimation(Map map) : base(map) { }

        internal static void Start(PupationSite site)
        {
            if (!site.Valid)
                return;
            HumanityEmergenceAnimation animation = site.map.GetComponent<HumanityEmergenceAnimation>();
            animation.ResolveDefs();
            Instance instance = new Instance
            {
                cell = site.cell,
                rotation = Rand.Range(-18f, 18f)
            };
            animation.active.Add(instance);
            animation.SpawnVeil(instance);
        }

        private void ResolveDefs()
        {
            if (veilDef != null)
                return;
            veilDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityEmergenceVeil");
            bloomDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityEmergenceBloom");
            wispDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityEmergenceWisp");
            ashDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityEmergenceAsh");
        }

        public override void MapComponentTick()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                int age = ++instance.age;
                if (age == 8)
                    SpawnBloom(instance);
                if (age == 10)
                {
                    SpawnAsh(instance);
                    SpawnWisps(instance, 8, 0.62f);
                }
                if (age == 24)
                    SpawnWisps(instance, 4, 0.38f);
                if (age == 42)
                    SpawnWisps(instance, 2, 0.24f);
                if (age >= DurationTicks)
                    active.RemoveAt(i);
            }
        }

        private void SpawnVeil(Instance instance)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            for (int i = 0; i < 3; i++)
            {
                FleckCreationData veil = FleckMaker.GetDataStatic(
                    center + new Vector3(Rand.Range(-0.13f, 0.13f), 0f,
                        Rand.Range(-0.08f, 0.08f)), map, veilDef,
                    Rand.Range(0.70f, 0.94f));
                veil.rotation = Rand.Range(-28f, 28f);
                veil.rotationRate = Rand.Range(-13f, 13f);
                veil.velocity = new Vector3(Rand.Range(-0.08f, 0.08f), 0f,
                    Rand.Range(0.05f, 0.13f));
                veil.instanceColor = new Color(0.28f, 0.27f, 0.30f, 0.56f);
                map.flecks.CreateFleck(veil);
            }
        }

        private void SpawnBloom(Instance instance)
        {
            FleckCreationData bloom = FleckMaker.GetDataStatic(
                instance.cell.ToVector3Shifted() + new Vector3(0f, 0f, 0.02f),
                map, bloomDef, 1.03f);
            bloom.rotation = instance.rotation;
            bloom.rotationRate = 8f;
            bloom.instanceColor = new Color(1f, 1f, 1f, 0.83f);
            map.flecks.CreateFleck(bloom);
        }

        private void SpawnAsh(Instance instance)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            for (int i = 0; i < 12; i++)
            {
                float angle = Rand.Range(0f, Mathf.PI * 2f);
                float speed = Rand.Range(0.43f, 0.89f);
                FleckCreationData ash = FleckMaker.GetDataStatic(
                    center + new Vector3(Rand.Range(-0.12f, 0.12f), 0f,
                        Rand.Range(0.08f, 0.28f)), map, ashDef,
                    Rand.Range(0.08f, 0.15f));
                ash.velocity = new Vector3(Mathf.Cos(angle) * speed, 0f,
                    Mathf.Sin(angle) * speed * 0.56f + 0.23f);
                ash.rotation = Rand.Range(0f, 360f);
                ash.rotationRate = Rand.Range(-95f, 95f);
                ash.instanceColor = new Color(0.72f, 0.70f, 0.73f, 0.78f);
                map.flecks.CreateFleck(ash);
            }
        }

        private void SpawnWisps(Instance instance, int count, float speed)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            for (int i = 0; i < count; i++)
            {
                float angle = Rand.Range(0f, Mathf.PI * 2f);
                float drift = speed * Rand.Range(0.58f, 1.05f);
                FleckCreationData wisp = FleckMaker.GetDataStatic(
                    center + new Vector3(Mathf.Cos(angle) * 0.10f, 0f,
                        Mathf.Sin(angle) * 0.07f + 0.10f), map, wispDef,
                    Rand.Range(0.39f, 0.61f));
                wisp.rotation = angle * Mathf.Rad2Deg - 90f + Rand.Range(-22f, 22f);
                wisp.rotationRate = Rand.Range(-18f, 18f);
                wisp.velocity = new Vector3(Mathf.Cos(angle) * drift, 0f,
                    Mathf.Sin(angle) * drift * 0.55f + Rand.Range(0.20f, 0.36f));
                wisp.instanceColor = new Color(1f, 1f, 1f,
                    count == 8 ? 0.73f : count == 4 ? 0.53f : 0.36f);
                map.flecks.CreateFleck(wisp);
            }
        }
    }
}
