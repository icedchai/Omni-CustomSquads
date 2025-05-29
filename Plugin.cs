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
    using Omni_CustomSquads.Configs;
    using PlayerRoles;
    using Omni_CustomSquads.Patches;

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

        /// <inheritdoc/>
        public override string Name => "Omni Custom Squads";

        /// <inheritdoc/>
        public override string Author => "icedchqi";

        /// <inheritdoc/>
        public override string Prefix => "omni_customsquads";

        /// <inheritdoc/>
        public override Version Version => new Version(1, 2, 1);

        private SquadManager SquadManager => SquadManager.Singleton;

        private SquadEventHandler SquadEventHandler { get; set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Singleton = this;

            Log.Debug($"{Config.CustomSquads.Count}");

            Patcher.DoPatching();

            CustomSquad vanilla = new CustomSquad { SquadName = VanillaSquad, UseCassieAnnouncement = true };

            if (Config.CiVanillaChance > 0)
            {
                SquadManager.CiPool.AddEntry(vanilla, Config.CiVanillaChance);
            }

            if (Config.NtfVanillaChance > 0)
            {
                SquadManager.NtfPool.AddEntry(vanilla, Config.NtfVanillaChance);
            }

            if (Config.CiMiniVanillaChance > 0)
            {
                SquadManager.CiMiniPool.AddEntry(vanilla, Config.CiMiniVanillaChance);
            }

            if (Config.NtfMiniVanillaChance > 0)
            {
                SquadManager.NtfMiniPool.AddEntry(vanilla, Config.NtfMiniVanillaChance);
            }

            for (int i = 0; i < Config.CustomSquads.Count; i++)
            {
                CustomSquad squad = Config.CustomSquads[i];

                squad.SquadName = squad.SquadName.ToLower();
                switch (squad.SquadType)
                {
                    case SpawnableFaction.NtfWave:
                        SquadManager.NtfPool.AddEntry(squad, squad.SpawnChance);
                        break;
                    case SpawnableFaction.ChaosWave:
                        SquadManager.CiPool.AddEntry(squad, squad.SpawnChance);
                        break;
                    case SpawnableFaction.NtfMiniWave:
                        SquadManager.NtfMiniPool.AddEntry(squad, squad.SpawnChance);
                        break;
                    case SpawnableFaction.ChaosMiniWave:
                        SquadManager.CiMiniPool.AddEntry(squad, squad.SpawnChance);
                        break;
                }

                Log.Debug($"Registered squad {squad.SquadName} with chance {squad.SpawnChance} under {squad.SquadType}");

                Log.Info($"{squad.SquadName} registered under id {i}");
            }

            SquadEventHandler = new SquadEventHandler();
            SquadEventHandler.RegisterEvents();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            SquadManager.CiPool.ClearEntries();
            SquadManager.NtfPool.ClearEntries();
            SquadManager.CiMiniPool.ClearEntries();
            SquadManager.NtfMiniPool.ClearEntries();
            SquadEventHandler.UnregisterEvents();
            SquadEventHandler = null;
            Singleton = null;
        }
    }
}
