using RemoteFortressReader;
using System.Collections.Generic;

namespace Dwarf_Portrait
{
    public class BodyPart
    {
        public BodyPartRaw OriginalPart { get; set; }
        public List<BodyPart> Children { get; set; }
        public int OriginalIndex { get; set; }
    }
}
