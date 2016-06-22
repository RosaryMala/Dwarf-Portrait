using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteFortressReader;

namespace Dwarf_Portrait
{
    public class ColorMod
    {
        public ColorMod()
        {
            Patterns = new List<ColorPattern>();
            CurrentValue = -1;
        }
        public ColorModifierRaw Original { get; set; }
        public List<ColorPattern> Patterns { get; set; }
        public int CurrentValue { get; set; }
        public List<ColorPattern> CurrentPatterns {
            get
            {
                if(CurrentValue == -1)
                {
                    return Patterns;
                }
                else
                {
                    return new List<ColorPattern>() { Patterns[CurrentValue] };
                }
            }
        }
    }
}
