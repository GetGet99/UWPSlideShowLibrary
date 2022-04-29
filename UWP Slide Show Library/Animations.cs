using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SlidesLib.Animations
{
    public abstract class Animation
    {
        public abstract void Forward();
        public abstract void Backward();
    }
    public class MoveAnimation : Animation
    {
        private readonly NumericAnimation TranslationXAnimation;
        private readonly NumericAnimation TranslationYAnimation;

        public MoveAnimation(UIElement UIControl, double Duration, Point From, Point To)
        {
            if (!(UIControl.RenderTransform is CompositeTransform))
                UIControl.RenderTransform = new CompositeTransform();
            TranslationXAnimation = new NumericAnimation(UIControl, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)", Duration, From.X, To.X);
            TranslationYAnimation = new NumericAnimation(UIControl, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)", Duration, From.Y, To.Y);
        }
        public override void Forward()
        {
            TranslationXAnimation.Forward();
            TranslationYAnimation.Forward();
        }
        public override void Backward()
        {
            TranslationXAnimation.Backward();
            TranslationYAnimation.Backward();
        }
        public static MoveAnimation Create(UIElement UIControl, double Duration, Point From, Point To)
            => new MoveAnimation(UIControl, Duration, From, To);
    }
    public class NumericAnimation : Animation
    {
        private readonly DoubleAnimation DoubleAnimationForward = new DoubleAnimation
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        private readonly DoubleAnimation DoubleAnimationBackward = new DoubleAnimation
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };


        private Storyboard StoryboardForward { get; } = new Storyboard();
        private Storyboard StoryboardBackward { get; } = new Storyboard();
        public NumericAnimation(DependencyObject Object, string Property, double Duration, double From, double To, bool DoBackward = true)
        {
            StoryboardForward.Children.Add(DoubleAnimationForward);
            Storyboard.SetTarget(DoubleAnimationForward, Object);
            Storyboard.SetTargetProperty(DoubleAnimationForward, Property);
            DoubleAnimationForward.Duration = new Duration(TimeSpan.FromMilliseconds(Duration));
            DoubleAnimationForward.From = From;
            DoubleAnimationForward.To = To;

            StoryboardBackward.Children.Add(DoubleAnimationBackward);
            Storyboard.SetTarget(DoubleAnimationBackward, Object);
            Storyboard.SetTargetProperty(DoubleAnimationBackward, Property);
            DoubleAnimationBackward.Duration = new Duration(TimeSpan.FromMilliseconds(Duration));
            DoubleAnimationBackward.From = To;
            DoubleAnimationBackward.To = From;
            if (DoBackward) Backward();
        }
        public override void Forward()
        {
            StoryboardForward.Begin();
        }
        public override void Backward()
        {
            StoryboardBackward.Begin();
        }
        public static NumericAnimation Create(DependencyObject Object, string Property, double Duration, double From, double To, bool DoBackward = false)
            => new NumericAnimation(Object, Property, Duration, From, To, DoBackward: DoBackward);
    }
    public class FadeInAnimation : Animation
    {
        private readonly NumericAnimation NumericAnimation;
        public FadeInAnimation(DependencyObject Object, double Duration, bool DoBackward = true)
        {
            NumericAnimation = new NumericAnimation(Object, "Opacity", Duration, 0, 1, DoBackward: DoBackward);
        }
        public override void Forward() => NumericAnimation.Forward();
        public override void Backward() => NumericAnimation.Backward();
        public static FadeInAnimation Create(DependencyObject Object, double Duration)
            => new FadeInAnimation(Object, Duration);
    }
    public class FadeOutAnimation : Animation
    {
        private readonly NumericAnimation NumericAnimation;
        public FadeOutAnimation(DependencyObject Object, double Duration, bool DoBackward = true)
        {
            NumericAnimation = new NumericAnimation(Object, "Opacity", Duration, 1, 0, DoBackward: DoBackward);
        }
        public override void Forward() => NumericAnimation.Forward();
        public override void Backward() => NumericAnimation.Backward();
        public static FadeOutAnimation Create(DependencyObject Object, double Duration, bool DoBackward = true)
            => new FadeOutAnimation(Object, Duration, DoBackward: DoBackward);
    }
    public class LambdaAnimation : Animation
    {
        readonly Action ForwardAction;
        readonly Action BackwardAction;
        public LambdaAnimation(Action Forward, Action Backward, bool InvokeBackwardNow = false)
        {
            ForwardAction = Forward;
            BackwardAction = Backward;
            if (InvokeBackwardNow)
                this.Backward();
        }
        public static LambdaAnimation Create(Action Forward, Action Backward, bool InvokeBackwardNow = false)
            => new LambdaAnimation(Forward, Backward, InvokeBackwardNow);

        public override void Backward()
        {
            BackwardAction?.Invoke();
        }

        public override void Forward()
        {
            ForwardAction?.Invoke();
        }
    }
}
