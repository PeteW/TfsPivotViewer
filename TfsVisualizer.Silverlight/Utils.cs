#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Pivot;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

#endregion

namespace TfsVisualizer.Silverlight
{
    public class TfsItemToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TfsWorkItem;
            if (item == null)
                return new SolidColorBrush(Colors.White);
            if (item.Type == "Product Backlog Item")
                return Extensions.GetColorFromHex("483d8b");
            if (item.Type == "Task")
                return Extensions.GetColorFromHex("808b3d");
            if (item.Type == "Test Case")
                return Extensions.GetColorFromHex("8b6f3d");
            if (item.Type == "Bug")
                return Extensions.GetColorFromHex("8b3d59");
            if (item.Type == "Feature")
                return Extensions.GetColorFromHex("e070e0");
            if (item.Type == "User Story")
                return Extensions.GetColorFromHex("70a8e0");
            return Extensions.GetColorFromHex("dcefe8");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new PivotViewerHyperlink("Open In TFS", new Uri(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TfsItemToForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TfsWorkItem;
            if (item == null)
                return new SolidColorBrush(Colors.Black);
            if (item.Type == "Product Backlog Item")
                return Extensions.GetColorFromHex("FFFFFF");
            if (item.Type == "Task")
                return Extensions.GetColorFromHex("FFFFFF");
            if (item.Type == "Test Case")
                return Extensions.GetColorFromHex("FFFFFF");
            if (item.Type == "Bug")
                return Extensions.GetColorFromHex("FFFFFF");
            return Extensions.GetColorFromHex("000000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Convert hex to solid color brush
        /// </summary>
        /// <param name="hexaColor"></param>
        /// <returns></returns>
        public static SolidColorBrush GetColorFromHex(string hexaColor)
        {
            hexaColor = hexaColor.Replace("#", string.Empty);
            var r = (byte) (Convert.ToUInt32(hexaColor.Substring(0, 2), 16));
            var g = (byte) (Convert.ToUInt32(hexaColor.Substring(2, 2), 16));
            var b = (byte) (Convert.ToUInt32(hexaColor.Substring(4, 2), 16));
            return new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

        /// <summary>
        /// Get the authority from a URI [scheme]://[host]:[port (if applicable)/
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetRoot(this Uri uri)
        {
            //Return variable declaration
            var result = string.Empty;
            //Formatting the fully qualified website url/name
            result = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.Port == 80? string.Empty:":" + uri.Port);
            if (!result.EndsWith("/"))
                result += "/";
            return result;
        }
    }
}