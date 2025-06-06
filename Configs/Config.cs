﻿namespace Omni_CustomSquads.Configs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Interfaces;
    using Omni_CustomSquads.EventHandlers;

    /// <summary>
    /// Config class.
    /// </summary>
    public class Config : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is in debug mode.
        /// </summary>
        public bool Debug { get; set; } = false;

        public int CiVanillaChance { get; set; } = 100;

        public int NtfVanillaChance { get; set; } = 100;

        public int CiMiniVanillaChance { get; set; } = 100;

        public int NtfMiniVanillaChance { get; set; } = 100;

        public CustomAnnouncement ThreatOverviewNoScps { get; set; } = new CustomAnnouncement { Words = "noscpsleft", Translation = "Substantial threat to safety remains within the facility -- exercise caution." };

        public CustomAnnouncement ThreatOverviewOneScp { get; set; } = new CustomAnnouncement { Words = "awaitingrecontainment 1 scpsubject", Translation = "Awaiting recontainment of: 1 SCP subject." };

        public CustomAnnouncement ThreatOverviewScps { get; set; } = new CustomAnnouncement { Words = "awaitingrecontainment %scps% scpsubjects", Translation = "Awaiting recontainment of: %scps% SCP subjects." };

        /// <summary>
        /// Gets or sets the list of <see cref="CustomSquad"/> to be registered.
        /// </summary>
        public List<CustomSquad> CustomSquads { get; set; } = new List<CustomSquad>
        {
            new CustomSquad(),
        };

        public CustomTerminationAnnouncementConfig CustomTerminationAnnouncementConfig { get; set; } = new ();
    }
}
