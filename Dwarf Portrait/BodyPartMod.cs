using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
