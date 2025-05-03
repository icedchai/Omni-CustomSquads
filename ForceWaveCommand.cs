namespace Omni_CustomSquads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Omni_CustomSquads.EventHandlers;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    //ForceWaveCmd.cs by icedchqi
    //Documented November 16th 2024
    public class ForceWaveCommand : ICommand
    {
        private static SquadManager SquadManager => SquadManager.Singleton;

        public string Command { get; } = "forcecustomwave";

        public string[] Aliases { get; } = new[]
        {
            "forcenextwave" ,
            "forcewave",
            "fwave",
        };

        public string Description { get; } = "Force next wave to be a specific custom squad. Only evaluates first argument.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "You do not have permission to use this command! Permission: PlayerPermissions.RoundEvents";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "List of available squads:";
                foreach (CustomSquad crew in SquadManager.RegisteredSquads)
                {
                    if (crew.SquadName == Plugin.VanillaSquad || crew is null)
                    {
                        continue;
                    }

                    response += $"\n{crew.SquadName}, {crew.SquadType}, {crew.SpawnChance} chance";
                }

                if (SquadManager.NextWaveNtf is null)
                {
                    response += "\n<color=yellow>Current squad MTF : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad MTF : {SquadManager.NextWaveNtf.SquadName}</color>";
                }

                if (SquadManager.NextWaveCi is null)
                {
                    response += "\n<color=yellow>Current squad CI : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad CI : {SquadManager.NextWaveCi.SquadName}</color>";
                }

                if (SquadManager.NextWaveNtfMini is null)
                {
                    response += "\n<color=yellow>Current squad MTFMini : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad MTFMini : {SquadManager.NextWaveNtf.SquadName}</color>";
                }

                if (SquadManager.NextWaveCiMini is null)
                {
                    response += "\n<color=yellow>Current squad CIMini : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad CIMini : {SquadManager.NextWaveCi.SquadName}</color>";
                }

                return false;
            }

            string arg0 = arguments.At(0).ToLower();
            if (!SquadManager.RegisteredSquads.Where(s => s.SquadName == arg0).Any())
            {
                response = "Please input a squad";
                return false;
            }

            CustomSquad customSquad = SquadManager.RegisteredSquads.Where(s => s.SquadName == arg0).FirstOrDefault();

            if (customSquad is null)
            {
                response = "Please input a squad";
                return false;
            }

            switch (customSquad.SquadType)
            {
                case SpawnableFaction.NtfWave:
                    SquadManager.NextWaveNtf = customSquad;
                    break;
                case SpawnableFaction.NtfMiniWave:
                    SquadManager.NextWaveNtfMini = customSquad;
                    break;
                case SpawnableFaction.ChaosWave:
                    SquadManager.NextWaveCi = customSquad;
                    break;
                case SpawnableFaction.ChaosMiniWave:
                    SquadManager.NextWaveCiMini = customSquad;
                    break;
            }

            response = $"Set {customSquad.SquadType} spawnwave to {customSquad.SquadName}";

            Log.Info($"{sender.LogName} {response}");
            return true;
        }
    }
}
