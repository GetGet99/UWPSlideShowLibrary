using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using CSharpUI;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace SlidesLib
{
    public class SlideContainer
    {
        public Size SlideSize
        {
            get => new Size(UIControl.Width, UIControl.Height);
            set
            {
                UIControl.Width = value.Width;
                UIControl.Height = value.Height;
            }
        }
        public Overlay UIControl { get; } = new Overlay();
        public List<Slide> Slides { get; } = new List<Slide>();
        public Slide CurrentSlide => Slides[CurrentSlideIndex];
        int CurrentSlideIndex = 0;
        public SlideContainer(Size SlideSize)
        {
            this.SlideSize = SlideSize;
            UIControl.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, SlideSize.Width, SlideSize.Height)
            };
        }
        public void FinalizeSlides()
        {
            foreach (var Slide in Slides)
                UIControl.AddChild(Slide.UIControl);
            UpdateSlidePosition(Instant: true);
        }
        public void ReloadSlides()
        {
            UIControl.Children.Clear();
            foreach (var Slide in Slides)
                Slide.ReInitialize();
            FinalizeSlides();
        }
        public void UpdateSlidePosition(bool Instant = false)
        {
            var CurrentSlide = this.CurrentSlide;
            var TranslateDuration = CurrentSlide.TransitionDuration;
            var SlideSize = this.SlideSize;
            double X = 0, Y = 0;
            bool First = true;
            Point[] Points = new Point[Slides.Count];
            foreach (var (i, Slide) in Slides.Enumerate())
            {
                var pos = Slide.SelfPosition(SlideSize);
                if (First) First = false;
                else
                {
                    X += pos.X;
                    Y += pos.Y;
                }
                Points[i] = new Point(X, Y);
            }
            var CurrentPt = Points[CurrentSlideIndex];
            foreach (var (i, Slide) in Slides.Enumerate())
            {
                var pt = Points[i];
                pt.X -= CurrentPt.X;
                pt.Y -= CurrentPt.Y;
                if (Instant) Slide.GlobalSlidePosition = pt;
                    Slide.Translate(pt, TranslateDuration);
            }
        }
        public bool NextAnimation()
        {
            if (CurrentSlide.NextAnimation())
            {
                return true;
            }
            else
            {
                if (CurrentSlideIndex + 1 < Slides.Count)
                {
                    CurrentSlideIndex++;
                    UpdateSlidePosition();
                    return true;
                }
                return false;
            }
        }
        public bool PreviousAnimation()
        {
            if (CurrentSlide.PreviousAnimation())
            {
                return true;
            }
            else
            {
                if (CurrentSlideIndex > 0)
                {
                    CurrentSlideIndex--;
                    UpdateSlidePosition();
                    return true;
                }
                return false;
            }
        }
    }
    public abstract class Slide
    {
        public virtual double TransitionDuration => 1000;
        private readonly DoubleAnimation TranslationXAnimation = new DoubleAnimation();
        private readonly DoubleAnimation TranslationYAnimation = new DoubleAnimation();
        class Animation : Timeline
        {
            public Animation()
            {
                
            }
        }
        Storyboard Storyboard { get; } = new Storyboard();
        public Slide()
        {
            Storyboard.SetTargetProperty(TranslationXAnimation, "TranslateX");
            TranslationXAnimation.EasingFunction = new CircleEase
            {
                EasingMode = EasingMode.EaseInOut
            };
            Storyboard.SetTargetProperty(TranslationYAnimation, "TranslateY");
            TranslationYAnimation.EasingFunction = new CircleEase
            {
                EasingMode = EasingMode.EaseInOut
            };
            Storyboard.Children.Add(TranslationXAnimation);
            Storyboard.Children.Add(TranslationYAnimation);
            ReInitialize();
        }
        public virtual Point SelfPosition(Size CanvasSize)
            => new Point(x: CanvasSize.Width, y: 0);
        public Point GlobalSlidePosition
        {
            get => UIControl.GlobalPosition;
            set => UIControl.GlobalPosition = value;
        }
        public void Translate(Point pt, double ms)
        {
            var pos = GlobalSlidePosition;
            var compositeTransform = UIControl.CompositeTransform;
            Storyboard.Stop();
            Storyboard.SetTarget(TranslationXAnimation, compositeTransform);
            Storyboard.SetTarget(TranslationYAnimation, compositeTransform);
            TranslationXAnimation.From = pos.X;
            TranslationYAnimation.From = pos.Y;
            TranslationXAnimation.To = pt.X;
            TranslationYAnimation.To = pt.Y;
            var duration = new Duration(TimeSpan.FromMilliseconds(ms));
            TranslationXAnimation.Duration = duration;
            TranslationYAnimation.Duration = duration;
            Storyboard.Duration = duration;
            Storyboard.Begin();
        }
        protected List<Animations.Animation> Animations { get; } = new List<Animations.Animation>();
        int CurrentAnimationIndex = -1;
        public bool NextAnimation()
        {
            if (CurrentAnimationIndex + 1 < Animations.Count)
            {
                Animations[++CurrentAnimationIndex].Forward();
                return true;
            }
            return false;
        }
        public bool PreviousAnimation()
        {
            if (CurrentAnimationIndex >= 0)
            {
                Animations[CurrentAnimationIndex--].Backward();
                return true;
            }
            return false;
        }
        public void ReInitialize()
        {
            Animations.Clear();
            CurrentAnimationIndex = -1;
            UIControl = new SlideElement();
            UIControl.Children.Add(OnCreate());
        }
        public SlideElement UIControl { get; private set; } = new SlideElement();
        protected abstract UIElement OnCreate();
    }
    public class SlideElement : Grid
    {
        public CompositeTransform CompositeTransform { get; } = new CompositeTransform();
        public double GlobalX
        {
            get => CompositeTransform.TranslateX;
            set => CompositeTransform.TranslateX = value;
        }
        public double GlobalY
        {
            get => CompositeTransform.TranslateY;
            set => CompositeTransform.TranslateY = value;
        }
        public Point GlobalPosition
        {
            get => new Point(GlobalX, GlobalY);
            set
            {
                GlobalX = value.X;
                GlobalY = value.Y;
            }
        }
        public SlideElement()
        {
            RenderTransform = CompositeTransform;
        }
    }
}