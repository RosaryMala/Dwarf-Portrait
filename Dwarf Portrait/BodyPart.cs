using RemoteFortressReader;
using System.Collections.Generic;

namespace Dwarf_Portrait
{
    public class BodyPart
    {
        public BodyPart()
        {
            Layers = new List<BodyPartLayer>();
            AppearanceMods = new List<BodyPartMod>();
        }
        public BodyPartRaw OriginalPart { get; set; }
        public List<BodyPart> Children { get; set; }
        public int OriginalIndex { get; set; }
        public BodyPartLayer SelectedLayer { get; set; }
        public List<BodyPartLayer> Layers { get; set; }
        public List<BodyPartMod> AppearanceMods { get; set; }
        public ColorMod ColorMod { get; set; }
    }
}
