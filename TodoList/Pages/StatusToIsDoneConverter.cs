using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoList.Converters
{
    public class StatusToIsDoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status == "Completed";
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDone)
            {
                return isDone ? "Completed" : "Pending";
            }
            return "Pending";
        }
    }
}
