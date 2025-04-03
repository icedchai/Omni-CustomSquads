namespace Omni_CustomSquads.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Enums;
    using ColdWaterLibrary.Types;
    using Exiled.API.Enums;

    /// <summary>
    /// A custom data type that contains all information needed to initiate a spawn wave.
    /// </summary>
    public class CustomSquad
    {
        /// <summary>
        /// Gets or sets a value indicating whether to make a CASSIE announcement upon this unit's arrival.
        /// </summary>
        [Description("Whether to make a CASSIE announcement")]
        public bool UseCassieAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets the squad name.
        /// </summary>
        [Description("Name used to refer to the squad in commands and logs")]
        public string SquadName { get; set; }

        /// <summary>
        /// Gets or sets the spawn wave this replaces.
        /// </summary>
        [Description("Respawn wave this will replace. Use NtfWave to get the NATO divisions (Juliett-15), and ChaosWave to not.")]
        public SpawnableFaction SquadType { get; set; }

        [Description("Chance this squad spawns instead of vanilla, or the other squads. Added up with vanilla spawn chance and other squad spawn chances.")]
        public int SpawnChance { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CASSIE announcement this squad will use.
        /// </summary>
        [Description("Announcement CASSIE will say when the custom squad enters")]
        public string EntranceAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets the subtitles this CASSIE announcement will use.
        /// </summary>
        public string EntranceAnnouncementSubs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use the default team vehicle (eg. CI Van, NTF Helo).
        /// </summary>
        public bool UseTeamVehicle { get; set; } = true;

        /*[Description("Schematic-related stuff")]
        public string SchematicName { get; set; }
        public RoomType SchematicRoom { get; set; }
        public Vector3 SchematicPositionOffset { get; set; }
        public Vector3 SchematicRotationOffset { get; set; }
        public float SchematicDestructionDelay { get; set; } = 40f;
        public float SpawnDelay { get; set; } = 0f;*/

        [Description("Role type corresponding to the letters in the spawn queue. ONLY PUT one character!!")]
        public Dictionary<char, OverallRoleType> CustomRoles { get; set; } = new Dictionary<char, OverallRoleType>
        {
            { '0', new OverallRoleType { RoleId = 6, RoleTypeSystem = TypeSystem.BaseGame } },
        };

        [Description("Put a string of numbers or letters corresponding to the custom-role lookup system above.")]
        public string SpawnQueue { get; set; } = "0123456789";
    }
}
