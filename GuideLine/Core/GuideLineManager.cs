using GuideLine.Core.Elements;
using GuideLine.WPF;
using GuideLine.WPF.Extensions;
using GuideLine.WPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace GuideLine.Core
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



        public ObservableCollection<GuideLineItem> GuideLines;
        private GuideLineItem _currentGuideLine; public GuideLineItem CurrentGuideLine
        {
            get { return _currentGuideLine; }
            set { _currentGuideLine = value; OnPropertyChanged(); }
        }
        public GuideLineItem PreviousGuideLine
        {
            get
            {
                int currentIndex = GuideLines.IndexOf(CurrentGuideLine);
                if (currentIndex > 0)
                {
                    return GuideLines[currentIndex - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GuideLineItem NextGuideLine
        {
            get
            {
                int currentIndex = GuideLines.IndexOf(CurrentGuideLine);
                if (currentIndex < GuideLines.Count - 1)
                {
                    return GuideLines[currentIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }


        public GuideLineManager()
        {
            instances.Add(this);

            GuideLines = new ObservableCollection<GuideLineItem>();

            ShowNextGuideLineStepCommand = new RelayCommand(
                command => CurrentGuideLine.ShowNextStep(),
                condition => CurrentGuideLine?.NextStep != null || NextGuideLine != null);
            ShowPreviousGuideLineStepCommand = new RelayCommand(
                command => CurrentGuideLine.ShowPreviousStep(),
                condition => CurrentGuideLine?.PreviousStep != null || PreviousGuideLine != null);

            SkipGuideLineCommand = new RelayCommand(
                command => CurrentGuideLine.StopGuideLine());

            CloseGuideLineCommand = new RelayCommand(command => StopGuideLine());
        }

        public void AddGuideLine(GuideLineItem guideLine)
        {
            GuideLines.Add(guideLine);
            guideLine.OnGuideLineCompleted += () => ShowNextGuideLine();
            guideLine.OnGuideLineRollBacked += () => ShowPreviousGuideLine();
        }


        public void StartGuideLine(string guideLineViewName)
        {
            _guideLine_View = Application.Current.MainWindow.FindChild<GuideLine_View>(guideLineViewName);
            StartGuideLine();
        }
        public void StartGuideLine(GuideLine_View guideLine_View)
        {
            _guideLine_View = guideLine_View;
            StartGuideLine();
        }
        private void StartGuideLine()
        {
            Debug.WriteLine("start " + Guid);

            _guideLine_View.DataContext = this;

            CurrentGuideLine = GuideLines.First();
            CurrentGuideLine.StartGuideLine();
        }



        public void StopGuideLine()
        {
            Debug.WriteLine("stop " + Guid);
            
            _guideLine_View.DataContext = null;
            _guideLine_View = null;

            OnGuideLineListCompleted?.Invoke();

            instances.Remove(this);
        }


        public void ShowNextGuideLine()
        {
            if (NextGuideLine != null)
            {
                CurrentGuideLine = NextGuideLine;
                CurrentGuideLine.StartGuideLine();
            }
            else
            {
                StopGuideLine();
            }
        }
        public void ShowPreviousGuideLine()
        {
            if (PreviousGuideLine != null)
            {
                CurrentGuideLine = PreviousGuideLine;
                CurrentGuideLine.StartGuideLine();
            }
            else
            {
                StopGuideLine();
            }
        }
    }
}
