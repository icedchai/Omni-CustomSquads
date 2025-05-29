namespace Omni_CustomSquads
{
    using Exiled.API.Features;
    using Omni_CustomSquads.EventHandlers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Manages the CustomSquads.
    /// </summary>
    public class SquadManager
    {
        /// <summary>
        /// The <see cref="SquadManager"/> singleton.
        /// </summary>
        public static SquadManager Singleton { get; private set; } = new SquadManager();

        /// <summary>
        /// Gets a value indicating whether the NTF Wave is currently vanilla.
        /// </summary>
        public bool IsNtfVanilla => NextWaveNtf == null || NextWaveNtf.SquadName == Plugin.VanillaSquad;

        /// <summary>
        /// Gets a value indicating whether the NTF Miniwave is currently vanilla.
        /// </summary>
        public bool IsNtfMiniVanilla => NextWaveNtfMini == null || NextWaveNtfMini.SquadName == Plugin.VanillaSquad;

        /// <summary>
        /// Gets a value indicating whether the Ci Wave is currently vanilla.
        /// </summary>
        public bool IsCiVanilla => NextWaveCi == null || NextWaveCi.SquadName == Plugin.VanillaSquad;

        /// <summary>
        /// Gets a value indicating whether the Ci Miniwave is currently vanilla.
        /// </summary>
        public bool IsCiMiniVanilla => NextWaveCiMini == null || NextWaveCiMini.SquadName == Plugin.VanillaSquad;

        /// <summary>
        /// Gets the next CustomSquad set to spawn.
        /// </summary>
        public CustomSquad NextWaveNtf { get; internal set; }

        /// <summary>
        /// Gets the next CustomSquad set to spawn.
        /// </summary>
        public CustomSquad NextWaveCi { get; internal set; }
        /// <summary>
        /// Gets the next CustomSquad set to spawn.
        /// </summary>
        public CustomSquad NextWaveNtfMini { get; internal set; }

        /// <summary>
        /// Gets the next CustomSquad set to spawn.
        /// </summary>
        public CustomSquad NextWaveCiMini { get; internal set; }

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
                float r = UnityEngine.Random.Range(0.01f, 1f) * accumulatedWeight;

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

        internal SquadPool NtfPool { get; set; } = new SquadPool();

        internal SquadPool CiPool { get; set; } = new SquadPool();

        internal SquadPool NtfMiniPool { get; set; } = new SquadPool();

        internal SquadPool CiMiniPool { get; set; } = new SquadPool();

        public List<CustomSquad> RegisteredSquads => NtfPool.RegisteredSquads.Concat(CiPool.RegisteredSquads).Concat(CiMiniPool.RegisteredSquads).Concat(NtfMiniPool.RegisteredSquads).ToList();
    }
}
