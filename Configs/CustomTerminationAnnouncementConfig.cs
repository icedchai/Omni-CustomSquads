namespace Omni_CustomSquads.Configs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Enums;
    using ColdWaterLibrary.Types;
    using PlayerRoles;

    public class CustomTerminationAnnouncementConfig
    {
        [Description("Whether this feature set is enabled")]
        public bool IsEnabled { get; set; } = true;
        [Description("The cassie announcement when a subject dies without a valid attacker (attacker doesn't exist)")]
        public CustomAnnouncement FallbackTerminationAnnouncement { get; set; } = new CustomAnnouncement
        {
            Words = "%subject% terminated . termination cause unspecified",
            Translation = "%subject% terminated. Termination cause unspecified."
        };
        public Dictionary<OverallRoleType, CustomAnnouncement> ScpCassieString { get; set; } = new Dictionary<OverallRoleType, CustomAnnouncement>()
        {
            {
                new OverallRoleType { RoleId = (sbyte)RoleTypeId.Scp049, RoleType = TypeSystem.BaseGame, },
                new CustomAnnouncement { Words = "scp 0 4 9", Translation = "SCP-049" }
            },
            {
                new OverallRoleType { RoleId = (sbyte)RoleTypeId.Scp106, RoleType = TypeSystem.BaseGame, },
                new CustomAnnouncement { Words = "scp 1 0 6", Translation = "SCP-106" }
            },
            {
                new OverallRoleType { RoleId = (sbyte)RoleTypeId.Scp096, RoleType = TypeSystem.BaseGame, },
                new CustomAnnouncement { Words = "scp 0 9 6", Translation = "SCP-096" }
            },
            {
                new OverallRoleType { RoleId = (sbyte)RoleTypeId.Scp173, RoleType = TypeSystem.BaseGame, },
                new CustomAnnouncement { Words = "scp 1 7 3", Translation = "SCP-173" }
            },
            {
                new OverallRoleType { RoleId = (sbyte)RoleTypeId.Scp939, RoleType = TypeSystem.BaseGame, },
                new CustomAnnouncement { Words = "scp 9 3 9", Translation = "SCP-939" }
            },
        };

        [Description("Use %subject% in the announcements for the termination's name. Key is overallroletype, Value is name of Key in scp_termination_cassie_announcements.")]
        public Dictionary<OverallRoleType, string> ScpTerminationAnnouncementIndex { get; set; } = new Dictionary<OverallRoleType, string>
        {
            {
                new OverallRoleType { RoleId = 6, RoleType = TypeSystem.BaseGame},
                "civil_science"
            },
            {
                new OverallRoleType { RoleId = 1, RoleType = TypeSystem.BaseGame},
                "civil_classd"
            },
            {
                new OverallRoleType { RoleId = 14, RoleType = TypeSystem.BaseGame},
                "goi_unknown_military"
            },
            {
                new OverallRoleType { RoleId = 15, RoleType = TypeSystem.BaseGame},
                "mtf_facsec"
            },
            {
                new OverallRoleType { RoleId = 4, RoleType = TypeSystem.BaseGame},
                "mtf_epsilon11"
            },
            {
                new OverallRoleType { RoleId = 11, RoleType = TypeSystem.BaseGame},
                "mtf_epsilon11"
            },
            {
                new OverallRoleType { RoleId = 12, RoleType = TypeSystem.BaseGame},
                "mtf_epsilon11"
            },
            {
                new OverallRoleType { RoleId = 13, RoleType = TypeSystem.BaseGame},
                "mtf_epsilon11"
            },
            {
                new OverallRoleType { RoleId = 8, RoleType = TypeSystem.BaseGame},
                "goi_chaos_insurgency"
            },
            {
                new OverallRoleType { RoleId = 18, RoleType = TypeSystem.BaseGame},
                "goi_chaos_insurgency"
            },
            {
                new OverallRoleType { RoleId = 19, RoleType = TypeSystem.BaseGame},
                "goi_chaos_insurgency"
            },
            {
                new OverallRoleType { RoleId = 20, RoleType = TypeSystem.BaseGame},
                "goi_chaos_insurgency"
            },
        };

        [Description("%division% is the killer's UnitName, or 'UNKNOWN' if unavailable. %subject% is the name of the victim as defined in scp_cassie_string.")]
        public Dictionary<string, CustomAnnouncement> ScpTerminationCassieAnnouncements { get; set; } = new Dictionary<string, CustomAnnouncement>
        {
            {
                "goi_unusual_incidents_unit",
                new CustomAnnouncement { Words = "%subject% terminated by the jam_1_3 un- use jam_1_2 u all pitch_1 In Jam_15_3 Sigma dids .g4 unit", Translation = "%subject% terminated by the Unusual Incidents Unit." }
            },
            {
                "goi_chaos_insurgency",
                new CustomAnnouncement { Words = "%subject% terminated by chaosinsurgency", Translation = "%subject% terminated by Chaos Insurgency." }
            },
            {
                "goi_unknown_military",
                new CustomAnnouncement { Words = "%subject% terminated by Unknown Military Personnel", Translation = "%subject% terminated by unknown military personnel." }
            },
            {
                "mtf_epsilon11",
                new CustomAnnouncement { Words = "%subject% terminated by mtfunit epsilon 11 division %division%", Translation = "%subject% terminated by Mobile Task Force Unit Epsilon-11, division %division%."}
            },
            {
                "mtf_facsec",
                new CustomAnnouncement { Words = "%subject% containedsuccessfully containmentunit %division%", Translation = "%subject% contained successfully. Containment unit: %division%."}
            },
            {
                "civil_science",
                new CustomAnnouncement { Words = "%subject% terminated by science personnel", Translation = "%subject% terminated by Science Personnel." }
            },
            {
                "civil_classd",
                new CustomAnnouncement { Words = "%subject% terminated by classd personnel", Translation="%subject% terminated by Class-D Personnel." }
            },
        };
    }
}
