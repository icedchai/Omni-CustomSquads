namespace Omni_CustomSquads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.Handlers;
    using Omni_CustomSquads.EventHandlers;
    using PlayerRoles;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        /// <summary>
        /// Vanilla squad name.
        /// </summary>
        public const string VanillaSquad = "vaniller";

        /// <summary>
        /// The <see cref="Plugin"/> singleton.
        /// </summary>
        public static Plugin Singleton;

        /// <summary>
        /// Gets the <see cref="Singleton"/>'s config.
        /// </summary>
        public static Config config => Singleton.Config;
        /// <summary>
        /// Gets the next CustomSquad set to spawn.
        /// </summary>
        public static CustomSquad NextWaveNtf { get; internal set; }

        /// <summary>
        /// Gets the next CustomSquad set to spawn.
        /// </summary>
        public static CustomSquad NextWaveCi { get; internal set; }

        /// <inheritdoc/>
        public override string Name => "Omni Custom Squads";

        /// <inheritdoc/>
        public override string Author => "icedchqi";

        /// <inheritdoc/>
        public override string Prefix => "omni_customsquads";

        /// <inheritdoc/>
        public override Version Version => new Version(1, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Singleton = this;

            if (Config.CustomSquads.Count == 0)
            {
                return;
            }

            CustomSquad vanilla = new CustomSquad { SquadName = VanillaSquad, UseCassieAnnouncement = true };

            if (Config.CiVanillaChance > 0)
            {
                SquadEventHandler.CiPool.AddEntry(vanilla, Config.CiVanillaChance);
            }

            if (Config.NtfVanillaChance > 0)
            {
                SquadEventHandler.NtfPool.AddEntry(vanilla, Config.NtfVanillaChance);
            }

            for (int i = 0; i < Config.CustomSquads.Count; i++)
            {
                CustomSquad squad = Config.CustomSquads[i];
                if (squad.SpawnChance == 0)
                {
                    return;
                }

                squad.SquadName = squad.SquadName.ToLower();
                // Shouldn't cause any issues unless the user has typed an infinite amount of the same squad...
                switch (squad.SquadType.GetFaction())
                {
                    case Faction.FoundationStaff:
                        SquadEventHandler.NtfPool.AddEntry(squad, squad.SpawnChance);
                        break;
                    case Faction.FoundationEnemy:
                        SquadEventHandler.CiPool.AddEntry(squad, squad.SpawnChance);
                        break;
                }

                Log.Debug($"Registered squad {squad.SquadName} with chance {squad.SpawnChance} under {squad.SquadType}");

                Log.Info($"{squad.SquadName} registered under id {i}");
            }

            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance += SquadEventHandler.OnChaosAnnouncing;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += SquadEventHandler.OnNtfAnnouncing;
            Exiled.Events.Handlers.Server.RespawningTeam += SquadEventHandler.OnSpawnWave;
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance -= SquadEventHandler.OnChaosAnnouncing;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= SquadEventHandler.OnNtfAnnouncing;
            Exiled.Events.Handlers.Server.RespawningTeam -= SquadEventHandler.OnSpawnWave;
            SquadEventHandler.CiPool.ClearEntries();
            SquadEventHandler.NtfPool.ClearEntries();
            Singleton = null;
        }
    }
}
