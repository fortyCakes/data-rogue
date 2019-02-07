using System.Collections.Generic;

namespace data_rogue_core
{
    public class SaveState
    {
        public string Seed;

        public List<string> Entities { get; set; } = new List<string>();
        public List<string> Maps { get; set; } = new List<string>();
        public ulong Time { get; set; }

        public List<string> Messages { get; set; } = new List<string>();
    }
}