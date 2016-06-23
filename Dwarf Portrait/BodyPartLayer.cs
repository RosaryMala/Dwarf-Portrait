using RemoteFortressReader;
using System.Collections.Generic;

namespace Dwarf_Portrait
{
    public class BodyPartLayer
    {
        public BodyPartLayer()
        {
            AppearanceMods = new List<BodyPartMod>();
        }
        public BodyPartLayerRaw Original { get; set; }
        public List<BodyPartMod> AppearanceMods { get; set; }
        public ColorMod ColorMod { get; set; }
    }
}
