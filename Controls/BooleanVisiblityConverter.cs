using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Timer.Controls
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = this.GetConverterParameter(parameter);
            bool selected = (bool)value;

            return param == selected ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Not Implemented");
        }

        private bool GetConverterParameter(object parameter)
        {
            bool result = false;

            try
            {
                if (parameter != null)
                    result = System.Convert.ToBoolean(parameter);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return result;
        }
    }
}
