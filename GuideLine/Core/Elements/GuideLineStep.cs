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

        public RelayCommand CompleteStepCommand { get; set; }

        public List<UIElement> UiElements { get; private set; }
        public List<string> UiElementNames { get; private set; }


        public string Title { get; set; }
        public string Message { get; set; }


        private GuideLineStep(string title, string message, Action OnStepInitializedAction = null, Action OnStepCompletedAction = null)
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

            CompleteStepCommand = new RelayCommand(command => CompleteStep());
        }

        public GuideLineStep(
            string title, string message, UIElement uiElement, 
            Action OnStepInitializedAction = null, Action OnStepCompletedAction = null) : this(title,message, OnStepInitializedAction, OnStepCompletedAction)
        {
            UiElements = new List<UIElement>() { uiElement };
        }
        public GuideLineStep(
            string title, string message, IEnumerable<UIElement> uiElements,
            Action OnStepInitializedAction = null, Action OnStepCompletedAction = null) : this(title, message, OnStepInitializedAction, OnStepCompletedAction)
        {
            UiElements = new List<UIElement>(uiElements);
        }

        public GuideLineStep(
            string title, string message, string uiElementName,
            Action OnStepInitializedAction = null, Action OnStepCompletedAction = null) : this(title, message, OnStepInitializedAction, OnStepCompletedAction)
        {
            UiElementNames = new List<string>() { uiElementName };
        }
        public GuideLineStep(
            string title, string message, IEnumerable<string> uiElementNames,
            Action OnStepInitializedAction = null, Action OnStepCompletedAction = null) : this(title, message, OnStepInitializedAction, OnStepCompletedAction)
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
    }
}
