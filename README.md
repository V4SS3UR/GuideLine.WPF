# GuideLine: Interactive Tutorial Overlay for WPF Applications

GuideLine is a powerful library for C# WPF applications that allows developers to create interactive, step-by-step tutorials. Enhance user experience by guiding users through your application with highlighted controls and explanatory dialogs.

<p align="center">
        <img src="https://github.com/user-attachments/assets/7943cfa7-eec3-4306-98f0-e3cc194cb77b" width="80%">      
</p>

## Features

- **Highlight Controls**: Draw attention to specific controls while dimming the rest of the application.
- **Explanatory Dialogs**: Provide contextual information and instructions for each step of the tutorial.
- **Navigation**: Easily navigate through tutorial steps with options to go to the previous step, next step, or skip the tutorial entirely.
- **Customization**: Customize the appearance and behavior of the tutorial overlays and dialogs to fit your application’s style.

## Getting Started

### Prerequisites

- .NET Framework or .NET Core with WPF support.
- Visual Studio or any compatible IDE for WPF development.

### Installation

To install GuideLine, you can download the latest release from the LINK page or clone the repository directly:
Add the GuideLine.WPF project to your solution.
Reference the GuideLine.WPF project in your main application project.

### Usage


#### 1.Add the GuideLine View to Your XAML root view

Include the GuideLine_View control in your main window or the appropriate user control.
```xml
<Window x:Class="YourNamespace.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:guideline="clr-namespace:GuideLine.WPF.View;assembly=GuideLine.WPF"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <!-- Your existing UI elements -->

        <guideline:GuideLine_View x:Name="MainGuideline" Grid.ColumnSpan="2"
                                  PreviewKeyDown="MainGuideline_PreviewKeyDown"
                                  HighlightCornerRadius="10" HighlightMargin="10"
                                  AnimateDialog="True" AnimationDuration="0:0:0.3">
            <!-- Optional custom template can be define here 
            <guideline:GuideLine_View.Resources>
                <Style TargetType="guideline:GuideLine_Dialog_View">
                    <Setter Property="Template">
                       
                    </Setter>
                </Style>
            </guideline:GuideLine_View.Resources>  -->
        </guideline:GuideLine_View>
    </Grid>    
</Window>
```

Code behind :
```cs
private void MainGuideline_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
{
    var guideLine_View = (GuideLine_View)sender;
    GuideLine_Dialog_View guideLine_Dialog_View = guideLine_View.FindChild<GuideLine_Dialog_View>();

    if (e.Key == System.Windows.Input.Key.Left)
    {
        var guideLineManager = guideLine_View.DataContext as GuideLineManager;
        guideLineManager.CurrentGuideLine.ShowPreviousStep();
        FocusManager.SetFocusedElement(guideLine_Dialog_View, this);
    }
    if (e.Key == System.Windows.Input.Key.Right)
    {
        var guideLineManager = guideLine_View.DataContext as GuideLineManager;
        guideLineManager.CurrentGuideLine.ShowNextStep();
        FocusManager.SetFocusedElement(guideLine_Dialog_View, this);
    }
}
```

#### 2.Define GuideLine Steps in the ViewModel or anywhere else

Create instances of GuideLineStep to define each step in your guideline. Each step should specify the title, message, and the UI element to highlight (UIElement object or name).
```cs
using GuideLine.WPF;

private void ShowGuide()
{
    UIElement DateDropdown = FindTheUIElement();

    GuideLineManager guideLineManager = new GuideLineManager();
    guideLineManager.AddGuideLine(new GuideLineItem(new GuideLineStep[]
    {
        new GuideLineStep(
            title: "Change Date",
            message: "You can change the calendar year by selecting a year from the dropdown list.",
            uiElement: DateDropdown),
        new GuideLineStep(
            title: "Add Topics to Report",
            message: "To allocate hours, check or drag and drop a topic into the reporting section.",
            uiElementNames: new string[] { "subjectListPanel", "reportPanel" }),

        // Access Guide
        new GuideLineStep(
            title: "Access Guide",
            message: "Thank you for following this guide. To access it again, click on the following button.",
            uiElementName: "GuideButton"),
    }));

    guideLineManager.StartGuideLine("MainGuideline");
}
```
## Customization

#### Highlight Appearance
You can customize the highlight appearance by setting the HighlightCornerRadius and HighlightMargin properties on the GuideLine_View control.

#### Animation
Enable or disable animations using the AnimateDialog property. You can also set the duration of the animation using the AnimationDuration property.

#### Dialog template
Here is the default template : [Default GuideLine_Dialog_View template](https://github.com/V4SS3UR/GuideLine/blob/master/GuideLine/WPF/View/GuideLine_Dialog_View.xaml)

<p align="center">
        <img src="https://github.com/user-attachments/assets/92497ffe-fca8-47a7-82ae-57a095673231">      
</p>

## Contributing

Contributions are welcome! Please submit pull requests or open issues to discuss potential improvements.

---

Enhance your application’s user onboarding experience with GuideLine!
