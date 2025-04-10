namespace Omni_CustomSquads.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Extensions;
    using ColdWaterLibrary.Types;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles;
    using Respawning.NamingRules;

    public static class SquadEventHandler
    {
        /// <summary>
        /// Makes Unit Name readable for CASSIE.
        /// </summary>
        /// <param name="unit">The unit name.</param>
        /// <returns>A CASSIE-readable unit-name.</returns>
        public static string MakeUnitNameReadable(string unit)
        {
            string output = string.Empty;
            string[] thing = unit.Split('-'); // thing = ["HOTEL", "09"]
            output += $"nato_{unit[0]} {(thing[1][0] == '0' ? thing[1][1] : thing[1])}"; // output = "nato_H 09"
            return output;
        }

        /// <summary>
        /// Squad chance pool.
        /// </summary>
        internal class SquadPool
        {
            private Dictionary<CustomSquad, int> entries = new();
            private int accumulatedWeight = 0;

            /// <summary>
            /// Clears entries.
            /// </summary>
            public void ClearEntries()
            {
                entries.Clear();
            }

            public List<CustomSquad> RegisteredSquads => entries.Keys.ToList();

            public void AddEntry(CustomSquad customSquad, int weight)
            {
                accumulatedWeight += weight;
                customSquad.SquadName = customSquad.SquadName.ToLower();

                entries.Add(customSquad, accumulatedWeight);
            }

            public CustomSquad GetRandomSquad()
            {
                float r = UnityEngine.Random.Range(0f, 1f) * accumulatedWeight;

                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries.Values.ToList()[i] >= r)
                    {
                        return entries.Keys.ToList()[i];
                    }
                }

                return null; // should only happen when there are no entries
            }
        }

        internal static SquadPool NtfPool { get; set; } = new SquadPool();

        internal static SquadPool CiPool { get; set; } = new SquadPool();

        public static List<CustomSquad> RegisteredSquads => NtfPool.RegisteredSquads.Concat(CiPool.RegisteredSquads).ToList();

        /// <summary>
        /// Event handler for SpawningTeamVehicleEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        public static void OnTeamVehicleSpawning(SpawningTeamVehicleEventArgs e)
        {
            if (e.Team.TargetFaction == Faction.FoundationStaff)
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
            }
        }

        /// <summary>
        /// Event handler for AnnouncingChaosEntranceEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        public static void OnChaosAnnouncing(AnnouncingChaosEntranceEventArgs e)
        {
            Log.Debug("Announcing CHAOS ENTRANCE");
            if (Plugin.NextWaveCi is null || Plugin.NextWaveCi.SquadName == Plugin.VanillaSquad)
            {
                return;
            }

            Log.Debug("Announcing CHAOS ENTRANCE: Custom Squad Detected");
            // Timing.CallDelayed(0.01f, Cassie.Clear);
            Plugin.NextWaveCi = null;
            e.IsAllowed = false;
        }

        /// <summary>
        /// Event handler for AnnouncingNtfEntranceEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        public static void OnNtfAnnouncing(AnnouncingNtfEntranceEventArgs e)
        {
            Log.Debug("Announcing NTF ENTRANCE");
            if (Plugin.NextWaveNtf is null || Plugin.NextWaveNtf.SquadName == Plugin.VanillaSquad)
            {
                Plugin.NextWaveNtf = null;
                return;
            }

            Log.Debug("Announcing NTF ENTRANCE: Custom Squad Detected");
            Plugin.NextWaveNtf = null;
            e.IsAllowed = false;
        }

        private static void HandleSpawnWave(CustomSquad customSquad, List<Player> players)
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

                    announcement = announcement.Replace("%division%", MakeUnitNameReadable(NamingRulesManager.GeneratedNames[Team.FoundationForces].LastOrDefault()));
                    announcementSubs = announcementSubs.Replace("%division%", NamingRulesManager.GeneratedNames[Team.FoundationForces].LastOrDefault());
                    Cassie.MessageTranslated(announcement, announcementSubs);
                });
            }
        }

        /// <summary>
        /// Event handler for RespawningTeamEventArgs.
        /// </summary>
        /// <param name="e">The event args.</param>
        public static void OnSpawnWave(RespawningTeamEventArgs e)
        {
            CustomSquad customSquad;
            List<Player> players = e.Players;
            Queue<RoleTypeId> queue = e.SpawnQueue;
            if (players.Count == 0)
            {
                Log.Debug("aa");
                return;
            }

            Log.Debug("bb");
            switch (e.NextKnownTeam)
            {
                case Faction.FoundationEnemy:
                    {
                        customSquad = Plugin.NextWaveCi;
                        if (customSquad is null)
                        {
                            Plugin.NextWaveCi = CiPool.GetRandomSquad();
                            customSquad = Plugin.NextWaveCi;
                            if (customSquad.SquadName == Plugin.VanillaSquad || customSquad is null)
                            {
                                return;
                            }
                        }

                        HandleSpawnWave(customSquad, e.Players);
                        break;
                    }

                case Faction.FoundationStaff:
                    {
                        customSquad = Plugin.NextWaveNtf;

                        if (customSquad is null)
                        {
                            Plugin.NextWaveNtf = NtfPool.GetRandomSquad();
                            customSquad = Plugin.NextWaveNtf;
                            if (customSquad.SquadName == Plugin.VanillaSquad || customSquad is null)
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
    }
}
