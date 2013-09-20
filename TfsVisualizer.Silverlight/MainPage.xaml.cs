using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
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
            pivotViewer.Loaded += pivotViewer_Loaded;
            pivotViewer.FilterChanged += pivotViewer_FilterChanged;
        }

        public string Filter
        {
            get
            {
                try
                {
                    return IsolatedStorageSettings.ApplicationSettings["Filter"].ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
            set { IsolatedStorageSettings.ApplicationSettings["Filter"] = value; }
        }

        void pivotViewer_FilterChanged(object sender, EventArgs e)
        {
            Filter = pivotViewer.Filter;
        }

        private void pivotViewer_Loaded(object sender, RoutedEventArgs e)
        {
            pivotViewer.ItemsSource = _tfsWorkItems;
        }

        private ObservableCollection<TfsWorkItem> _tfsWorkItems = new ObservableCollection<TfsWorkItem>(); 

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
                _tfsWorkItems.Clear();
                //deserialize JSON array into objects and feed it to the pivot viewer
                var serializer = new DataContractJsonSerializer(typeof (List<TfsWorkItem>));
                var incomingWorkItems = (List<TfsWorkItem>) serializer.ReadObject(e.Result);
                incomingWorkItems.ForEach(x=>_tfsWorkItems.Add(x));
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
            try
            {
                pivotViewer.Filter = Filter;
            }
            catch
            {
                pivotViewer.Filter = string.Empty;
            }
        }

        private void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
