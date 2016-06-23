using RemoteFortressReader;

namespace Dwarf_Portrait
{
    public class BodyPartMod
    {
        public BodyPartMod()
        {
            CurrentValue = 100;
        }
        public BpAppearanceModifier Original { get; set; }
        public int CurrentValue { get; set; }
    }
}
