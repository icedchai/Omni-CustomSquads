namespace Omni_CustomSquads.Patches
{
    using ColdWaterLibrary.Features.Extensions;
    using ColdWaterLibrary.Features.Wrappers;
    using Exiled.API.Features;
    using Footprinting;
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [HarmonyPatch(typeof(Footprint), MethodType.Constructor, new Type[] { typeof(ReferenceHub) })]
    public static class FootprintConstructorPatch
    {
        internal static Dictionary<Footprint, OverallRoleType> FootprintOverallRoleLookupTable { get; set; } = new ();

        [HarmonyPostfix]
        public static void Postfix(ReferenceHub hub, Footprint __instance)
        {
            Player player = Player.Get(hub);
            FootprintOverallRoleLookupTable.Add(__instance, player.GetOverallRoleType());
        }
    }
}
