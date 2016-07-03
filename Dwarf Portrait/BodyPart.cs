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
            Children = new List<BodyPart>();
        }
        public BodyPartRaw OriginalPart { get; set; }
        public List<BodyPart> Children { get; set; }
        public int OriginalIndex { get; set; }
        public BodyPartLayer SelectedLayer { get; set; }
        public List<BodyPartLayer> Layers { get; set; }
        public List<BodyPartMod> AppearanceMods { get; set; }
        public ColorMod ColorMod { get; set; }
        public bool IsInternal
        {
            get
            {
                if (OriginalPart == null)
                    return false;
                else
                    return
                        OriginalPart.flags[(int)UnitFlags.body_part_raw_flags.INTERNAL];
            }
        }
        public bool IsHead
        {
            get
            {
                if (OriginalPart == null)
                    return false;
                else
                    return
                        OriginalPart.flags[(int)UnitFlags.body_part_raw_flags.HEAD];
            }
        }
        public bool IsLeft
        {
            get
            {
                if (OriginalPart == null)
                    return false;
                else
                    return
                        OriginalPart.flags[(int)UnitFlags.body_part_raw_flags.LEFT];
            }
        }
        public bool IsRight
        {
            get
            {
                if (OriginalPart == null)
                    return false;
                else
                    return
                        OriginalPart.flags[(int)UnitFlags.body_part_raw_flags.RIGHT];
            }
        }
        public bool IsLowerBody
        {
            get
            {
                if (OriginalPart == null)
                    return false;
                else
                    return
                        OriginalPart.flags[(int)UnitFlags.body_part_raw_flags.LOWERBODY];
            }
        }

        public bool IsEmbedded
        {
            get
            {
                if (OriginalPart == null)
                    return false;
                else
                    return
                        OriginalPart.flags[(int)UnitFlags.body_part_raw_flags.EMBEDDED];
            }
        }

        public bool LeadsToHead
        {
            get
            {
                if (IsHead)
                    return true;
                if (Children == null)
                    return false;
                foreach (BodyPart child in Children)
                {
                    if (child.LeadsToHead)
                        return true;
                }
                return false;
            }
        }
    }
}
