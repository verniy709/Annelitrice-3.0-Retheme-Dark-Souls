using RimWorld;
using Verse;

namespace Annelitrice.Retheme.DarkSouls
{
    internal struct PupationSite
    {
        internal Map map;
        internal IntVec3 cell;
        internal bool Valid => map != null && cell.IsValid;
    }

    // The base mod already calls these EffecterDefs at the right moments.
    // Their custom children start the longer animations without patching code.
    public sealed class SubEffecter_HumanityToPupa : SubEffecter
    {
        public SubEffecter_HumanityToPupa(SubEffecterDef def, Effecter parent) : base(def, parent) { }

        public override void SubTrigger(TargetInfo A, TargetInfo B, int overrideSpawnTick, bool force)
        {
            AshAnimation.Start(new PupationSite { map = A.Map, cell = A.Cell });
        }
    }

    public sealed class SubEffecter_PupaFirestorm : SubEffecter
    {
        public SubEffecter_PupaFirestorm(SubEffecterDef def, Effecter parent) : base(def, parent) { }

        public override void SubTrigger(TargetInfo A, TargetInfo B, int overrideSpawnTick, bool force)
        {
            FirestormHatchAnimation.Start(new PupationSite { map = A.Map, cell = A.Cell });
        }
    }

    // The base ability already invokes each ability comp with both endpoints.
    public sealed class CompProperties_DarksignTransfer : CompProperties_AbilityEffect
    {
        public CompProperties_DarksignTransfer()
        {
            compClass = typeof(CompAbilityEffect_DarksignTransfer);
        }
    }

    public sealed class CompAbilityEffect_DarksignTransfer : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (parent != null && parent.pawn != null)
                DarksignTransferAnimation.Start(parent.pawn, target.Pawn);
        }
    }
}
