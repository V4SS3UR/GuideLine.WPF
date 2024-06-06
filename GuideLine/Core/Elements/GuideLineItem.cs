using GuideLine.WPF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuideLine.Core.Elements
{
    public class GuideLineItem : ObservableObject
    {
        public event Action OnGuideLineCompleted;
        public event Action OnGuideLineRollBacked;


        public List<GuideLineStep> GuideLineSteps;
        private GuideLineStep _currentStep; public GuideLineStep CurrentStep
        {
            get { return _currentStep; }
            set 
            { 
                _currentStep = value; 

                if(_currentStep != null)
                {
                    _currentStep.InitializeStep();
                }

                OnPropertyChanged(); 
            }
        }
        public GuideLineStep PreviousStep
        {
            get
            {
                int currentIndex = GuideLineSteps.IndexOf(CurrentStep);
                if (currentIndex > 0)
                {
                    return GuideLineSteps[currentIndex - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GuideLineStep NextStep
        {
            get
            {
                int currentIndex = GuideLineSteps.IndexOf(CurrentStep);
                if (currentIndex < GuideLineSteps.Count - 1)
                {
                    return GuideLineSteps[currentIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }


        public GuideLineItem(IEnumerable<GuideLineStep> guideLineSteps)
        {
            GuideLineSteps = new List<GuideLineStep>(guideLineSteps);
        }


        public void StartGuideLine()
        {
            CurrentStep = GuideLineSteps.First();
        }
        public void StopGuideLine()
        {
            OnGuideLineCompleted?.Invoke();
        }
        public void RollBackGuideLine()
        {
            OnGuideLineRollBacked?.Invoke();
        }


        public void ShowNextStep()
        {
            if (NextStep != null)
            {
                CurrentStep.CompleteStep();
                CurrentStep = NextStep;
            }
            else
            {
                StopGuideLine();
            }
        }
        public void ShowPreviousStep()
        {
            if (PreviousStep != null)
            {
                CurrentStep = PreviousStep;
            }
            else
            {
                RollBackGuideLine();
            }
        }
    }
}
