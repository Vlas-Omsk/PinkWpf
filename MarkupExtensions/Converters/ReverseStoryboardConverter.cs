using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(Storyboard), typeof(Storyboard))]
    public class ReverseStoryboardConverter : MarkupExtension, IValueConverter
    {
        private static BoolToVisibilityConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var storyboard = ((Storyboard)value).Clone();
            storyboard.AutoReverse = true;
            storyboard.SkipToFill();
            return storyboard;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new BoolToVisibilityConverter());
        }
    }
}
