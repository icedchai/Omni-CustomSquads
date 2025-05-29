namespace Omni_CustomSquads.Patches
{
    using HarmonyLib;
    using Respawning.Announcements;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class SubtitlePreventionPatch
    {
        [HarmonyPatch(nameof(WaveAnnouncementBase.SendSubtitles))]
        [HarmonyPatch(typeof(NtfWaveAnnouncement))]
        [HarmonyPrefix]
        public static bool NtfPrefix() => SquadManager.Singleton.IsNtfVanilla;

        [HarmonyPatch(typeof(NtfMiniwaveAnnouncement))]
        [HarmonyPrefix]
        public static bool NtfMiniPrefix() => SquadManager.Singleton.IsNtfMiniVanilla;

        [HarmonyPatch(typeof(ChaosWaveAnnouncement))]
        [HarmonyPrefix]
        public static bool ChaosPrefix() => SquadManager.Singleton.IsCiVanilla;

        [HarmonyPatch(typeof(ChaosMiniwaveAnnouncement))]
        [HarmonyPrefix]
        public static bool ChaosMiniPrefix() => SquadManager.Singleton.IsCiMiniVanilla;
    }
}
