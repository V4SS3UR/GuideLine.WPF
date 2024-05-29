using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GuideLine.WPF;

namespace GuideLine.Core.Elements
{
    public class GuideLineStep
    {
        public event Action OnStepCompleted;

        public RelayCommand CompleteStepCommand { get; set; }


        public ObservableCollection<UIElement> Elements { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }



        public GuideLineStep(string title, string message, IEnumerable<UIElement> elements)
        {
            Title = title;
            Message = message;
            Elements = new ObservableCollection<UIElement>(elements);

            CompleteStepCommand = new RelayCommand(command => CompleteStep());
        }

        public void CompleteStep()
        {
            OnStepCompleted?.Invoke();
        }
    }
}
