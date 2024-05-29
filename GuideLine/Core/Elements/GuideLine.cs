using GuideLine.WPF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuideLine.Core.Elements
{
    public class GuideLine : ObservableObject
    {
        public event Action OnGuideLineCompleted;



        public List<GuideLineStep> GuideLineSteps;
        private GuideLineStep _currentStep; public GuideLineStep CurrentStep
        {
            get { return _currentStep; }
            set { _currentStep = value; OnPropertyChanged(); }
        }




        public GuideLine(IEnumerable<GuideLineStep> guideLineSteps)
        {
            GuideLineSteps = new List<GuideLineStep>();

            foreach (var step in guideLineSteps)
            {
                step.OnStepCompleted += () => ShowNextStep();
            }

            GuideLineSteps.AddRange(guideLineSteps);
        }


        public void StartGuideLine()
        {
            CurrentStep = GuideLineSteps.First();
        }
        public void StopGuideLine()
        {
            foreach (var step in GuideLineSteps)
            {
                step.OnStepCompleted -= () => ShowNextStep();
            }

            OnGuideLineCompleted?.Invoke();
        }


        public void ShowNextStep()
        {
            int currentIndex = GuideLineSteps.IndexOf(CurrentStep);

            if (currentIndex < GuideLineSteps.Count - 1)
            {
                CurrentStep = GuideLineSteps[currentIndex + 1];
            }
            else
            {
                StopGuideLine();
            }
        }
        public void ShowPreviousStep()
        {
            int currentIndex = GuideLineSteps.IndexOf(CurrentStep);
            if (currentIndex > 0)
            {
                CurrentStep = GuideLineSteps[currentIndex - 1];
            }
            else
            {
                StopGuideLine();
            }
        }
    }
}
