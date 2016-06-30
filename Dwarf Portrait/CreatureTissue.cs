using RemoteFortressReader;
using System.Windows.Media;

namespace Dwarf_Portrait
{
    public class CreatureTissue
    {
        public CreatureTissue(TissueRaw original)
        {
            Original = original;
            if (DFConnection.MaterialRaws != null)
                Material = DFConnection.MaterialRaws[original.material];
        }
        public TissueRaw Original { get; set; }
        public MaterialDefinition Material { get; set; }
        public Color Color
        {
            get
            {
                if (Material == null)
                    return Color.FromRgb(255, 0, 255);
                else
                    return Color.FromRgb((byte)Material.state_color.red, (byte)Material.state_color.green, (byte)Material.state_color.blue);
            }
        }
    }
}
