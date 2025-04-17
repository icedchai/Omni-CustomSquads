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
        public bool UseCassieAnnouncement { get; set; } = true;

        /// <summary>
        /// Gets or sets the squad name.
        /// </summary>
        public string SquadName { get; set; } = "delta4";

        /// <summary>
        /// Gets or sets the spawn wave this replaces.
        /// </summary>
        [Description("Respawn wave this will replace. Use NtfWave (or NtfMiniWave) to get the NATO divisions (Juliett-15), and ChaosWave (or ChaosMiniWave) to not. Use miniwave if you want this to replace an existing wave.")]
        public SpawnableFaction SquadType { get; set; } = SpawnableFaction.NtfWave;

        [Description("Chance this squad spawns instead of vanilla, or the other squads. Added up with vanilla spawn chance and other squad spawn chances.")]
        public int SpawnChance { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CASSIE announcement this squad will use.
        /// </summary>
        public string EntranceAnnouncement { get; set; } = "mtfunit delta 4 designated minute men division %division% hasentered";

        /// <summary>
        /// Gets or sets the subtitles this CASSIE announcement will use.
        /// </summary>
        public string EntranceAnnouncementSubs { get; set; } = "Mobile Task Force Unit Delta-4 designated 'Minutemen', division %division%, has entered the facility.";

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
            { '0', new OverallRoleType { RoleId = 6, RoleType = TypeSystem.BaseGame } },
            { '1', new OverallRoleType { RoleId = 7, RoleType = TypeSystem.BaseGame } },
        };

        [Description("Put a string of numbers or letters corresponding to the custom-role lookup system above.")]
        public string SpawnQueue { get; set; } = "0123456789";
    }
}
