using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Pool
{
    public sealed class PumpTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PumpIn { get; set; }
        public DataTemplate PumpOut { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if ((item as Plumpung).Forse >= 0) return PumpIn;
            else return PumpOut;
        }
    }
}
