using System.Collections.Generic;

namespace data_rogue_core.Entities
{
    public interface ITaggable
    {
        List<string> Tags { get; }

        bool Is(string tag);
    }
}