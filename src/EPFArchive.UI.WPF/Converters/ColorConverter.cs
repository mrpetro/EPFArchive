using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EPFArchive.UI.Converters
{
    public class ColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is System.Drawing.Color))
                throw new InvalidOperationException($"Value must be a {typeof(System.Drawing.Color)}");

            if (targetType != typeof(System.Windows.Media.Brush))
                throw new InvalidOperationException($"The target must be a {typeof(System.Windows.Media.Brush)}");

            var c = (System.Drawing.Color)value;

            return new SolidColorBrush(Color.FromArgb(c.A,c.R, c.G, c.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
