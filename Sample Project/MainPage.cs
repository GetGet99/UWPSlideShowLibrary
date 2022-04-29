using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using CSharpUI;
using SlidesLib;
using System;
using Windows.UI.Xaml.Media;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace SampleProject
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            var SlideContainer = new SlideContainer(new Size(1920, 1080));
            BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
            var ScrollViewer = new ScrollViewer
            {
                Content = SlideContainer.UIControl,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            };
            Content = ScrollViewer;

            // Optional: Add some white color to distinguish between background and actual slides
            SlideContainer.UIControl.Background = new SolidColorBrush(Color.FromArgb(255 / 10, 255, 255, 255));
            
            AddSlides(SlideContainer);
            SlideContainer.FinalizeSlides();

            // Handle Window Resize Logic (to fit the slide content into Window)
            SizeChanged += delegate
            {
                var Width = ActualWidth;
                var Height = ActualHeight;
                var SlideSize = SlideContainer.SlideSize;
                double WidthScale = Width / SlideSize.Width;
                double HeightScale = Height / SlideSize.Height;
                var zoomFactor = (float)Math.Min(WidthScale, HeightScale);
                double XLeft = Width - (SlideSize.Width * zoomFactor);
                double YLeft = Height - (SlideSize.Height * zoomFactor);
                ScrollViewer.ChangeView(horizontalOffset: XLeft / 2, verticalOffset: YLeft / 2, zoomFactor: zoomFactor);
            };
            CoreWindow.GetForCurrentThread().KeyDown += (_, e) =>
            {
                e.Handled = true;
                switch (e.VirtualKey)
                {
                    case VirtualKey.R:
                        // Use this with C# hot reload
                        SlideContainer.ReloadSlides();
                        break;
                    case VirtualKey.Right:
                    case VirtualKey.PageDown:
                        SlideContainer.NextAnimation();
                        break;
                    case VirtualKey.Left:
                    case VirtualKey.PageUp:
                        SlideContainer.PreviousAnimation();
                        break;
                    case VirtualKey.F11:
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                        break;
                    case VirtualKey.Escape:
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                        break;
                }
                e.Handled = false;
            };
        }
        void AddSlides(SlideContainer SlideContainer)
        {
            SlideContainer.Slides.Add(new Slides.Title());
            SlideContainer.Slides.Add(new Slides.Statistics());
        }
    }
}
