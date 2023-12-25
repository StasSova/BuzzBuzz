using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Services
{
    public class ColorConverterService : IValueConverter
    {
        public ColorConverterService() { }
        private static ColorConverterService instance;
        public static ColorConverterService Instance
        {
            get
            {
                if (instance == null)
                    instance = new ColorConverterService();
                return instance;
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string colorString)
                {
                    if (Color.TryParse(colorString, out var color))
                        return color;
                    else return Colors.WhiteSmoke; // or another default color if the conversion fails
                }
            }
            catch { }

            return Colors.White; // or another default color if the conversion fails
        }

        public Color ConvertStringToColor(string colorString)
        {
            if (Color.TryParse(colorString, out var color))
                return color;
            else
                return Colors.White; // or another default color if the conversion fails
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
