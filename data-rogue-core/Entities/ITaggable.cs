using System.Collections.Generic;

namespace data_rogue_core.Entities
{
    public interface ITaggable
    {
        List<string> Tags { get; set; }

        bool Is(string tag);
    }
}