using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Annelitrice.Retheme.DarkSouls
{
    // The curse is shared through touch: a low stream of dark mist crosses
    // the short gap, then briefly brands the recipient.
    public sealed class DarksignTransferAnimation : MapComponent
    {
        private const int DurationTicks = 80;

        private sealed class Instance
        {
            internal Vector3 source;
            internal Vector3 target;
            internal int age;
        }

        private readonly List<Instance> active = new List<Instance>();
        private static FleckDef brandDef;
        private static FleckDef transferMistDef;
        private static FleckDef smokeDef;
        private static FleckDef emberDef;

        public DarksignTransferAnimation(Map map) : base(map) { }

        internal static void Start(Pawn caster, Pawn recipient)
        {
            if (caster == null || recipient == null || !caster.Spawned ||
                !recipient.Spawned || caster.Map == null || caster.Map != recipient.Map)
                return;

            DarksignTransferAnimation animation =
                caster.Map.GetComponent<DarksignTransferAnimation>();
            animation.ResolveDefs();
            Instance instance = new Instance
            {
                source = caster.Position.ToVector3Shifted(),
                target = recipient.Position.ToVector3Shifted(),
                age = 0
            };
            animation.active.Add(instance);
            animation.SpawnSourceBrand(instance);
            animation.SpawnTransferMist(instance, 0);
        }

        private void ResolveDefs()
        {
            if (brandDef != null)
                return;
            brandDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_DarksignBrand");
            transferMistDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_DarksignTransferMist");
            smokeDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_DarksignSmoke");
            emberDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_DarksignEmber");
        }

        public override void MapComponentTick()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                int age = ++instance.age;

                if (age <= 34 && age % 2 == 0)
                    SpawnTransferMist(instance, age);
                if (age == 34)
                    SpawnTargetBrand(instance);
                if (age == 43)
                    SpawnTargetSmoke(instance);
                if (age >= 34 && age <= 54 && age % 5 == 4)
                    SpawnBrandEmbers(instance);

                if (age >= DurationTicks)
                    active.RemoveAt(i);
            }
        }

        private void SpawnSourceBrand(Instance instance)
        {
            FleckCreationData mark = FleckMaker.GetDataStatic(
                instance.source + new Vector3(0f, 0f, 0.03f), map, brandDef, 0.92f);
            mark.rotation = Rand.Range(-14f, 14f);
            mark.instanceColor = new Color(1f, 1f, 1f, 0.52f);
            map.flecks.CreateFleck(mark);
        }

        private void SpawnTransferMist(Instance instance, int age)
        {
            Vector3 delta = instance.target - instance.source;
            delta.y = 0f;
            float distance = Mathf.Sqrt(delta.x * delta.x + delta.z * delta.z);
            if (distance < 0.05f)
                return;

            Vector3 direction = delta / distance;
            Vector3 sideways = new Vector3(-direction.z, 0f, direction.x);
            float progress = Mathf.Clamp01((age + 2f) / 38f);
            float curve = 0.12f * Mathf.Sin(Mathf.PI * progress);
            Vector3 position = instance.source + delta * progress +
                sideways * (curve + Rand.Range(-0.045f, 0.045f)) +
                new Vector3(0f, 0f, 0.05f);
            FleckCreationData mist = FleckMaker.GetDataStatic(
                position, map, transferMistDef, Rand.Range(0.30f, 0.43f));
            mist.rotation = Rand.Range(-24f, 24f);
            mist.rotationRate = Rand.Range(-10f, 10f);
            mist.velocity = direction * Rand.Range(0.09f, 0.17f) +
                sideways * Rand.Range(-0.05f, 0.05f) +
                new Vector3(0f, 0f, 0.04f);
            mist.instanceColor = new Color(0.55f, 0.50f, 0.60f,
                Rand.Range(0.31f, 0.43f));
            map.flecks.CreateFleck(mist);
        }

        private void SpawnTargetBrand(Instance instance)
        {
            FleckCreationData mark = FleckMaker.GetDataStatic(
                instance.target + new Vector3(0f, 0f, 0.04f), map, brandDef, 1.45f);
            mark.rotation = Rand.Range(-10f, 10f);
            mark.instanceColor = new Color(1f, 1f, 1f, 0.80f);
            map.flecks.CreateFleck(mark);
        }

        private void SpawnTargetSmoke(Instance instance)
        {
            FleckCreationData smoke = FleckMaker.GetDataStatic(
                instance.target + new Vector3(0f, 0f, 0.07f),
                map, smokeDef, 0.56f);
            smoke.rotation = Rand.Range(-18f, 18f);
            smoke.rotationRate = Rand.Range(-8f, 8f);
            smoke.velocity = new Vector3(0f, 0f, 0.08f);
            smoke.instanceColor = new Color(1f, 1f, 1f, 0.20f);
            map.flecks.CreateFleck(smoke);
        }

        private void SpawnBrandEmbers(Instance instance)
        {
            for (int i = 0; i < 2; i++)
            {
                float angle = Rand.Range(0f, Mathf.PI * 2f);
                float radius = Rand.Range(0.28f, 0.36f);
                Vector3 outward = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                FleckCreationData ember = FleckMaker.GetDataStatic(
                    instance.target + outward * radius,
                    map, emberDef, Rand.Range(0.045f, 0.075f));
                ember.velocity = outward * Rand.Range(0.05f, 0.11f) +
                    new Vector3(0f, 0f, Rand.Range(0.07f, 0.14f));
                ember.instanceColor = new Color(1f, Rand.Range(0.29f, 0.50f),
                    Rand.Range(0.06f, 0.13f), Rand.Range(0.52f, 0.73f));
                map.flecks.CreateFleck(ember);
            }
        }
    }
}

