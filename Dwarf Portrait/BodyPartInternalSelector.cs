using System.Windows;
using System.Windows.Controls;

namespace Dwarf_Portrait
{
    class BodyPartInternalSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            BodyPart part = item as BodyPart;
            if(item == null)
                return base.SelectTemplate(item, container);

            if(part.IsInternal)
                return element.FindResource("BodyPartInternalTemplate") as HierarchicalDataTemplate;
            else
                return element.FindResource("BodyPartExternalTemplate") as HierarchicalDataTemplate;
        }
    }
}
