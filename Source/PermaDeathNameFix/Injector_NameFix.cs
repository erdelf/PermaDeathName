using RimWorld;
using Verse;

namespace PermaDeathNameFix
{
    using JetBrains.Annotations;

    [StaticConstructorOnStartup]
    [UsedImplicitly]
    public class Injector_NameFix
    {
        static Injector_NameFix()
        {
            Detour.TryDetourFromTo(source: typeof(NamePlayerFactionDialogUtility).GetMethod(name: nameof(NamePlayerFactionDialogUtility.IsValidName)),
                destination: typeof(ReplacementCode).GetMethod(name: nameof(ReplacementCode._IsValidName)));
            Detour.TryDetourFromTo(source: typeof(PermadeathModeUtility).GetMethod(name: nameof(PermadeathModeUtility.CheckUpdatePermadeathModeUniqueNameOnGameLoad)),
                destination: typeof(ReplacementCode).GetMethod(name: nameof(ReplacementCode._CheckUpdatePermadeathModeUniqueNameOnGameLoad)));
        }
    }
}