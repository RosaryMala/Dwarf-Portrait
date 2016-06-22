using System.Windows;
using System.Windows.Controls;

namespace Dwarf_Portrait
{
    class ColorPatternTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            ColorPattern pattern = item as ColorPattern;
            switch (pattern.Pattern)
            {
                case RemoteFortressReader.PatternType.MONOTONE:
                    break;
                case RemoteFortressReader.PatternType.STRIPES:
                    if(pattern.Colors.Count == 7)
                        return element.FindResource("Stripes7PatternTemplate") as DataTemplate;
                    else
                    return element.FindResource("StripesPatternTemplate") as DataTemplate;
                case RemoteFortressReader.PatternType.IRIS_EYE:
                    return element.FindResource("IrisEyePatternTemplate") as DataTemplate;
                case RemoteFortressReader.PatternType.SPOTS:
                    return element.FindResource("SpotsPatternTemplate") as DataTemplate;
                case RemoteFortressReader.PatternType.PUPIL_EYE:
                    return element.FindResource("PupilEyePatternTemplate") as DataTemplate;
                case RemoteFortressReader.PatternType.MOTTLED:
                    return element.FindResource("MottledPatternTemplate") as DataTemplate;
                default:
                    break;
            }
            return element.FindResource("MonotonePatternTemplate") as DataTemplate; 
        }
    }
}
