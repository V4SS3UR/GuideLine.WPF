using GuideLine.WPF;
using GuideLine.WPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace GuideLine.Core.Elements
{
    public class GuideLineManager : ObservableObject
    {
        public event Action OnGuideLineListCompleted;

        private static List<GuideLineManager> instances = new List<GuideLineManager>();

        public string Guid { get; private set; } = System.Guid.NewGuid().ToString();
        private GuideLine_View _guideLine_View;

        public RelayCommand ShowNextGuideLineStepCommand { get; set; }
        public RelayCommand ShowPreviousGuideLineStepCommand { get; set; }
        public RelayCommand SkipGuideLineCommand { get; set; }
        public RelayCommand CloseGuideLineCommand { get; set; }



        public ObservableCollection<GuideLine> GuideLines;
        private GuideLine _currentGuideLine; public GuideLine CurrentGuideLine
        {
            get { return _currentGuideLine; }
            set { _currentGuideLine = value; OnPropertyChanged(); }
        }


        public GuideLineManager()
        {
            instances.Add(this);

            GuideLines = new ObservableCollection<GuideLine>();

            ShowNextGuideLineStepCommand = new RelayCommand(command => CurrentGuideLine.ShowNextStep());
            ShowPreviousGuideLineStepCommand = new RelayCommand(command => CurrentGuideLine.ShowPreviousStep());
            SkipGuideLineCommand = new RelayCommand(command => CurrentGuideLine.StopGuideLine());
            CloseGuideLineCommand = new RelayCommand(command => StopGuideLine());
        }

        public void AddGuideLine(GuideLine guideLine)
        {
            GuideLines.Add(guideLine);
            guideLine.OnGuideLineCompleted += () => ShowNextGuideLine();
        }


        public void StartGuideLine(string guideLineName)
        {
            Debug.WriteLine("start " + Guid);

            _guideLine_View = Application.Current.MainWindow.FindChild<GuideLine_View>(guideLineName);

            _guideLine_View.GuideLineHighlighter.IsEnabled = true;
            _guideLine_View.DataContext = this;
            _guideLine_View.IsEnabled = true;

            CurrentGuideLine = GuideLines.First();
            CurrentGuideLine.StartGuideLine();
        }
        public void StopGuideLine()
        {
            Debug.WriteLine("stop " + Guid);

            this.CurrentGuideLine = null;
            this.GuideLines.Clear();

            _guideLine_View.GuideLineHighlighter.IsEnabled = false;
            _guideLine_View.DataContext = null;
            _guideLine_View = null;

            OnGuideLineListCompleted?.Invoke();

            instances.Remove(this);
        }


        public void ShowNextGuideLine()
        {
            int currentIndex = GuideLines.IndexOf(CurrentGuideLine);

            if (currentIndex < GuideLines.Count - 1)
            {
                CurrentGuideLine = GuideLines[currentIndex + 1];
                CurrentGuideLine.StartGuideLine();
            }
            else
            {
                StopGuideLine();
            }
        }
        public void ShowPreviousGuideLine()
        {
            int currentIndex = GuideLines.IndexOf(CurrentGuideLine);
            if (currentIndex > 0)
            {
                CurrentGuideLine = GuideLines[currentIndex - 1];
                CurrentGuideLine.StartGuideLine();
            }
            else
            {
                StopGuideLine();
            }
        }
    }
}
