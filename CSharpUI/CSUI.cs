using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CSharpUI
{
    public class CenterBoth : UserControl
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            Content.Measure(finalSize);
            Content.Arrange(new Rect(
                (finalSize.Width - Content.DesiredSize.Width) / 2.0,
                (finalSize.Height - Content.DesiredSize.Height) / 2.0,
                Content.DesiredSize.Width,
                Content.DesiredSize.Height)
            );
            return finalSize;
        }

        public CenterBoth SetChild(UIElement Element)
        {
            Content = Element;
            return this;
        }
    }
    public class CenterHorizontally : UserControl
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Content.Measure(availableSize);
            return new Size(availableSize.Width, Math.Min(availableSize.Height, Content.DesiredSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Content.Measure(finalSize);
            Content.Arrange(new Rect((Content.DesiredSize.Width - finalSize.Width) / 2.0, 0.0, Content.DesiredSize.Width, Content.DesiredSize.Height));
            return finalSize;
        }

        public CenterHorizontally SetChild(UIElement Element)
        {
            Content = Element;
            return this;
        }
    }
    public class CenterVertically : UserControl
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Content.Measure(availableSize);
            return new Size(Math.Min(availableSize.Width, Content.DesiredSize.Width), availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Content.Measure(finalSize);
            Content.Arrange(new Rect(0.0, (Content.DesiredSize.Height - finalSize.Height) / 2.0, Content.DesiredSize.Width, Content.DesiredSize.Height));
            return finalSize;
        }

        public CenterVertically SetChild(UIElement Element)
        {
            Content = Element;
            return this;
        }
    }
    public class ColumnGrid : Panel
    {
        public static void SetColumnDefinition(UIElement element, ColumnDefinition value)
        {
            element.SetValue(ColumnGrid.ColumnDefinitionProperty, value);
        }

        public static ColumnDefinition GetColumnDefinition(UIElement element)
        {
            return (ColumnDefinition)element.GetValue(ColumnGrid.ColumnDefinitionProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement Element2 in Children)
            {
                Element2.Measure(availableSize);
            }
            IEnumerable<ValueTuple<UIElement, ColumnDefinition>> definition = Children.Select(Element => (Element, GetColumnDefinition(Element) ?? new ColumnDefinition()));
            bool flag = definition.Any(x => x.Item2.Width.GridUnitType == GridUnitType.Star);
            double Width;
            if (flag)
            {
                Width = availableSize.Width;
            }
            else
            {
                Width = definition.Sum(x =>
                {
                    GridUnitType gridUnitType = x.Item2.Width.GridUnitType;
                    GridUnitType gridUnitType2 = gridUnitType;
                    double result;
                    if (gridUnitType2 != GridUnitType.Auto)
                    {
                        if (gridUnitType2 != GridUnitType.Pixel)
                        {
                            result = 0.0;
                        }
                        else
                        {
                            result = x.Item2.Width.Value;
                        }
                    }
                    else
                    {
                        result = x.Item1.DesiredSize.Width;
                    }
                    return result;
                });
            }
            double val;
            if (Children.Count != 0)
            {
                val = Children.Max((UIElement x) => x.DesiredSize.Height);
            }
            else
            {
                val = 0.0;
            }
            double Height = Math.Min(val, availableSize.Height);
            Width = ((Width == 0.0) ? (Width + 1.0) : Width);
            Height = ((Height == 0.0) ? (Height + 1.0) : Height);
            return new Size(Width, Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IEnumerable<(UIElement Element, ColumnDefinition ColumnDefinition)> definition = Children.Select(Element => (Element, GetColumnDefinition(Element) ?? new ColumnDefinition()));
            foreach (UIElement Element3 in Children)
            {
                Element3.Measure(finalSize);
            }
            double TotalNonStar = definition.Sum(x =>
            {
                GridUnitType gridUnitType = x.ColumnDefinition.Width.GridUnitType;
                GridUnitType gridUnitType2 = gridUnitType;
                double result;
                if (gridUnitType2 != GridUnitType.Auto)
                {
                    if (gridUnitType2 != GridUnitType.Pixel)
                    {
                        result = 0.0;
                    }
                    else
                    {
                        result = x.ColumnDefinition.Width.Value;
                    }
                }
                else
                {
                    result = x.Element.DesiredSize.Width;
                }
                return result;
            });
            double TotalStar = definition.Sum(x => (x.ColumnDefinition.Width.GridUnitType == GridUnitType.Star) ? x.ColumnDefinition.Width.Value : 0.0);
            double Star = (TotalStar == 0.0) ? 0.0 : ((finalSize.Width - TotalNonStar) / TotalStar);
            bool flag = Star < 0.0;
            if (flag)
            {
                Star = 0.0;
            }
            double ProcessValue((UIElement Element, ColumnDefinition ColumnDefinition) x)
            {
                double result;
                switch (x.ColumnDefinition.Width.GridUnitType)
                {
                    case GridUnitType.Auto:
                        result = x.Element.DesiredSize.Width;
                        break;
                    case GridUnitType.Pixel:
                        result = x.ColumnDefinition.Width.Value;
                        break;
                    case GridUnitType.Star:
                        result = Star * x.ColumnDefinition.Width.Value;
                        break;
                    default:
                        result = 0.0;
                        break;
                }
                return result;
            }
            var ColumnWidth = definition.Select(x => (x.Element, ProcessValue(x)));
            double X = 0.0;
            foreach (ValueTuple<UIElement, double> valueTuple in ColumnWidth)
            {
                UIElement Element2 = valueTuple.Item1;
                double Width = valueTuple.Item2;
                Element2.Arrange(new Rect(X, 0.0, Width, finalSize.Height));
                X += Width;
            }
            return finalSize;
        }

        public ColumnGrid AddChild(ColumnDefinition ColumnDefinition, UIElement Element)
        {
            SetColumnDefinition(Element, ColumnDefinition);
            Children.Add(Element);
            return this;
        }

        public ColumnGrid AddChild(GridLength GridLength, UIElement Element)
        {
            var columnDefinition = new ColumnDefinition
            {
                Width = GridLength
            };
            return AddChild(columnDefinition, Element);
        }

        public static readonly DependencyProperty ColumnDefinitionProperty = DependencyProperty.Register("ColumnDefinition", typeof(ColumnDefinition), typeof(ColumnGrid), new PropertyMetadata(new ColumnDefinition(), delegate (DependencyObject obj, DependencyPropertyChangedEventArgs evargs)
        {
            ColumnGrid Parent = (
               (obj is FrameworkElement frameworkElement) ? frameworkElement.Parent : null
           ) as ColumnGrid;
            Parent?.InvalidateArrange();
        }));
    }
    public class ElementOverlay : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement Element in Children)
            {
                Element.Measure(finalSize);
                Element.Arrange(new Rect(0.0, 0.0, Element.DesiredSize.Width, Element.DesiredSize.Height));
            }
            return finalSize;
        }
    }
    public class Overlay : Grid
    {
        public Overlay AddChild(UIElement Element)
        {
            Children.Add(Element);
            return this;
        }
    }
    public class ReverseRowGrid : RowGrid
    {
        protected override Size Arranging(IEnumerable<UIElement> Children, Size finalSize)
        {
            return base.Arranging(Children.Reverse<UIElement>(), finalSize);
        }
    }
    public class ReverseStackPanel : Panel
    {
        public ReverseStackPanel AddChild(UIElement Child)
        {
            base.Children.Add(Child);
            return this;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement Element2 in base.Children)
            {
                Element2.Measure(availableSize);
            }
            double Height = base.Children.Sum((UIElement Element) => Element.DesiredSize.Height);
            double val;
            if (base.Children.Count != 0)
            {
                val = base.Children.Max((UIElement x) => x.DesiredSize.Width);
            }
            else
            {
                val = 0.0;
            }
            double Width = Math.Min(val, availableSize.Width);
            Width = ((Width == 0.0) ? (Width + 1.0) : Width);
            Height = ((Height == 0.0) ? (Height + 1.0) : Height);
            return new Size(Width, Height);
        }

        protected virtual Size Arranging(IEnumerable<UIElement> Children, Size finalSize)
        {
            double Y = 0.0;
            foreach (UIElement Element in Children)
            {
                Element.Measure(finalSize);
                double Height = Element.DesiredSize.Height;
                Element.Arrange(new Rect(0.0, Y, finalSize.Width, Height));
                Y += Height;
            }
            return finalSize;
        }
    }
    public class RowGrid : Panel
    {
        public static void SetRowDefinition(UIElement element, RowDefinition value)
        {
            element.SetValue(RowGrid.RowDefinitionProperty, value);
        }

        public static RowDefinition GetRowDefinition(UIElement element)
        {
            return (RowDefinition)element.GetValue(RowDefinitionProperty);
        }

        protected override Size ArrangeOverride(Size finalSize) => Arranging(Children, finalSize);
        
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement Element2 in base.Children)
            {
                Element2.Measure(availableSize);
            }
            IEnumerable<ValueTuple<UIElement, RowDefinition>> definition = Children.Select(Element => (Element, GetRowDefinition(Element) ?? new RowDefinition()));
            bool flag = definition.Any(x => x.Item2.Height.GridUnitType == GridUnitType.Star);
            double Height;
            if (flag)
            {
                Height = availableSize.Height;
            }
            else
            {
                Height = definition.Sum(x =>
                {
                    GridUnitType gridUnitType = x.Item2.Height.GridUnitType;
                    GridUnitType gridUnitType2 = gridUnitType;
                    double result;
                    if (gridUnitType2 != GridUnitType.Auto)
                    {
                        if (gridUnitType2 != GridUnitType.Pixel)
                        {
                            result = 0.0;
                        }
                        else
                        {
                            result = x.Item2.Height.Value;
                        }
                    }
                    else
                    {
                        result = x.Item1.DesiredSize.Height;
                    }
                    return result;
                });
            }
            double val;
            if (base.Children.Count != 0)
            {
                val = base.Children.Max((UIElement x) => x.DesiredSize.Width);
            }
            else
            {
                val = 0.0;
            }
            double Width = Math.Min(val, availableSize.Width);
            Width = ((Width == 0.0) ? (Width + 1.0) : Width);
            Height = ((Height == 0.0) ? (Height + 1.0) : Height);
            return new Size(Width, Height);
        }

        protected virtual Size Arranging(IEnumerable<UIElement> Children, Size finalSize)
        {
            var definition = Children.Select(Element => (Element, RowDefinition: GetRowDefinition(Element) ?? new RowDefinition()));
            foreach (UIElement Element3 in Children)
            {
                Element3.Measure(finalSize);
            }
            double TotalNonStar = definition.Sum(x =>
            {
                GridUnitType gridUnitType = x.RowDefinition.Height.GridUnitType;
                GridUnitType gridUnitType2 = gridUnitType;
                double result;
                if (gridUnitType2 != GridUnitType.Auto)
                {
                    if (gridUnitType2 != GridUnitType.Pixel)
                    {
                        result = 0.0;
                    }
                    else
                    {
                        result = x.RowDefinition.Height.Value;
                    }
                }
                else
                {
                    result = x.Element.DesiredSize.Height;
                }
                return result;
            });
            double TotalStar = definition.Sum(x => (x.RowDefinition.Height.GridUnitType == GridUnitType.Star) ? x.RowDefinition.Height.Value : 0.0);
            double Star = (TotalStar == 0.0) ? 0.0 : ((finalSize.Height - TotalNonStar) / TotalStar);
            bool flag = Star < 0.0;
            if (flag)
            {
                Star = 0.0;
            }
            double ProcessValue((UIElement Element, RowDefinition RowDefinition) x)
            {
                double result;
                switch (x.RowDefinition.Height.GridUnitType)
                {
                    case GridUnitType.Auto:
                        result = x.Element.DesiredSize.Height;
                        break;
                    case GridUnitType.Pixel:
                        result = x.RowDefinition.Height.Value;
                        break;
                    case GridUnitType.Star:
                        result = Star * x.RowDefinition.Height.Value;
                        break;
                    default:
                        result = 0.0;
                        break;
                }
                return result;
            }
            IEnumerable<ValueTuple<UIElement, double>> RowHeight = definition.Select(x => (x.Element, ProcessValue(x)));
            double Y = 0.0;
            foreach (ValueTuple<UIElement, double> valueTuple in RowHeight)
            {
                UIElement Element2 = valueTuple.Item1;
                double Height = valueTuple.Item2;
                Element2.Arrange(new Rect(0.0, Y, finalSize.Width, Height));
                Y += Height;
            }
            return finalSize;
        }

        public RowGrid AddChild(RowDefinition RowDefinition, UIElement Element)
        {
            SetRowDefinition(Element, RowDefinition);
            Children.Add(Element);
            return this;
        }

        public RowGrid AddChild(GridLength GridLength, UIElement Element)
        {
            RowDefinition rowDefinition = new RowDefinition
            {
                Height = (GridLength)
            };
            return AddChild(rowDefinition, Element);
        }

        public static readonly DependencyProperty RowDefinitionProperty = DependencyProperty.Register("RowDefinition", typeof(RowDefinition), typeof(RowGrid), new PropertyMetadata(new RowDefinition(), delegate (DependencyObject obj, DependencyPropertyChangedEventArgs evargs)
        {
            Panel Parent = ((obj is FrameworkElement frameworkElement) ? frameworkElement.Parent : null) as Panel;
            Parent?.InvalidateArrange();
        }));
    }
    public static class Extension
    {
        public static IEnumerable<(int Index, T Element)> Enumerate<T>(this IEnumerable<T> Enumerable)
        {
            int i = 0;
            foreach (var element in Enumerable) yield return (i++, element);
        }
        public static T Edit<T>(this T Value, Action<T> Action)
        {
            Action?.Invoke(Value);
            return Value;
        }
        public static T AddChild<T>(this T Value, UIElement Child) where T : Panel
        {
            Value.Children.Add(Child);
            return Value;
        }
        public static T SetContent<T>(this T Value, UIElement Child) where T : UserControl
        {
            Value.Content = Child;
            return Value;
        }
        public static T SetContent<T>(this T Value, object Child) where T : ContentControl
        {
            Value.Content = Child;
            return Value;
        }
        public static T SetToVariable<T>(this T Value, out T Varable)
        {
            Varable = Value;
            return Value;
        }

        public static Image SetImageFromAsset(this Image Value, string AssetName)
        {
            var b = new BitmapImage
            {
                UriSource = new Uri($"ms-appx:///Assets/{AssetName}")
            };
            Value.Source = b;
            return Value;
        }
    }
}