using GuideLine.WPF;
using System;
using System.Collections.Generic;
using System.Windows;

namespace GuideLine.Core.Elements
{
    public class GuideLineStep
    {
        public event Action OnStepInitialized;
        public event Action OnStepCompleted;
        public event Action OnStepRolledBack;

        public RelayCommand CompleteStepCommand { get; set; }

        public List<UIElement> UiElements { get; private set; }
        public List<string> UiElementNames { get; private set; }


        public string Title { get; set; }
        public string Message { get; set; }


        public GuideLineStep(string title, string message, 
            Action OnStepInitializedAction = null,
            Action OnStepCompletedAction = null,
            Action OnStepRolledBackAction = null)
        {
            Title = title;
            Message = message;

            if (OnStepInitializedAction != null)
            {
                OnStepInitialized += OnStepInitializedAction;
            }

            if (OnStepCompletedAction != null)
            {
                OnStepCompleted += OnStepCompletedAction;
            }

            if (OnStepRolledBackAction != null)
            {
                OnStepRolledBack += OnStepRolledBackAction;
            }

            CompleteStepCommand = new RelayCommand(command => CompleteStep());
        }

        public GuideLineStep(
            string title, string message, UIElement uiElement, 
            Action OnStepInitializedAction = null, 
            Action OnStepCompletedAction = null,
            Action OnStepRolledBackAction = null) : this(title,message, OnStepInitializedAction, OnStepCompletedAction, OnStepRolledBackAction)
        {
            UiElements = new List<UIElement>() { uiElement };
        }
        public GuideLineStep(
            string title, string message, IEnumerable<UIElement> uiElements,
            Action OnStepInitializedAction = null, 
            Action OnStepCompletedAction = null,
            Action OnStepRolledBackAction = null) : this(title, message, OnStepInitializedAction, OnStepCompletedAction, OnStepRolledBackAction)
        {
            UiElements = new List<UIElement>(uiElements);
        }

        public GuideLineStep(
            string title, string message, string uiElementName,
            Action OnStepInitializedAction = null,
            Action OnStepCompletedAction = null,
            Action OnStepRolledBackAction = null) : this(title, message, OnStepInitializedAction, OnStepCompletedAction, OnStepRolledBackAction)
        {
            UiElementNames = new List<string>() { uiElementName };
        }
        public GuideLineStep(
            string title, string message, IEnumerable<string> uiElementNames,
            Action OnStepInitializedAction = null, 
            Action OnStepCompletedAction = null,
            Action OnStepRolledBackAction = null) : this(title, message, OnStepInitializedAction, OnStepCompletedAction, OnStepRolledBackAction)
        {
            UiElementNames = new List<string>(uiElementNames);
        }

        public void InitializeStep()
        {
            OnStepInitialized?.Invoke();
        }

        public void CompleteStep()
        {
            OnStepCompleted?.Invoke();
        }

        public void StepRolledBack()
        {
            OnStepRolledBack?.Invoke();
        }
    }
}
