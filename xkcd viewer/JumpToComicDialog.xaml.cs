using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace xkcd_viewer
{
    public sealed partial class JumpToComicDialog : ContentDialog
    {
        private int selection = 0;
        private bool canClose = true;
        public JumpToComicDialog()
        {
            this.InitializeComponent();
        }

        public int SelectedComic
        {
            get
            {
                return selection;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                selection = int.Parse(number.Text);
                if (selection > 0 && selection <= MainPage.newestComic)
                    canClose = true;
                else
                    throw new OverflowException();
            }
            catch (ArgumentNullException)
            {
                errorText.Text = "Please enter a number between 1 and " + MainPage.newestComic;
                canClose = false;
            }
            catch (FormatException)
            {
                errorText.Text = "Please enter a number between 1 and " + MainPage.newestComic;
                canClose = false;
            }
            catch (OverflowException)
            {
                errorText.Text = "Please enter a number between 1 and " + MainPage.newestComic;
                canClose = false;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            canClose = true;
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (!canClose)
                args.Cancel = true;
        }
    }
}
