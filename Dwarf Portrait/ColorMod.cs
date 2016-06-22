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
            Patterns = new List<List<string>>();
            CurrentValue = -1;
        }
        public ColorModifierRaw Original { get; set; }
        public List<List<string>> Patterns { get; set; }
        public int CurrentValue { get; set; }
        public List<string> CurrentPattern {
            get
            {
                if(CurrentValue == -1)
                {
                    List<string> output = new List<string>();
                    foreach (var patt in Patterns)
                    {
                        foreach(var col in patt)
                        {
                            output.Add(col);
                        }
                    }
                    return output;
                }
                else
                {
                    return Patterns[CurrentValue];
                }
            }
        }
    }
}
