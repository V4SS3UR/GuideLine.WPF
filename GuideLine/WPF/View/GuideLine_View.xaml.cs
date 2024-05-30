using GuideLine.WPF.Control;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GuideLine.WPF.View
{
    /// <summary>
    /// Logique d'interaction pour GuideLine_View.xaml
    /// </summary>
    public partial class GuideLine_View : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty HighlightMarginProperty = DependencyProperty.Register(nameof(HighlightMargin), typeof(double), typeof(GuideLine_View), new PropertyMetadata(0.0, OnHighlightMarginChangedCallback));
        private static void OnHighlightMarginChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLine_View)?.OnHighlightMarginChanged((double)e.NewValue);
        }
        public double HighlightMargin
        {
            get { return (double)GetValue(HighlightMarginProperty); }
            set { SetValue(HighlightMarginProperty, value); }
        }


        public static readonly DependencyProperty HighlightCornerRadiusProperty = DependencyProperty.Register(nameof(HighlightCornerRadius), typeof(double), typeof(GuideLine_View), new PropertyMetadata(0.0, OnHighlightCornerRadiusChangedCallback));
        private static void OnHighlightCornerRadiusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLine_View)?.OnHighlightCornerRadiusChanged((double)e.NewValue);
        }
        public double HighlightCornerRadius
        {
            get { return (double)GetValue(HighlightCornerRadiusProperty); }
            set { SetValue(HighlightCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty AnimateDialogProperty = DependencyProperty.Register(nameof(AnimateDialog), typeof(bool), typeof(GuideLine_View), new PropertyMetadata(true, OnAnimateDialogChangedCallback));
        private static void OnAnimateDialogChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLine_View)?.OnAnimateDialogChanged((bool)e.NewValue);
        }
        public bool AnimateDialog
        {
            get { return (bool)GetValue(AnimateDialogProperty); }
            set { SetValue(AnimateDialogProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof(AnimationDuration), typeof(TimeSpan), typeof(GuideLine_View), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 300), OnAnimationDurationChangedCallback));
        private static void OnAnimationDurationChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GuideLine_View)?.OnAnimationDurationChanged((TimeSpan)e.NewValue);
        }
        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }
        #endregion


        GuideLineHighlighter _highlighter;

        public GuideLine_View()
        {
            InitializeComponent();
            _highlighter = this.GuideLineHighlighter;

            this.Loaded += GuideLine_View_Loaded;
        }

        private void GuideLine_View_Loaded(object sender, RoutedEventArgs e)
        {
            _highlighter.UpdateHighlightMarginProperty(HighlightMargin);
            _highlighter.UpdateHighlightCornerRadius(HighlightCornerRadius);
            _highlighter.UpdateDialogAnimated(AnimateDialog);
            _highlighter.UpdateDialogAnimationDuration(AnimationDuration);
        }


        private void OnHighlightMarginChanged(double newMargin)
        {
            _highlighter.UpdateHighlightMarginProperty(newMargin);
        }
        private void OnHighlightCornerRadiusChanged(double newCornerRadius)
        {
            _highlighter.UpdateHighlightCornerRadius(newCornerRadius);
        }
        private void OnAnimateDialogChanged(bool animateDialog)
        {
            _highlighter.UpdateDialogAnimated(animateDialog);
        }
        private void OnAnimationDurationChanged(TimeSpan animationDuration)
        {
            _highlighter.UpdateDialogAnimationDuration(animationDuration);
        }
    }
}
