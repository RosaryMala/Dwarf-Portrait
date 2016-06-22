using System.Windows;
using System.Windows.Controls;

namespace Dwarf_Portrait
{
    class BodyPartInternalSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}
