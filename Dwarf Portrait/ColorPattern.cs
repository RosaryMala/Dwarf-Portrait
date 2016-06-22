using RemoteFortressReader;
using System.Collections.Generic;

namespace Dwarf_Portrait
{
    public class ColorPattern
    {
        public ColorPattern()
        {
            Colors = new List<string>();
        }
        PatternDescriptor original;
        public PatternDescriptor Original
        {
            get
            {
                return original;
            }
            set
            {
                original = value;
                Colors.Clear();
                foreach (var color in value.colors)
                {
                    Colors.Add(string.Format("#FF{0:X2}{1:X2}{2:X2}", color.red, color.green, color.blue));
                }
            }
        }
        public List<string> Colors { get; set; }
        public PatternType Pattern
        {
            get
            {
                return original.pattern;
            }
        }
    }
}
