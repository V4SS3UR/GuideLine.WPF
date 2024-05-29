
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using GuideLine.WPF.View;
using System.Collections.Generic;
using System;
using System.Linq;
using GuideLine.WPF;

namespace GuideLine.Core
{
    public class GuideLineHighlighter : Grid
    {
        #region Dependency Properties
        public static readonly DependencyProperty HighlightMarginProperty = DependencyProperty.Register(nameof(HighlightMargin), typeof(double), typeof(GuideLineHighlighter), new PropertyMetadata(0.0, OnOnHighlightMarginChangedCallback));
        private static void OnOnHighlightMarginChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLineHighlighter)?.OnHighlightMarginChanged((double)e.NewValue);
        }
        public double HighlightMargin
        {
            get { return (double)GetValue(HighlightMarginProperty); }
            set { SetValue(HighlightMarginProperty, value); }
        }

        public static readonly DependencyProperty HighlightCornerRadiusProperty = DependencyProperty.Register(nameof(HighlightCornerRadius), typeof(double), typeof(GuideLineHighlighter), new FrameworkPropertyMetadata(0.0));

        public double HighlightCornerRadius
        {
            get { return (double)GetValue(HighlightCornerRadiusProperty); }
            set { SetValue(HighlightCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty HighlightedUiElementsProperty = DependencyProperty.Register(nameof(HighlightedUiElements), typeof(IEnumerable<UIElement>), typeof(GuideLineHighlighter), new PropertyMetadata(null, OnHighlightedUiElementsChangedCallback));

        public IEnumerable<UIElement> HighlightedUiElements
        {
            get { return (IEnumerable<UIElement>)GetValue(HighlightedUiElementsProperty); }
            set { SetValue(HighlightedUiElementsProperty, value); }
        }

        private static void OnHighlightedUiElementsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLineHighlighter)?.OnHighlightedUiElementsChanged((IEnumerable<UIElement>)e.NewValue);
        }
        #endregion


        private Grid TutorialOverlay;
        private Path OverlayPath;
        private Border InfoPanel;


        public GuideLineHighlighter()
        {
            CreateTutorialOverlay();

            Loaded += GuideLineHighlighter_Loaded;
            SizeChanged += GuideLineHighlighter_SizeChanged;
        }

        

        private void CreateTutorialOverlay()
        {
            // Create the Grid for the overlay
            TutorialOverlay = new Grid();

            // Create the Path for the overlay
            OverlayPath = new Path
            {
                Fill = new SolidColorBrush(Colors.Black) { Opacity = 0.6 },
                Stretch = Stretch.Fill,
                Style = null
            };

            OverlayPath.LayoutUpdated += OverlayPath_LayoutUpdated;

            // Add the Path to the Grid
            TutorialOverlay.Children.Add(OverlayPath);

            // Add the TutorialOverlay to the main grid
            Children.Add(TutorialOverlay);
        }
        private void CreateInfoPanel()
        {
            // Create the InfoPanel
            InfoPanel = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            InfoPanel.Child = new ContentControl()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = new GuideLine_Dialog_View(),
                DataContext = DataContext,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
            };

            // Add the InfoPanel to the Grid
            TutorialOverlay.Children.Add(InfoPanel);
        }

        private void HighlightFeatures(IEnumerable<UIElement> uIElements)
        {
            double width = TutorialOverlay.ActualWidth;
            double height = TutorialOverlay.ActualHeight;

            // Create the full screen rectangle
            RectangleGeometry screenGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

            Geometry geometry = screenGeometry;
            foreach (UIElement uIElement in uIElements)
            {
                RectangleGeometry highlightGeometry = GetUiElementGeometry(uIElement);

                // Combine geometries to create a "hole" in the overlay
                geometry = new CombinedGeometry(GeometryCombineMode.Exclude, geometry, highlightGeometry);
            }

            OverlayPath.Data = geometry;

        }
        private RectangleGeometry GetUiElementGeometry(UIElement uIElement)
        {
            Rect uiElementBounds = GetElementBounds(uIElement);

            // Create the highlight rectangle with margin and rounded corners
            Rect highlightBounds = new Rect(
                uiElementBounds.Left - HighlightMargin,
                uiElementBounds.Top - HighlightMargin,
                uiElementBounds.Width + 2 * HighlightMargin,
                uiElementBounds.Height + 2 * HighlightMargin
            );
            RectangleGeometry highlightGeometry = new RectangleGeometry(highlightBounds, HighlightCornerRadius, HighlightCornerRadius);

            return highlightGeometry;
        }


        private void PositionInfoPanel(Rect highlightBounds)
        {
            if (HighlightedUiElements == null || !HighlightedUiElements.Any())
            {
                // If there are no highlighted elements, position the InfoPanel in the center of the screen
                InfoPanel.Margin = new Thickness((ActualWidth - InfoPanel.ActualWidth) / 2, (ActualHeight - InfoPanel.ActualHeight) / 2, 0, 0);
                return;
            }

            double panelWidth = InfoPanel.ActualWidth;
            double panelHeight = InfoPanel.ActualHeight;

            //Try positioning the InfoPanel outside the right edge of the highlight
            double finalLeft = GetHorizontalPosition(highlightBounds, panelWidth, true);
            double finalTop;

            if (finalLeft != -1)
            {
                finalTop = GetVerticalPosition(highlightBounds, panelHeight, false);
            }
            else
            {
                finalLeft = GetHorizontalPosition(highlightBounds, panelWidth, false);
                finalTop = GetVerticalPosition(highlightBounds, panelHeight, true);
            }

            //animate the infopanel margin
            ThicknessAnimation animation = new ThicknessAnimation
            {
                From = InfoPanel.Margin,
                To = new Thickness(finalLeft, finalTop, 0, 0),
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            InfoPanel.BeginAnimation(MarginProperty, animation);
        }
        private double GetHorizontalPosition(Rect highlightBounds, double panelWidth, bool outside)
        {
            double minX;
            double maxX;

            if (outside)
            {
                //Try positioning outside the right edge of the highlight
                if (highlightBounds.Right + HighlightMargin < ActualWidth)
                {
                    minX = highlightBounds.Right + HighlightMargin;
                    maxX = ActualWidth - panelWidth;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return minX;
                    }
                }

                //Try positioning outside the left edge of the highlight
                if (highlightBounds.Left - HighlightMargin > 0)
                {
                    minX = 0;
                    maxX = highlightBounds.Left - HighlightMargin - panelWidth;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return maxX;
                    }
                }
            }
            else
            {
                //Try positioning Inside the highlight at the right edge
                if (highlightBounds.Right + HighlightMargin < ActualWidth)
                {
                    minX = highlightBounds.Right + HighlightMargin - panelWidth;
                    maxX = highlightBounds.Right + HighlightMargin;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return maxX;
                    }
                }


                //Try positioning Inside the highlight at the left edge
                if (highlightBounds.Left - HighlightMargin > 0)
                {
                    minX = highlightBounds.Left - HighlightMargin;
                    maxX = highlightBounds.Left - HighlightMargin + panelWidth;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return minX;
                    }
                }
            }

            return -1;
        }
        private double GetVerticalPosition(Rect highlightBounds, double panelHeight, bool outside)
        {
            double minY;
            double maxY;

            if (outside)
            {
                // Try positioning outside the bottom edge of the highlight
                if (highlightBounds.Bottom + HighlightMargin < ActualHeight)
                {
                    minY = highlightBounds.Bottom + HighlightMargin;
                    maxY = ActualHeight - panelHeight;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return minY;
                    }
                }

                // Try positioning outside the top edge of the highlight
                if (highlightBounds.Top - HighlightMargin > 0)
                {
                    minY = 0;
                    maxY = highlightBounds.Top - HighlightMargin - panelHeight;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return maxY;
                    }
                }
            }
            else
            {
                // Try positioning inside the highlight at the bottom edge
                if (highlightBounds.Bottom + HighlightMargin < ActualHeight)
                {
                    minY = highlightBounds.Bottom + HighlightMargin - panelHeight;
                    maxY = highlightBounds.Bottom + HighlightMargin;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return minY;
                    }
                }

                // Try positioning inside the highlight at the top edge
                if (highlightBounds.Top - HighlightMargin > 0)
                {
                    minY = highlightBounds.Top - HighlightMargin;
                    maxY = highlightBounds.Top - HighlightMargin + panelHeight;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return minY;
                    }
                }
            }

            return -1;
        }




        private Rect GetElementBounds(UIElement element)
        {
            GuideLine_View guideLine_View = this.FindParent<GuideLine_View>();
            var parent = VisualTreeHelper.GetParent(guideLine_View) as FrameworkElement;
            GeneralTransform transform = element.TransformToVisual(parent);
            Point topLeft = transform.Transform(new Point(0, 0));
            return new Rect(topLeft, new Size(element.RenderSize.Width, element.RenderSize.Height));
        }
        private Rect GetMaximumCombinedBounds(IEnumerable<UIElement> elements)
        {
            if (elements == null || !elements.Any())
            {
                return Rect.Empty;
            }

            // Initialize the combined bounds with the first element's bounds
            Rect combinedBounds = GetElementBounds(elements.First());

            // Iterate over the remaining elements and union their bounds with the combined bounds
            foreach (var element in elements.Skip(1))
            {
                Rect elementBounds = GetElementBounds(element);
                combinedBounds = Rect.Union(combinedBounds, elementBounds);
            }

            return combinedBounds;
        }

        #region event handlers
        private void GuideLineHighlighter_Loaded(object sender, RoutedEventArgs e)
        {
            CreateInfoPanel();
        }

        private void GuideLineHighlighter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (HighlightedUiElements != null && InfoPanel != null)
            {
                HighlightFeatures(HighlightedUiElements);
                PositionInfoPanel(GetMaximumCombinedBounds(HighlightedUiElements));
            }
        }
        private void OverlayPath_LayoutUpdated(object sender, EventArgs e)
        {
            if (InfoPanel != null)
            {
                PositionInfoPanel(GetMaximumCombinedBounds(HighlightedUiElements));
            }
        }

        private void OnHighlightMarginChanged(double newMargin)
        {
            if (HighlightedUiElements != null && InfoPanel != null)
            {
                HighlightFeatures(HighlightedUiElements);
            }
        }
        private void OnHighlightedUiElementsChanged(IEnumerable<UIElement> newElements)
        {
            if (newElements != null && InfoPanel != null)
            {
                HighlightFeatures(newElements);
            }
        }
        #endregion
    }
}
