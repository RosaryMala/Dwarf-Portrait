using RemoteFortressReader;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using UnitFlags;

namespace Dwarf_Portrait
{
    class Creature
    {
        public Creature()
        {
            AppearanceMods = new List<BodyPartMod>();
            BodypartTree = new List<BodyPart>();
            BodyPartList = new List<BodyPart>();
        }
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
                if (value.profession_color != null)
                    professionColor = string.Format("#FF{0:X2}{1:X2}{2:X2}", value.profession_color.red, value.profession_color.green, value.profession_color.blue);
                else
                    professionColor = Colors.LightGray.ToString();

                BodypartTree.Clear();
                BodyPartList.Clear();

                if (CasteRaw != null)
                {
                    for (int i = 0; i < CasteRaw.body_parts.Count; i++)
                    {
                        BodyPart part = new BodyPart();
                        part.OriginalIndex = i;
                        part.OriginalPart = CasteRaw.body_parts[i];

                        foreach (var originalLayer in part.OriginalPart.layers)
                        {
                            BodyPartLayer layer = new BodyPartLayer();
                            layer.Original = originalLayer;
                            part.Layers.Add(layer);
                        }

                        BodyPartList.Add(part);
                    }

                    foreach (BodyPart part in BodyPartList)
                    {
                        if (part.OriginalPart.parent == -1)
                            BodypartTree.Add(part);
                        else
                        {
                            if (BodyPartList[part.OriginalPart.parent].Children == null)
                                BodyPartList[part.OriginalPart.parent].Children = new List<BodyPart>();
                            BodyPartList[part.OriginalPart.parent].Children.Add(part);
                        }
                    }

                    for (int i = 0; i < CasteRaw.modifier_idx.Count; i++)
                    {
                        BodyPartMod mod = new BodyPartMod();
                        mod.Original = CasteRaw.modifiers[CasteRaw.modifier_idx[i]];
                        if(value.appearance != null)
                        {
                            mod.CurrentValue = value.appearance.bp_modifiers[i];
                        }
                        var part = BodyPartList[CasteRaw.part_idx[i]];
                        if (CasteRaw.layer_idx[i] == -1)
                            part.AppearanceMods.Add(mod);
                        else
                            part.Layers[CasteRaw.layer_idx[i]].AppearanceMods.Add(mod);
                    }

                    for (int i = 0; i < CasteRaw.body_appearance_modifiers.Count; i++)
                    {
                        BodyPartMod mod = new BodyPartMod();
                        mod.Original = CasteRaw.body_appearance_modifiers[i];
                        if (value.appearance != null)
                        {
                            mod.CurrentValue = value.appearance.body_modifiers[i];
                        }

                        AppearanceMods.Add(mod);
                    }

                    for(int i = 0; i < CasteRaw.color_modifiers.Count;i++)
                    {
                        ColorMod mod = new ColorMod();
                        mod.Original = CasteRaw.color_modifiers[i];
                        foreach (PatternDescriptor pattern in CasteRaw.color_modifiers[i].patterns)
                        {
                            ColorPattern pat = new ColorPattern();
                            pat.Original = pattern;
                            mod.Patterns.Add(pat);
                        }

                        if(value.appearance != null)
                        {
                            mod.CurrentValue = value.appearance.colors[i];
                        }

                        for(int j = 0; j < mod.Original.body_part_id.Count; j++)
                        {
                            if(mod.Original.tissue_layer_id[j] == -1)
                            {
                                if(BodyPartList[mod.Original.body_part_id[j]].ColorMod == null)
                                    BodyPartList[mod.Original.body_part_id[j]].ColorMod = mod;
                            }
                            else
                            {
                                if(BodyPartList[mod.Original.body_part_id[j]].Layers[mod.Original.tissue_layer_id[j]].ColorMod == null)
                                    BodyPartList[mod.Original.body_part_id[j]].Layers[mod.Original.tissue_layer_id[j]].ColorMod = mod;
                            }
                        }
                    }
                }
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

        public CreatureRaw CreatureRaw
        {
            get
            {
                if (DFConnection.creatureRawList == null)
                    return null;
                if (unitDefinition == null)
                    return null;
                if (DFConnection.creatureRawList.creature_raws.Count <= unitDefinition.race.mat_type)
                    return null;
                return DFConnection.creatureRawList.creature_raws[unitDefinition.race.mat_type];
            }
        }

        public CasteRaw CasteRaw
        {
            get
            {
                if (CreatureRaw == null)
                    return null;
                return CreatureRaw.caste[unitDefinition.race.mat_index];
            }
        }

        public List<BodyPart> BodypartTree { get; set; }
        public List<BodyPart> BodyPartList { get; set; }

        public BodyPart SelectedBodyPart { get; set; }

        public List<BodyPartMod> AppearanceMods { get; set; }

        public int Index { get; set; }
    }
}
