namespace Omni_CustomSquads.Patches
{
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Patcher
    {
        /// <summary>
        /// Do patching.
        /// </summary>
        public static void DoPatching()
        {
            var harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.PatchAll();
        }
    }
}
