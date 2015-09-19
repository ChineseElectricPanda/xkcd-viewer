using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace xkcd_viewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, OnComicLoadListener
    {
        const int pivotBuffer = 10;
        public static int newestComic = 1;
        PivotItem[] pivotItems = new PivotItem[pivotBuffer];
        ComicPanel[] comicPanels = new ComicPanel[pivotBuffer];
        int currentPanel = 0;
        int currentComic = 1;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            Task<int> currentComicTask = ComicPanel.getNewestComicNumber();
            currentComicTask.Wait();
            newestComic = currentComicTask.Result;
            currentComic = 500;

            for (int i = 0; i < pivotBuffer; i++)
            {
                pivotItems[i] = new PivotItem();
                pivotItems[i].Margin = new Thickness(0, 0, 0, 0);
                pivotItems[i].Content = new Grid();
                pivot.Items.Add(pivotItems[i]);
                comicPanels[i] = new ComicPanel(this);
                comicPanels[i].addOnComicLoadListener(this);
                ((Grid)pivotItems[i].Content).Children.Add(comicPanels[i]);
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPanel == 0 && pivot.SelectedIndex == pivotBuffer - 1)
                currentComic--;
            else if (currentPanel == pivotBuffer - 1 && pivot.SelectedIndex == 0)
                currentComic++;
            else if (currentPanel < pivot.SelectedIndex)
                currentComic++;
            else if (currentPanel > pivot.SelectedIndex)
                currentComic--;
            currentPanel = pivot.SelectedIndex;
            comicPanels[currentPanel].OnNavigatedTo();
            updateComicPanels();
        }

        private void updateComicPanels()
        {
            int currentPosition = pivot.SelectedIndex;
            int previousPosition = (currentPosition - 1);
            if (previousPosition < 0)
                previousPosition = pivotBuffer - 1;

            for (int i = 0; i < pivotBuffer - 2; i++)
            {
                comicPanels[(currentPosition + i) % pivotBuffer].Number = currentComic + i;
            }
            comicPanels[previousPosition].Number = currentComic - 1;
        }

        public void OnComicLoaded(XkcdJsonObject comicJson)
        {
            for (int i = 0; i < pivotBuffer; i++)
            {
                if (comicPanels[i].comic == comicJson)
                {
                    pivotItems[i].Header = comicJson.title;
                }
            }
        }

        private async void jumpToAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new JumpToComicDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (dialog.SelectedComic != 0)
                {
                    currentComic = dialog.SelectedComic;
                    updateComicPanels();
                }

            }
        }

        private void explainAppBarIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExplanationPage), currentComic.ToString());
        }

        private void altTextAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var comic = comicPanels[currentPanel].comic;
            var dialog = new MessageDialog(comic.alt,comic.num+" - "+comic.title);
            dialog.ShowAsync();
        }
    }
}
