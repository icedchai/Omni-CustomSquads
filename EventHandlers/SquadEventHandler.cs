namespace Omni_CustomSquads.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Extensions;
    using ColdWaterLibrary.Types;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles;
    using Respawning.NamingRules;
    using Utils;

    public class SquadEventHandler
    {
        private SquadManager SquadManager => SquadManager.Singleton;

        /// <summary>
        /// Registers events.
        /// </summary>
        public void RegisterEvents()
        {
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
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance -= OnChaosAnnouncing;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnNtfAnnouncing;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnSpawnWave;
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

                OverallRoleType roleType;
                if (!customSquad.CustomRoles.TryGetValue(c, out roleType))
                {
                    Log.Info($"Couldn't find the specified role of Key {c} in {customSquad.SquadName}'s roles.");
                    break;
                }

                Player player = players.RandomItem();
                Timing.CallDelayed(0.01f, () => player.SetOverallRoleType(roleType));
                players.Remove(player);
                Log.Info($"Spawned {player} for {customSquad.SquadName}");
            }

            if (customSquad.UseCassieAnnouncement)
            {
                // This delay ensures that the plugin grabs the correct "latest" Unit Name
                Timing.CallDelayed(0.01f, () =>
                {
                    string announcement = customSquad.EntranceAnnouncement;
                    string announcementSubs = customSquad.EntranceAnnouncementSubs;

                    if (NamingRulesManager.TryGetNamingRule(Team.FoundationForces, out UnitNamingRule rule))
                    {
                        announcement = announcement.Replace("%division%", rule.TranslateToCassie(rule.LastGeneratedName));
                        announcementSubs = announcementSubs.Replace("%division%", rule.LastGeneratedName);
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
            Queue<RoleTypeId> queue = e.SpawnQueue;
            if (players.Count == 0)
            {
                return;
            }

            switch (e.Wave.SpawnableFaction)
            {
                case SpawnableFaction.ChaosWave:
                    {
                        customSquad = SquadManager.NextWaveCi;

                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveCi = SquadManager.CiPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveCi;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
                        }

                        HandleSpawnWave(customSquad, e.Players);
                        break;
                    }

                case SpawnableFaction.NtfWave:
                    {
                        customSquad = SquadManager.NextWaveNtf;
                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveNtf = SquadManager.NtfPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveNtf;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
                        }

                        HandleSpawnWave(customSquad, players);
                        break;
                    }

                case SpawnableFaction.ChaosMiniWave:
                    {
                        customSquad = SquadManager.NextWaveCiMini;

                        if (customSquad.SquadName == Plugin.VanillaSquad)
                        {
                            return;
                        }

                        if (customSquad is null)
                        {
                            SquadManager.NextWaveCiMini = SquadManager.CiMiniPool.GetRandomSquad();
                            customSquad = SquadManager.NextWaveCiMini;
                            if (customSquad is null || customSquad.SquadName == Plugin.VanillaSquad)
                            {
                                return;
                            }
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
                AnnounceSubjectDeath(e.Attacker, e.Player);
            }
        }

        /// <summary>
        /// Announces a subject's death.
        /// </summary>
        /// <param name="attacker">The player who killed the victim.</param>
        /// <param name="victim">The player whose death is being announced.</param>
        private void AnnounceSubjectDeath(Player attacker, Player victim)
        {
            CustomAnnouncement subjectName = null;
            if (victim is null)
            {
                return;
            }

            if (attacker is null)
            {
                foreach (OverallRoleType newType in Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpCassieString.Keys)
                {
                    if (victim.HasOverallRoleType(newType))
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

                CustomAnnouncement fallback = Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.FallbackTerminationAnnouncement;
                Cassie.MessageTranslated(fallback.Words.Replace("%subject%", subjectName.Words), fallback.Translation.Replace("%subject%", subjectName.Translation));
                return;
            }

            string announcementName = null;
            foreach (OverallRoleType newType in Plugin.Singleton.Config.CustomTerminationAnnouncementConfig.ScpTerminationAnnouncementIndex.Keys)
            {
                if (attacker.HasOverallRoleType(newType))
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
                if (victim.HasOverallRoleType(newType))
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
            if (!string.IsNullOrWhiteSpace(attacker.UnitName) && NamingRulesManager.TryGetNamingRule(Team.FoundationForces, out UnitNamingRule rule))
            {
                cassie = cassie.Replace("%division%", rule.TranslateToCassie(attacker.UnitName));
                subs = subs.Replace("%division%", attacker.UnitName);
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
