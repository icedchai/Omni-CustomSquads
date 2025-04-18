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
                foreach (CustomSquad crew in SquadEventHandler.RegisteredSquads)
                {
                    if (crew.SquadName == Plugin.VanillaSquad || crew is null)
                    {
                        continue;
                    }

                    response += $"\n{crew.SquadName}, {crew.SquadType}, {crew.SpawnChance} chance";
                }

                if (Plugin.NextWaveNtf is null)
                {
                    response += "\n<color=yellow>Current squad MTF : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad MTF : {Plugin.NextWaveNtf.SquadName}</color>";
                }

                if (Plugin.NextWaveCi is null)
                {
                    response += "\n<color=yellow>Current squad CI : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad CI : {Plugin.NextWaveCi.SquadName}</color>";
                }

                if (Plugin.NextWaveNtfMini is null)
                {
                    response += "\n<color=yellow>Current squad MTFMini : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad MTFMini : {Plugin.NextWaveNtf.SquadName}</color>";
                }

                if (Plugin.NextWaveCiMini is null)
                {
                    response += "\n<color=yellow>Current squad CIMini : NONE</color>";
                }
                else
                {
                    response += $"\n<color=yellow>Current squad CIMini : {Plugin.NextWaveCi.SquadName}</color>";
                }

                return false;
            }

            string arg0 = arguments.At(0).ToLower();
            if (!SquadEventHandler.RegisteredSquads.Where(s => s.SquadName == arg0).Any())
            {
                response = "Please input a squad";
                return false;
            }

            CustomSquad customSquad = SquadEventHandler.RegisteredSquads.Where(s => s.SquadName == arg0).FirstOrDefault();
            if (customSquad.SquadType == SpawnableFaction.NtfWave)
            {
                Plugin.NextWaveNtf = customSquad;
                response = $"Set next MTF Spawnwave to {arg0}";
            }
            else if (customSquad.SquadType == SpawnableFaction.ChaosWave)
            {
                Plugin.NextWaveCi = customSquad;
                response = $"Set next CI Spawnwave to {arg0}";
            }
            else if (customSquad.SquadType == SpawnableFaction.NtfMiniWave)
            {
                Plugin.NextWaveNtfMini = customSquad;
                response = $"Set next MTF Miniwave to {arg0}";
            }
            else if (customSquad.SquadType == SpawnableFaction.ChaosMiniWave)
            {
                Plugin.NextWaveCiMini = customSquad;
                response = $"Set next CI Miniwave to {arg0}";
            }
            else
            {
                response = "Please input a squad";
                return false;
            }

            Log.Info($"{sender.LogName} {response}");
            return true;
        }
    }
}
