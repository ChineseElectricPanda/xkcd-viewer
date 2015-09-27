using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace xkcd_viewer
{
    public sealed partial class ComicPanel : UserControl
    {
        private Page context;
        public static async Task<int> getNewestComicNumber()
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(new Uri("http://www.xkcd.com/info.0.json"));
            XkcdJsonObject jsonObject = JsonConvert.DeserializeObject<XkcdJsonObject>(response);
            return jsonObject.num;
        }

        private List<OnComicLoadListener> onComicLoadListeners = new List<OnComicLoadListener>();
        public XkcdJsonObject comic;
        public ComicPanel(Page context)
        {
            this.context = context;
            this.InitializeComponent();
        }

        public void addOnComicLoadListener(OnComicLoadListener l)
        {
            this.onComicLoadListeners.Add(l);
        }

        public void OnNavigatedTo()
        {
            scrollViewer.ChangeView(0, 0, 1);
        }

        private async void loadComic(int number)
        {
            comic = await getComicJson(number);
            if (comic != null)
            {
                image.Source = new BitmapImage(new Uri(comic.img));
                scrollViewer.Visibility = Visibility.Visible;
                loadFailedPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                loadFailed(number);
            }
            foreach (OnComicLoadListener l in onComicLoadListeners)
            {
                l.OnComicLoaded(comic);
            }
        }

        private void loadFailed(int num)
        {
            scrollViewer.Visibility = Visibility.Collapsed;
            loadFailedPanel.Visibility = Visibility.Visible;
            loadFailedText.Text = "Could not load comic " + num;
            comic = new XkcdJsonObject
            {
                num = num,
                title = num+" not found",
                alt = "",
                img = ""
            };
        }

        private async Task<XkcdJsonObject> getComicJson(int number)
        {
            try {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync(new Uri("http://www.xkcd.com/" + number + "/info.0.json"));
                XkcdJsonObject jsonObject = JsonConvert.DeserializeObject<XkcdJsonObject>(response);
                return jsonObject;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var transform = (CompositeTransform)image.RenderTransform;

            transform.ScaleX = e.Cumulative.Scale;
            transform.ScaleY = e.Cumulative.Scale;

            transform.TranslateX = e.Cumulative.Translation.X;
            transform.TranslateY = e.Cumulative.Translation.Y;

            e.Handled = true;
        }

        public int Number
        {
            set
            {
                if (comic == null || comic.num != value)
                {
                    if (value < 1)
                        value = 1;
                    if (value > MainPage.newestComic)
                        value = MainPage.newestComic;
                    loadComic(value);
                }
            }
        }

        private void image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            context.Frame.Navigate(typeof(ComicFullScreenView), comic.img);
            e.Handled = true;
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            loadFailedPanel.Visibility = Visibility.Collapsed;
            loadComic(comic.num);

        }
    }
}
