using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Annelitrice.Retheme.DarkSouls
{
    public sealed class AshAnimation : MapComponent
    {
        private const int DurationTicks = 150;
        private const float ShardAccelerationZ = -0.8f;
        private const float ShardFadeIn = 0.04f;
        private const float ShardFadeOut = 0.36f;
        private const float OrbFadeIn = 0.12f;
        private const float OrbFadeOut = 0.55f;

        private sealed class Shard
        {
            internal int start;
            internal int flight;
            internal float fromX;
            internal float fromZ;
            internal float toX;
            internal float toZ;
            internal float size;
            internal float angle;
            internal float spin;
        }

        private sealed class Orb
        {
            internal int start;
            internal Vector2 direction;
            internal float distance;
            internal float offsetX;
            internal float offsetZ;
            internal float size;
            internal float spin;
        }

        private sealed class Instance
        {
            internal IntVec3 cell;
            internal int age;
            internal Shard[] shards;
            internal Orb[] orbs;
            internal float smokeRotation;
        }

        private readonly List<Instance> active = new List<Instance>();
        private static FleckDef smokeDef;
        private static FleckDef hazeDef;
        private static FleckDef shadowDef;
        private static FleckDef eyesDef;
        private static FleckDef shardDef;
        private static FleckDef orbDef;

        public AshAnimation(Map map) : base(map) { }

        internal static void Start(PupationSite site)
        {
            if (!site.Valid)
                return;
            AshAnimation animation = site.map.GetComponent<AshAnimation>();
            animation.ResolveDefs();
            Instance instance = new Instance
            {
                cell = site.cell,
                age = 0,
                shards = CreateShards(),
                orbs = CreateOrbs(),
                smokeRotation = Rand.Range(-9f, 9f)
            };
            animation.active.Add(instance);
            animation.SpawnOpening(instance);
        }

        private void ResolveDefs()
        {
            if (smokeDef != null)
                return;
            smokeDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_AshSmoke");
            hazeDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_AshHaze");
            shadowDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityShadow");
            eyesDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityEyes");
            shardDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_AshFragment");
            orbDef = DefDatabase<FleckDef>.GetNamed("Anneli_Retheme_DarkSouls_HumanityOrb");
        }

        private static Shard[] CreateShards()
        {
            Shard[] result = new Shard[9];
            for (int i = 0; i < result.Length; i++)
            {
                float side = i % 2 == 0 ? -1f : 1f;
                result[i] = new Shard
                {
                    start = Rand.RangeInclusive(43, 70),
                    flight = Rand.RangeInclusive(55, 72),
                    fromX = Rand.Range(-0.14f, 0.14f),
                    fromZ = Rand.Range(0.40f, 0.57f),
                    toX = side * Rand.Range(0.46f, 1.02f),
                    toZ = Rand.Range(-0.58f, -0.23f),
                    size = Rand.Range(0.18f, 0.24f),
                    angle = Rand.Range(-50f, 50f),
                    spin = Rand.Range(-95f, 95f)
                };
            }
            return result;
        }

        private static Orb[] CreateOrbs()
        {
            Vector2[] bearings =
            {
                new Vector2(-0.82f, 0.53f),
                new Vector2(0.73f, 0.68f),
                new Vector2(-0.67f, -0.64f),
                new Vector2(0.91f, -0.32f)
            };
            Orb[] result = new Orb[bearings.Length];
            for (int i = 0; i < result.Length; i++)
            {
                Vector2 varied = bearings[i] + new Vector2(Rand.Range(-0.17f, 0.17f),
                    Rand.Range(-0.17f, 0.17f));
                result[i] = new Orb
                {
                    start = 18 + i * 7 + Rand.RangeInclusive(-2, 2),
                    direction = varied.normalized,
                    distance = Rand.Range(0.58f, 0.82f),
                    offsetX = Rand.Range(-0.09f, 0.09f),
                    offsetZ = Rand.Range(0.12f, 0.26f),
                    size = Rand.Range(0.28f, 0.34f),
                    spin = Rand.Range(-14f, 14f)
                };
            }
            return result;
        }

        private void SpawnOpening(Instance instance)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            FleckCreationData shadow = FleckMaker.GetDataStatic(center, map, shadowDef, 1.25f);
            shadow.instanceColor = new Color(1f, 1f, 1f, 0.58f);
            map.flecks.CreateFleck(shadow);

            FleckCreationData smoke = FleckMaker.GetDataStatic(
                center + new Vector3(0f, 0f, 0.04f), map, smokeDef, 1.24f);
            smoke.rotation = instance.smokeRotation;
            smoke.instanceColor = new Color(1f, 1f, 1f, 0.72f);
            smoke.velocity = new Vector3(0f, 0f, 0.08f);
            map.flecks.CreateFleck(smoke);

            // The humanity's eyes flare once, then disappear into the new pupa.
            SpawnEyes(instance.cell);
        }

        public override void MapComponentTick()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                instance.age++;
                int age = instance.age;

                if (age == 12 || age == 27 || age == 43 || age == 60)
                    SpawnHaze(instance, age);

                foreach (Shard shard in instance.shards)
                    if (age == shard.start)
                        SpawnShard(instance.cell, shard);

                foreach (Orb orb in instance.orbs)
                    if (age == orb.start)
                        SpawnOrb(instance.cell, orb);

                if (age >= DurationTicks)
                    active.RemoveAt(i);
            }
        }

        private void SpawnHaze(Instance instance, int age)
        {
            Vector3 center = instance.cell.ToVector3Shifted();
            float side = age == 12 || age == 43 ? -1f : 1f;
            FleckCreationData haze = FleckMaker.GetDataStatic(
                center + new Vector3(side * Rand.Range(0.14f, 0.25f), 0f,
                    Rand.Range(-0.05f, 0.12f)), map, hazeDef, Rand.Range(0.68f, 0.88f));
            haze.rotation = side * Rand.Range(12f, 30f);
            haze.rotationRate = side * Rand.Range(6f, 12f);
            haze.velocity = new Vector3(side * Rand.Range(0.10f, 0.17f), 0f,
                Rand.Range(0.07f, 0.13f));
            haze.instanceColor = new Color(1f, 1f, 1f, Rand.Range(0.40f, 0.55f));
            map.flecks.CreateFleck(haze);
        }

        private void SpawnShard(IntVec3 cell, Shard shard)
        {
            float seconds = shard.flight / 60f;
            Vector3 center = cell.ToVector3Shifted();
            FleckCreationData data = FleckMaker.GetDataStatic(
                center + new Vector3(shard.fromX, 0f, shard.fromZ), map,
                shardDef, shard.size);
            data.rotation = shard.angle;
            data.rotationRate = shard.spin / seconds;
            data.velocity = new Vector3((shard.toX - shard.fromX) / seconds, 0f,
                (shard.toZ - shard.fromZ) / seconds - 0.5f * ShardAccelerationZ * seconds);
            data.solidTimeOverride = Mathf.Max(0.01f, seconds - ShardFadeIn - ShardFadeOut);
            data.instanceColor = new Color(1f, 1f, 1f, 0.78f);
            map.flecks.CreateFleck(data);
        }

        private void SpawnOrb(IntVec3 cell, Orb orb)
        {
            float seconds = (DurationTicks - orb.start) / 60f;
            Vector3 center = cell.ToVector3Shifted();
            FleckCreationData data = FleckMaker.GetDataStatic(
                center + new Vector3(orb.offsetX, 0f, orb.offsetZ), map,
                orbDef, orb.size);
            data.velocity = new Vector3(orb.direction.x * orb.distance / seconds,
                0f, orb.direction.y * orb.distance / seconds);
            data.rotationRate = orb.spin;
            data.solidTimeOverride = Mathf.Max(0.01f, seconds - OrbFadeIn - OrbFadeOut);
            data.instanceColor = new Color(1f, 1f, 1f, 0.82f);
            map.flecks.CreateFleck(data);
        }

        private void SpawnEyes(IntVec3 cell)
        {
            FleckCreationData data = FleckMaker.GetDataStatic(
                cell.ToVector3Shifted(), map, eyesDef, 1.25f);
            data.instanceColor = new Color(1f, 1f, 1f, 0.88f);
            map.flecks.CreateFleck(data);
        }
    }
}

