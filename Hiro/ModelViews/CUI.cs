using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hiro.ModelViews
{
    public static class CUI
    {
        public static readonly DependencyProperty AnimatedProperty = DependencyProperty.RegisterAttached(
            "AnimatedProperty",
            typeof(int),
            typeof(CUI),
            new PropertyMetadata(default(int)));

        public static void SetAnimatedProperty(UIElement element, int value)
        {
            element.SetValue(AnimatedProperty, value);
        }

        public static int GetAnimatedProperty(UIElement element)
        {
            return (int)element.GetValue(AnimatedProperty);
        }
    }
}
