using GuideLine.Core.Elements;
using GuideLine.WPF.Extensions;
using GuideLine.WPF.View;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GuideLine.WPF.Control
{
    public class GuideLineHighlighter : Grid
    {
        #region Dependency Properties           
        public static readonly DependencyProperty GuideLineStepProperty = DependencyProperty.Register(nameof(GuideLineStep), typeof(GuideLineStep), typeof(GuideLineHighlighter), new PropertyMetadata(null, OnGuideLineStepChangedCallback));
        public GuideLineStep GuideLineStep
        {
            get { return (GuideLineStep)GetValue(GuideLineStepProperty); }
            set { SetValue(GuideLineStepProperty, value); }
        }
        private static void OnGuideLineStepChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLineHighlighter)?.OnGuideLineStepChanged((GuideLineStep)e.NewValue);
        }
        #endregion


        private Grid TutorialOverlay;
        private Path OverlayPath;
        private Border InfoPanel;

        private double _highlightMargin;
        private double _highlightCornerRadius;
        private bool _animateDialog;
        private TimeSpan _animationDuration;


        public GuideLineHighlighter()
        {
            CreateTutorialOverlay();

            Loaded += GuideLineHighlighter_Loaded;
            SizeChanged += GuideLineHighlighter_SizeChanged;
        }

        public void UpdateHighlightMarginProperty(double highlightMargin)
        {
            _highlightMargin = highlightMargin;
            RefreshUI();
        }
        public void UpdateHighlightCornerRadius(double highlightCornerRadius)
        {
            _highlightCornerRadius = highlightCornerRadius;
            RefreshUI();
        }
        public void UpdateDialogAnimated(bool animateDialog)
        {
            _animateDialog = animateDialog;
        }
        public void UpdateDialogAnimationDuration(TimeSpan animationDuration)
        {
            _animationDuration = animationDuration;
        }
        private void RefreshUI()
        {
            var uiElements = GetUIElements();
            if (uiElements != null && InfoPanel != null)
            {
                HighlightFeatures(uiElements);
                PositionInfoPanel(GetMaximumCombinedBounds(uiElements));
            }
        }


        private void CreateTutorialOverlay()
        {
            // Create the Grid for the overlay
            TutorialOverlay = new Grid()
            {
                Name = "TutorialOverlay",
            };

            // Create the Path for the overlay
            OverlayPath = new Path
            {
                Name = "OverlayPath",
                Fill = new SolidColorBrush(Colors.Black) { Opacity = 0.6 },
                Stretch = Stretch.None,
                Style = null
            };

            OverlayPath.LayoutUpdated += TutorialOverlay_LayoutUpdated;

            // Add the Path to the Grid
            TutorialOverlay.Children.Add(OverlayPath);

            // Add the TutorialOverlay to the main grid
            Children.Add(TutorialOverlay);
        }       
        private void CreateInfoPanel()
        {
            // Get or Create the InfoPanel
            InfoPanel = this.FindChild<Border>("InfoPanel");
            if (InfoPanel == null)
            {                
                InfoPanel = new Border
                {
                    Name = "InfoPanel",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                InfoPanel.Margin = new Thickness((ActualWidth - InfoPanel.ActualWidth) / 2, (ActualHeight - InfoPanel.ActualHeight) / 2, 0, 0);

                // Add the InfoPanel to the Grid
                TutorialOverlay.Children.Add(InfoPanel);
            }

            InfoPanel.Child = new ContentControl()
            {
                Name = "InfoPanelContent",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = new GuideLine_Dialog_View(),
                Margin = new Thickness(0),
                Padding = new Thickness(0),
            };
        }




        private IEnumerable<UIElement> GetUIElements()
        {
            if(this.GuideLineStep == null)
            {
                return new List<UIElement>();
            }

            var uiElements = new List<UIElement>();

            if (this.GuideLineStep.UiElements != null)
            {
                uiElements.AddRange(this.GuideLineStep.UiElements);
            }

            if (this.GuideLineStep.UiElementNames != null)
            {
                foreach (string elementName in this.GuideLineStep.UiElementNames)
                {
                    var contentAncestor = GetGuideLineViewAncestor();
                    UIElement element = contentAncestor.FindChild(elementName);
                    if (element != null)
                    {
                        uiElements.Add(element);
                    }
                    else
                    {
                        throw new Exception($"Failed to find {elementName} from the visual tree of GuideLine_View's direct ancestor");
                    }
                }
            }

            return uiElements;
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
                uiElementBounds.Left - _highlightMargin,
                uiElementBounds.Top - _highlightMargin,
                uiElementBounds.Width + 2 * _highlightMargin,
                uiElementBounds.Height + 2 * _highlightMargin
            );
            RectangleGeometry highlightGeometry = new RectangleGeometry(highlightBounds, _highlightCornerRadius, _highlightCornerRadius);

            return highlightGeometry;
        }


        private void PositionInfoPanel(Rect highlightBounds)
        {
            double panelWidth = InfoPanel.ActualWidth;
            double panelHeight = InfoPanel.ActualHeight;
            double finalLeft = (OverlayPath .ActualWidth / 2) - (InfoPanel.ActualWidth / 2);
            double finalTop = (OverlayPath.ActualHeight / 2) - (InfoPanel.ActualHeight / 2);

            if (highlightBounds != Rect.Empty)
            {
                //Try positioning the InfoPanel outside the right edge of the highlight
                finalLeft = GetHorizontalPosition(highlightBounds, panelWidth, true);

                if (finalLeft != -1)
                {
                    finalTop = GetVerticalPosition(highlightBounds, panelHeight, false);
                }
                else
                {
                    finalLeft = GetHorizontalPosition(highlightBounds, panelWidth, false);
                    finalTop = GetVerticalPosition(highlightBounds, panelHeight, true);
                }
            }

            //animate the infopanel margin
            ThicknessAnimation animation = new ThicknessAnimation
            {
                From = InfoPanel.Margin,
                To = new Thickness(finalLeft, finalTop, 0, 0),
                Duration = _animateDialog ? _animationDuration : new TimeSpan(0),
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
                if (highlightBounds.Right + _highlightMargin < ActualWidth)
                {
                    minX = highlightBounds.Right + _highlightMargin;
                    maxX = ActualWidth - panelWidth;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return minX;
                    }
                }

                //Try positioning outside the left edge of the highlight
                if (highlightBounds.Left - _highlightMargin > 0)
                {
                    minX = 0;
                    maxX = highlightBounds.Left - _highlightMargin - panelWidth;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return maxX;
                    }
                }
            }
            else
            {
                //Try positioning Inside the highlight at the right edge
                if (highlightBounds.Right + _highlightMargin < ActualWidth)
                {
                    minX = highlightBounds.Right + _highlightMargin - panelWidth;
                    maxX = highlightBounds.Right + _highlightMargin;

                    if (minX >= 0 && maxX >= 0 && minX < maxX)
                    {
                        return maxX;
                    }
                }


                //Try positioning Inside the highlight at the left edge
                if (highlightBounds.Left - _highlightMargin > 0)
                {
                    minX = highlightBounds.Left - _highlightMargin;
                    maxX = highlightBounds.Left - _highlightMargin + panelWidth;

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
                if (highlightBounds.Bottom + _highlightMargin < ActualHeight)
                {
                    minY = highlightBounds.Bottom + _highlightMargin;
                    maxY = ActualHeight - panelHeight;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return minY;
                    }
                }

                // Try positioning outside the top edge of the highlight
                if (highlightBounds.Top - _highlightMargin > 0)
                {
                    minY = 0;
                    maxY = highlightBounds.Top - _highlightMargin - panelHeight;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return maxY;
                    }
                }
            }
            else
            {
                // Try positioning inside the highlight at the bottom edge
                if (highlightBounds.Bottom + _highlightMargin < ActualHeight)
                {
                    minY = highlightBounds.Bottom + _highlightMargin - panelHeight;
                    maxY = highlightBounds.Bottom + _highlightMargin;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return minY;
                    }
                }

                // Try positioning inside the highlight at the top edge
                if (highlightBounds.Top - _highlightMargin > 0)
                {
                    minY = highlightBounds.Top - _highlightMargin;
                    maxY = highlightBounds.Top - _highlightMargin + panelHeight;

                    if (minY >= 0 && maxY >= 0 && minY < maxY)
                    {
                        return minY;
                    }
                }
            }

            return -1;
        }



        private FrameworkElement GetGuideLineViewAncestor()
        {
            GuideLine_View guideLine_View = this.FindParent<GuideLine_View>();
            var parent = VisualTreeHelper.GetParent(guideLine_View) as FrameworkElement;
            return parent;
        }
        private Rect GetElementBounds(UIElement element)
        {            
            var parent = GetGuideLineViewAncestor();

            try
            {
                GeneralTransform transform = element.TransformToVisual(parent);
                Point topLeft = transform.Transform(new Point(0, 0));
                return new Rect(topLeft, new Size(element.RenderSize.Width, element.RenderSize.Height));
            }
            catch (Exception ex)
            {
                string elementName = element.GetValue(FrameworkElement.NameProperty) as string;
                throw new Exception($"{elementName} ({element.GetType()}), GuideLine_View :\n{ex.Message}");
            }            
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
            RefreshUI();
        }
        private void TutorialOverlay_LayoutUpdated(object sender, EventArgs e)
        {
            var uiElements = GetUIElements();
            if (InfoPanel != null)
            {
                PositionInfoPanel(GetMaximumCombinedBounds(uiElements));
            }
        }

        private void OnGuideLineStepChanged(GuideLineStep guideLineStep)
        {
            RefreshUI();
        }
        #endregion
    }
}
