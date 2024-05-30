using GuideLine.WPF;
using System;
using System.Collections.Generic;
using System.Windows;

namespace GuideLine.Core.Elements
{
    public class GuideLineStep
    {
        public event Action OnStepCompleted;

        public RelayCommand CompleteStepCommand { get; set; }

        public List<UIElement> UiElements { get; private set; }
        public List<string> UiElementNames { get; private set; }


        public string Title { get; set; }
        public string Message { get; set; }


        private GuideLineStep(string title, string message)
        {
            Title = title;
            Message = message;
            CompleteStepCommand = new RelayCommand(command => CompleteStep());
        }

        public GuideLineStep(string title, string message, UIElement uiElement) : this(title,message)
        {
            UiElements = new List<UIElement>() { uiElement };
        }
        public GuideLineStep(string title, string message, IEnumerable<UIElement> uiElements) : this(title, message)
        {
            UiElements = new List<UIElement>(uiElements);
        }

        public GuideLineStep(string title, string message, string uiElementName) : this(title, message)
        {
            UiElementNames = new List<string>() { uiElementName };
        }
        public GuideLineStep(string title, string message, IEnumerable<string> uiElementNames) : this(title, message)
        {
            UiElementNames = new List<string>(uiElementNames);
        }

        public void CompleteStep()
        {
            OnStepCompleted?.Invoke();
        }
    }
}
