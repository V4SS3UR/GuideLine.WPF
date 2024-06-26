using GuideLine.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;

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
            set { _currentStep = value; OnPropertyChanged(); }
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
            var step = GuideLineSteps.First();
            step.InitializeStep();
            CurrentStep = step;
        }
        public void StopGuideLine()
        {
            if (CurrentStep != null)
            {
                CurrentStep.CompleteStep();
            }

            OnGuideLineCompleted?.Invoke();
        }
        public void RollBackGuideLine()
        {
            if (CurrentStep != null)
            {
                CurrentStep.StepRolledBack();
            }

            OnGuideLineRollBacked?.Invoke();
        }


        public void ShowNextStep()
        {
            if(CurrentStep != null)
            {
                CurrentStep.CompleteStep();
            }
            
            if (NextStep != null)
            {
                NextStep.InitializeStep();
                CurrentStep = NextStep;
            }
            else
            {
                StopGuideLine();
            }                
        }
        public void ShowPreviousStep()
        {
            if (CurrentStep != null)
            {
                CurrentStep.StepRolledBack();
            }

            if (PreviousStep != null)
            {
                PreviousStep.InitializeStep();
                CurrentStep = PreviousStep;
            }
            else
            {
                RollBackGuideLine();
            }
        }
    }
}
