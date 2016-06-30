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
        ColorMod _colorMod;
        ColorMod _fakeColorMod;
        public ColorMod ColorMod {
            get
            {
                if(_colorMod != null)
                    return _colorMod;
                if (Tissue == null)
                    return null;

                if(_fakeColorMod == null && Tissue.Material.state_color != null)
                {

                    ColorPattern patt = new ColorPattern();
                    PatternDescriptor pattOrig = new PatternDescriptor();
                    pattOrig.colors.Add(Tissue.Material.state_color);
                    pattOrig.pattern = PatternType.MONOTONE;
                    pattOrig.id = Tissue.Original.id;

                    patt.Original = pattOrig;

                    _fakeColorMod = new ColorMod();
                    _fakeColorMod.Patterns.Add(patt);
                }

                return _fakeColorMod;

            }
            set
            {
                _colorMod = value;
            }
        }
        public CreatureTissue Tissue { get; set; }
    }
}
