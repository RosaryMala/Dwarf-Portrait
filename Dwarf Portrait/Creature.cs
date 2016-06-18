using RemoteFortressReader;
using System.Globalization;
using UnitFlags;

namespace Dwarf_Portrait
{
    class Creature
    {
        private UnitDefinition _unitDefinition;
        public UnitDefinition unitDefinition {
            get
            {
                return _unitDefinition;
            }
            set
            {
                _unitDefinition = value;
                flags1 = (UnitFlags1)value.flags1;
                flags2 = (UnitFlags2)value.flags2;
                flags3 = (UnitFlags3)value.flags3;

                name = value.name;
                if (DFConnection.creatureRawList != null)
                {
                    if (unitDefinition.race.mat_type >= DFConnection.creatureRawList.creature_raws.Count)
                        race = "INVALID_RACE";
                    else
                    {
                        race = TI.ToTitleCase(DFConnection.creatureRawList.creature_raws[unitDefinition.race.mat_type].name[0]);
                    }
                }
                professionColor = string.Format("#FF{0:X2}{1:X2}{2:X2}", value.profession_color.red, value.profession_color.green, value.profession_color.blue);
            }
        }
        static TextInfo TI = new CultureInfo("en-US", false).TextInfo;

        string name = "NULL_UNIT";
        string race = "NULL_RACE";
        string professionColor = "#FF000000";

        public string Name
        {
            get
            {
                if (name == "")
                    return "(No Name)";
                return name;
            }
        }
        public string Race { get { return race; } }
        public string ProfessionColor { get { return professionColor; } }

        public UnitFlags1 flags1;
        public UnitFlags2 flags2;
        public UnitFlags3 flags3;

        public override string ToString()
        {
            if (name == "")
                return race;
            else
                return name + ", " + race;
        }
    }
}
