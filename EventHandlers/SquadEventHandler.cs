namespace Omni_CustomSquads.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Omni_CustomSquads.Configs;
    using ColdWaterLibrary.Features.Extensions;
    using ColdWaterLibrary.Features.Wrappers;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles;
    using Respawning.NamingRules;
    using Utils;
    using Exiled.API.Features.DamageHandlers;
    using Footprinting;
    using Omni_CustomSquads.Patches;

    public class SquadEventHandler
    {
        private SquadManager SquadManager => SquadManager.Singleton;

        /// <summary>
        /// Registers events.
        /// </summary>
        public void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += Reset;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination += OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance += OnChaosAnnouncing;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnNtfAnnouncing;
            Exiled.Events.Handlers.Server.RespawningTeam += OnSpawnWave;
        }

        /// <summary>
        /// Unregisters events.
        /// </summary>
        public void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= Reset;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance -= OnChaosAnnouncing;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnNtfAnnouncing;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnSpawnWave;
        }

        private void Reset()
        {
            FootprintConstructorPatch.FootprintOverallRoleLookupTable.Clear();
        }

        /// <summary>
        /// Event handler for SpawningTeamVehicleEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        private void OnTeamVehicleSpawning(SpawningTeamVehicleEventArgs e)
        {
            // TODO: Fix this.
            /*if (e.Team.TargetFaction == Faction.FoundationStaff)
            {
                if (Plugin.NextWaveNtf is null)
                {
                    Plugin.NextWaveNtf = NtfPool.GetRandomSquad();
                }

                if (Plugin.NextWaveNtf.UseTeamVehicle || Plugin.NextWaveNtf.SquadName == Plugin.VanillaSquad)
                {
                    return;
                }

                e.IsAllowed = false;
            }
            else
            {
                if (Plugin.NextWaveCi is null)
                {
                    Plugin.NextWaveCi = CiPool.GetRandomSquad();
                }

                if (Plugin.NextWaveCi.UseTeamVehicle || Plugin.NextWaveCi.SquadName == Plugin.VanillaSquad)
                {
                    return;
                }

                e.IsAllowed = false;
            }*/
        }

        /// <summary>
        /// Event handler for AnnouncingChaosEntranceEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        private void OnChaosAnnouncing(AnnouncingChaosEntranceEventArgs e)
        {

            if (e.Wave.IsMiniWave)
            {
                if (SquadManager.NextWaveCiMini is null || SquadManager.NextWaveCiMini.SquadName == Plugin.VanillaSquad)
                {
                    SquadManager.NextWaveCiMini = null;
                    return;
                }

                Log.Debug("Announcing Ci ENTRANCE: Custom Squad Detected");
                SquadManager.NextWaveCiMini = null;
                e.IsAllowed = false;
                return;
            }
            else
            {
                if (SquadManager.NextWaveCi is null || SquadManager.NextWaveCi.SquadName == Plugin.VanillaSquad)
                {
                    SquadManager.NextWaveCi = null;
                    return;
                }

                Log.Debug("Announcing Ci ENTRANCE: Custom Squad Detected");
                SquadManager.NextWaveCi = null;
                e.IsAllowed = false;
            }
        }

        /// <summary>
        /// Event handler for AnnouncingNtfEntranceEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        private void OnNtfAnnouncing(AnnouncingNtfEntranceEventArgs e)
        {
            Log.Debug("Announcing NTF ENTRANCE");

            if (e.Wave.IsMiniWave)
            {
                if (SquadManager.NextWaveNtfMini is null || SquadManager.NextWaveNtfMini.SquadName == Plugin.VanillaSquad)
                {
                    SquadManager.NextWaveNtfMini = null;
                    return;
                }

                Log.Debug("Announcing NTF ENTRANCE: Custom Squad Detected");
                SquadManager.NextWaveNtfMini = null;
                e.IsAllowed = false;
                return;
            }
            else
            {
                if (SquadManager.NextWaveNtf is null || SquadManager.NextWaveNtf.SquadName == Plugin.VanillaSquad)
                {
                    SquadManager.NextWaveNtf = null;
                    return;
                }

                Log.Debug("Announcing NTF ENTRANCE: Custom Squad Detected");
                SquadManager.NextWaveNtf = null;
                e.IsAllowed = false;
            }
        }

        private void HandleSpawnWave(CustomSquad customSquad, List<Player> players)
        {
            foreach (char c in customSquad.SpawnQueue)
            {
                if (players.IsEmpty())
                {
                    Log.Info($"Finished spawning {customSquad.SquadName}");
                    break;
                }

                if (!customSquad.CustomRoles.TryGetValue(c, out string roleType))
                {
                    Log.Info($"Couldn't find the specified role of Key {c} in {customSquad.SquadName}'s roles.");
                    break;
                }

                Player player = players.RandomItem();
                Timing.CallDelayed(0.01f, () => player.SetOverallRoleType(roleType));
                players.Remove(player);
            }

            players.Clear();

            if (customSquad.UseCassieAnnouncement)
            {
                // This delay ensures that the plugin grabs the correct "latest" Unit Name
                Timing.CallDelayed(0.01f, () =>
                {
                    string announcement = customSquad.EntranceAnnouncement;
                    string announcementSubs = customSquad.EntranceAnnouncementSubs;
                    string threatOverview;
                    string threatOverviewSubs;
                    int scpCount = Player.List.Where(p => p.IsScp && p.Role != RoleTypeId.Scp0492).Count();
                    switch (scpCount)
                    {
                        case 0:
                            threatOverview = Plugin.config.ThreatOverviewNoScps.Words;
                            threatOverviewSubs = Plugin.config.ThreatOverviewNoScps.Translation;
                            break;
                        case 1:
                            threatOverview = Plugin.config.ThreatOverviewOneScp.Words;
                            threatOverviewSubs = Plugin.config.ThreatOverviewOneScp.Translation;
                            break;
                        default:
                            threatOverview = Plugin.config.ThreatOverviewScps.Words;
                            threatOverviewSubs = Plugin.config.ThreatOverviewScps.Translation;
                            break;
                    }

                    if (NamingRulesManager.TryGetNamingRule(Team.FoundationForces, out UnitNamingRule rule))
                    {
                        announcement = announcement.Replace("%division%", rule.TranslateToCassie(rule.LastGeneratedName)).Replace("%threat%", threatOverview).Replace("%scps%", $"{scpCount}");
                        announcementSubs = announcementSubs.Replace("%division%", rule.LastGeneratedName).Replace("%threat%", threatOverviewSubs).Replace("%scps%", $"{scpCount}");
                    }

                    Cassie.MessageTranslated(announcement, announcementSubs);
                });
            }
        }

        /// <summary>
        /// Event handler for RespawningTeamEventArgs.
        /// </summary>
        /// <param name="e">The event args.</param>
        private void OnSpawnWave(RespawningTeamEventArgs e)
        {
            CustomSquad customSquad;
            List<Player> players = e.Players;
            if (players.Count == 0)
            {
                return;
            }

            switch (e.Wave.SpawnableFaction)
            {
                case SpawnableFaction.ChaosWave:
                    {
                        customSquad = SquadManager.NextWaveCi;

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveCi = SquadManager.CiPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveCi;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
                        }

                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        HandleSpawnWave(customSquad, e.Players);
                        break;
                    }

                case SpawnableFaction.NtfWave:
                    {
                        customSquad = SquadManager.NextWaveNtf;

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveNtf = SquadManager.NtfPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveNtf;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
                        }

                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        HandleSpawnWave(customSquad, players);
                        break;
                    }

                case SpawnableFaction.ChaosMiniWave:
                    {
                        customSquad = SquadManager.NextWaveCiMini;

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveCiMini = SquadManager.CiMiniPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveCiMini;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
                        }

                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        HandleSpawnWave(customSquad, e.Players);
                        break;
                    }

                case SpawnableFaction.NtfMiniWave:
                    {
                        customSquad = SquadManager.NextWaveNtfMini;

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveNtfMini = SquadManager.NtfMiniPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveNtfMini;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
                        }

                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        HandleSpawnWave(customSquad, players);
                        break;
                    }

                default:
                    return;
            }
        }

        /// <summary>
        /// Event handler for AnnouncingScpTerminationEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs e)
        {
            if (!Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.IsEnabled)
            {
                return;
            }

            // Disables SCP termination announcements
            e.IsAllowed = false;
        }

        /// <summary>
        /// Event handler for DyingEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        private void OnDying(DyingEventArgs e)
        {
            if (Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.IsEnabled)
            {
                AnnounceSubjectDeath(e.DamageHandler);
            }
        }

        /// <summary>
        /// Announces a subject's death.
        /// </summary>
        /// <param name="attacker">The player who killed the victim.</param>
        /// <param name="victim">The player whose death is being announced.</param>
        private void AnnounceSubjectDeath(CustomDamageHandler damageHandler)
        {
            CustomAnnouncement subjectName = null;
            Player player = damageHandler.Target;
            Player attacker = damageHandler.Attacker;

            if (attacker is null)
            {
                foreach (OverallRoleType newType in Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpCassieString.Keys)
                {
                    if (player.HasOverallRoleType(newType))
                    {
                        if (!Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpCassieString.TryGetValue(newType, out subjectName))
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (subjectName is null)
                {
                    return;
                }

                CustomAnnouncement deathAnnouncement;
                if (!Plugin.config.CustomTerminationAnnouncementConfig.NoAttackerTerminationMessages.TryGetValue(damageHandler.Type, out deathAnnouncement))
                {
                    deathAnnouncement = Plugin.config.CustomTerminationAnnouncementConfig.FallbackTerminationAnnouncement;
                }

                Cassie.MessageTranslated(deathAnnouncement.Words.Replace("%subject%", subjectName.Words), deathAnnouncement.Translation.Replace("%subject%", subjectName.Translation));
                return;
            }

            string announcementName = null;
            OverallRoleType attackerRoleType = FootprintConstructorPatch.FootprintOverallRoleLookupTable[damageHandler.AttackerFootprint];

            foreach (OverallRoleType newType in Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpTerminationAnnouncementIndex.Keys)
            {
                if (attackerRoleType == newType)
                {
                    if (!Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpTerminationAnnouncementIndex.TryGetValue(newType, out announcementName))
                    {
                        return;
                    }
                }
            }

            string cassie;
            string subs;
            CustomAnnouncement announcement;
            if (!Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpTerminationCassieAnnouncements.TryGetValue(announcementName, out announcement))
            {
                return;
            }

            cassie = announcement.Words;
            subs = announcement.Translation;
            foreach (OverallRoleType newType in Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpCassieString.Keys)
            {
                if (player.HasOverallRoleType(newType))
                {
                    if (!Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpCassieString.TryGetValue(newType, out subjectName))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (subjectName is null)
            {
                return;
            }

            cassie = cassie.Replace("%subject%", subjectName.Words);
            subs = subs.Replace("%subject%", subjectName.Translation);

            // Make sure unit name is not empty.
            if (!string.IsNullOrWhiteSpace(damageHandler.AttackerFootprint.UnitName) && NamingRulesManager.TryGetNamingRule(Team.FoundationForces, out UnitNamingRule rule))
            {
                cassie = cassie.Replace("%division%", rule.TranslateToCassie(damageHandler.AttackerFootprint.UnitName));
                subs = subs.Replace("%division%", damageHandler.AttackerFootprint.UnitName);
            }
            else
            {
                cassie = cassie.Replace("%division%", "unknown");
                subs = subs.Replace("%division%", "UNKNOWN");
            }

            Cassie.MessageTranslated(cassie, subs);
        }
    }
}
