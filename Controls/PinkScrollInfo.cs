using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PinkWpf.Controls
{
    public class PinkScrollInfo : ContentControl, IScrollInfo
    {
        public ScrollViewer ScrollOwner { get; set; }
        public bool CanHorizontallyScroll { get; set; }
        public bool CanVerticallyScroll { get; set; }

        private static readonly Size _positiveInfiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        private const double _lineSize = 16;
        private const double _wheelSize = 3 * _lineSize;
        private Size _extent;
        private Size _viewport;
        private Point _targetOffset;
        private DoubleAnimationUsingKeyFrames _verticalScrollAnimation = new DoubleAnimationUsingKeyFrames()
        {
            KeyFrames = new DoubleKeyFrameCollection() { new SplineDoubleKeyFrame() }
        };
        private DoubleAnimationUsingKeyFrames _horizontalScrollAnimation = new DoubleAnimationUsingKeyFrames()
        {
            KeyFrames = new DoubleKeyFrameCollection() { new SplineDoubleKeyFrame() }
        };

        public double ExtentHeight => _extent.Height;
        public double ExtentWidth => _extent.Width;
        public double ViewportHeight => _viewport.Height;
        public double ViewportWidth => _viewport.Width;
        public Point TargetOffset => _targetOffset;
        public bool IsMouseWheelHorizontalScrolling =>
            Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                (ScrollOwner.ComputedVerticalScrollBarVisibility != Visibility.Visible && ScrollOwner.ComputedHorizontalScrollBarVisibility == Visibility.Visible);

        protected override Size MeasureOverride(Size availableSize)
        {
            var content = (UIElement)VisualTreeHelper.GetChild(this, 0);
            var measureSize = _positiveInfiniteSize;
            if (ScrollOwner.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled)
                measureSize = new Size(measureSize.Width, availableSize.Height);
            if (ScrollOwner.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
                measureSize = new Size(availableSize.Width, measureSize.Height);
            content?.Measure(measureSize);

            VerifyScrollData(availableSize, content.DesiredSize);

            return _viewport;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Content == null)
                return finalSize;

            var content = (UIElement)VisualTreeHelper.GetChild(this, 0);
            var rect = new Rect(-HorizontalOffset, -VerticalOffset, content.DesiredSize.Width, content.DesiredSize.Height);
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
                rect.Width = Math.Max(content.DesiredSize.Width, finalSize.Width);
            if (VerticalAlignment == VerticalAlignment.Stretch)
                rect.Height = Math.Max(content.DesiredSize.Height, finalSize.Height);
            content.Arrange(rect);

            VerifyScrollData(finalSize, new Size(content.DesiredSize.Width, content.DesiredSize.Height));

            return finalSize;
        }

        public void SetHorizontalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
            if (offset != _targetOffset.X)
            {
                _targetOffset.X = offset;

                var pinkScrollViewer = GetPinkScrollViewer();
                var splineDoubleKeyFrame = (SplineDoubleKeyFrame)_horizontalScrollAnimation.KeyFrames[0];
                splineDoubleKeyFrame.Value = offset;
                splineDoubleKeyFrame.KeyTime = pinkScrollViewer.ScrollingTime;
                splineDoubleKeyFrame.KeySpline = pinkScrollViewer.ScrollingSpline;

                BeginAnimation(HorizontalOffsetProperty, _horizontalScrollAnimation, HandoffBehavior.Compose);
            }
        }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
            if (offset != _targetOffset.Y)
            {
                _targetOffset.Y = offset;

                var pinkScrollViewer = GetPinkScrollViewer();
                var splineDoubleKeyFrame = (SplineDoubleKeyFrame)_verticalScrollAnimation.KeyFrames[0];
                splineDoubleKeyFrame.Value = offset;
                splineDoubleKeyFrame.KeyTime = pinkScrollViewer.ScrollingTime;
                splineDoubleKeyFrame.KeySpline = pinkScrollViewer.ScrollingSpline;

                BeginAnimation(VerticalOffsetProperty, _verticalScrollAnimation, HandoffBehavior.Compose);
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null || visual == this || !IsAncestorOf(visual))
                return Rect.Empty;

            var elementRectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);

            if (ActualHeight > elementRectangle.Height)
            {
                if (elementRectangle.Y < 0)
                {
                    if (elementRectangle.Y + elementRectangle.Height < -ActualHeight / 2)
                        BringIntoViewVerticalCenter(elementRectangle);
                    else
                        BringIntoViewVerticalTop(elementRectangle);
                }
                else if (elementRectangle.Y + elementRectangle.Height > ActualHeight)
                {
                    if (elementRectangle.Y > ActualHeight * 1.5)
                        BringIntoViewVerticalCenter(elementRectangle);
                    else
                        BringIntoViewVerticalBottom(elementRectangle);
                }
            }
            if (ActualWidth > elementRectangle.Width)
            {
                if (elementRectangle.X < 0)
                {
                    if (elementRectangle.X + elementRectangle.Width < -ActualWidth / 2)
                        BringIntoViewHorizontalCenter(elementRectangle);
                    else
                        BringIntoViewHorizontalLeft(elementRectangle);
                }
                else if (elementRectangle.X + elementRectangle.Width > ActualWidth)
                {
                    if (elementRectangle.X > ActualWidth * 1.5)
                        BringIntoViewHorizontalCenter(elementRectangle);
                    else
                        BringIntoViewHorizontalRight(elementRectangle);
                }
            }

            return rectangle;
        }

        // [------]
        //  ------
        //  ------
        private void BringIntoViewVerticalTop(Rect elementRect) =>
            SetVerticalOffset(VerticalOffset - (elementRect.Height - (elementRect.Y + elementRect.Height)));

        private void BringIntoViewHorizontalLeft(Rect elementRect) =>
            SetHorizontalOffset(HorizontalOffset - (elementRect.Width - (elementRect.X + elementRect.Width)));

        //  ------
        // [------]
        //  ------
        private void BringIntoViewVerticalCenter(Rect elementRect) =>
            SetVerticalOffset(VerticalOffset - (elementRect.Height - (elementRect.Y + elementRect.Height)) - (ActualHeight / 2 - elementRect.Height / 2));

        private void BringIntoViewHorizontalCenter(Rect elementRect) =>
            SetHorizontalOffset(HorizontalOffset - (elementRect.Width - (elementRect.X + elementRect.Width)) - (ActualWidth / 2 - elementRect.Width / 2));

        //  ------
        //  ------
        // [------]
        private void BringIntoViewVerticalBottom(Rect elementRect) =>
            SetVerticalOffset(VerticalOffset + (elementRect.Height - (ActualHeight - elementRect.Y)));

        private void BringIntoViewHorizontalRight(Rect elementRect) =>
            SetHorizontalOffset(HorizontalOffset + (elementRect.Width - (ActualWidth - elementRect.X)));


        protected void VerifyScrollData(Size viewport, Size extent)
        {
            if (double.IsInfinity(viewport.Width))
                viewport.Width = extent.Width;

            if (double.IsInfinity(viewport.Height))
                viewport.Height = extent.Height;

            _extent = extent;
            _viewport = viewport;

            var scrollWidth = ExtentWidth - ViewportWidth;
            var scrollHeight = ExtentHeight - ViewportHeight;
            if (scrollWidth < 0)
                scrollWidth = 0;
            if (scrollHeight < 0)
                scrollHeight = 0;

            if (_targetOffset.X > scrollWidth)
                SetHorizontalOffset(scrollWidth);
            if (_targetOffset.Y > scrollHeight)
                SetVerticalOffset(scrollHeight);

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }

        private PinkScrollViewer GetPinkScrollViewer()
        {
            if (ScrollOwner is PinkScrollViewer)
                return (PinkScrollViewer)ScrollOwner;
            throw new Exception("ScrollOwner.GetType() != typeof(PinkScrollViewer)");
        }

        #region HorizontalOffsetProperty
        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
        }

        private void SetCurrentHorizontalOffset(double value)
        {
            SetCurrentValue(HorizontalOffsetProperty, value);
        }

        internal readonly static DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(PinkScrollInfo), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, OnHorizontalOffsetChanged));

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkScrollInfo = (PinkScrollInfo)d;
            pinkScrollInfo.VisualOffset = new Vector(-(double)e.NewValue, pinkScrollInfo.VerticalOffset);
            pinkScrollInfo.ScrollOwner.InvalidateScrollInfo();
        }
        #endregion

        #region VerticalOffsetProperty
        public double VerticalOffset
        {
            get => (double)GetValue(VerticalOffsetProperty);
        }

        private void SetCurrentVerticalOffset(double value)
        {
            SetCurrentValue(VerticalOffsetProperty, value);
        }

        internal readonly static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(PinkScrollInfo), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, OnVerticalOffsetChanged));

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkScrollInfo = (PinkScrollInfo)d;
            pinkScrollInfo.VisualOffset = new Vector(pinkScrollInfo.HorizontalOffset, -(double)e.NewValue);
            pinkScrollInfo.ScrollOwner.InvalidateScrollInfo();
        }
        #endregion

        #region Movement Methods
        public void LineDown()
        { SetVerticalOffset(VerticalOffset + _lineSize); }

        public void LineUp()
        { SetVerticalOffset(VerticalOffset - _lineSize); }

        public void LineLeft()
        { SetHorizontalOffset(HorizontalOffset - _lineSize); }

        public void LineRight()
        { SetHorizontalOffset(HorizontalOffset + _lineSize); }

        public void MouseWheelDown()
        {
            if (IsMouseWheelHorizontalScrolling)
                MouseWheelRight();
            else
                SetVerticalOffset(VerticalOffset + _wheelSize);
        }

        public void MouseWheelUp()
        {
            if (IsMouseWheelHorizontalScrolling)
                MouseWheelLeft();
            else
                SetVerticalOffset(VerticalOffset - _wheelSize);
        }

        public void MouseWheelLeft()
        { SetHorizontalOffset(HorizontalOffset - _wheelSize); }

        public void MouseWheelRight()
        { SetHorizontalOffset(HorizontalOffset + _wheelSize); }

        public void PageDown()
        { SetVerticalOffset(VerticalOffset + ViewportHeight); }

        public void PageUp()
        { SetVerticalOffset(VerticalOffset - ViewportHeight); }

        public void PageLeft()
        { SetHorizontalOffset(HorizontalOffset - ViewportWidth); }

        public void PageRight()
        { SetHorizontalOffset(HorizontalOffset + ViewportWidth); }
        #endregion
    }
}