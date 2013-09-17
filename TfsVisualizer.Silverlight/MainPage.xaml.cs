using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Pivot;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Json;

namespace TfsVisualizer.Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void Load()
        {
            var url = App.HttpHandlerUri;

            //add parameters if specified in the url
            var parameterBuilder = new StringBuilder("");
            foreach (var key in HtmlPage.Document.QueryString.Keys)
            {
                parameterBuilder.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(HtmlPage.Document.QueryString[key]));
            }
            url += parameterBuilder.ToString();
            txtUrl.Text = url;
            var wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            wc.OpenReadAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        public void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                //deserialize JSON array into objects and feed it to the pivot viewer
                var serializer = new DataContractJsonSerializer(typeof (List<TfsWorkItem>));
                var tfsWorkItems = (List<TfsWorkItem>) serializer.ReadObject(e.Result);
                pivotViewer.ItemsSource = new ObservableCollection<TfsWorkItem>(tfsWorkItems);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }
    }
}
