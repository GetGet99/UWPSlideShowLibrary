using CSharpUI;
using SlidesLib;
using SlidesLib.Animations;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using ProgressRing = Microsoft.UI.Xaml.Controls.ProgressRing;

namespace SampleProject.Slides
{
    class Title : Slide
    {
        protected override UIElement OnCreate()
            => new Overlay()
            .AddChild(new CenterBoth
            {
                Content = new TextBlock
                {
                    Text = "Replace this text with Presentation Title!",
                    FontSize = 100
                }
            })
            .AddChild(
                new TextBlock
                {
                    Text = "Created By: Get0457\nUse Left and Right Arrow Key to control the slide\nF11 to full screen, and press R to reload the slides (typically use with C# hot reload)\n",
                    FontSize = 50,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                }
            );
    }
    class Statistics : Slide
    {
        protected override UIElement OnCreate()
            => new Overlay()
            .AddChild(new CenterBoth
            {
                Content =
                    new StackPanel()
                    .AddChild(
                        new Overlay()
                        .AddChild(
                            new ProgressRing
                            {
                                Value = 100,
                                IsIndeterminate = false,
                                Width = 300,
                                Height = 300,
                                Foreground = new SolidColorBrush(Colors.DimGray)
                            }
                        )
                        .AddChild(
                            new ProgressRing
                            {
                                Value = 40,
                                IsIndeterminate = false,
                                Width = 300,
                                Height = 300
                            }
                        )
                    )
                    .AddChild(new TextBlock
                    {
                        Text = "\n40%\n of something does something!",
                        FontSize = 50,
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    })
            }.Edit(x => Animations.Add(FadeOutAnimation.Create(x, 500))))
            .AddChild(
                new TextBlock
                {
                    Text = "\"You can add text and animations. The animations includes fade in,\nfade out, and move element. Experiment with them\"",
                    FontSize = 50,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
                .Edit(x => Animations.Add(FadeInAnimation.Create(x, 500)))
            )
            .AddChild(
                new TextBlock
                {
                    Text = "You can put citation here: (Name, Year)",
                    FontSize = 30,
                    Padding = new Thickness(15),
                    TextAlignment = TextAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                }
            );
    }
}
