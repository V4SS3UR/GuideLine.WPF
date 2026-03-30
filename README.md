# GuideLine.WPF - Interactive Tutorial & Onboarding Overlay for WPF

> **Step-by-step walkthrough overlays for WPF applications.** Highlight UI controls, dim the background, and guide your users through your app with contextual dialogs. No third-party UI framework required.

[![NuGet](https://img.shields.io/nuget/v/GuideLine.WPF.svg)](https://www.nuget.org/packages/GuideLine.WPF/)
[![License](https://img.shields.io/badge/license-BSD--3--Clause-blue.svg)](LICENSE)

<p align="center">
        <img src="https://github.com/user-attachments/assets/7943cfa7-eec3-4306-98f0-e3cc194cb77b" width="80%">      
</p>

## Why GuideLine.WPF?

Most WPF applications ship with no built-in user onboarding. New users struggle to discover features, and support costs rise. **GuideLine.WPF** solves this by letting you add an interactive tutorial layer directly on top of your existing UI, without rewriting a single screen.

- ✅ Drop it into any existing WPF window in minutes
- ✅ Zero dependency on third-party UI frameworks
- ✅ Fully customizable look & feel
- ✅ Keyboard-accessible and animation-ready

## Features

- **🔦 Spotlight UI Elements**: Precisely highlight one or more controls (by reference or by name) while the rest of the interface is dimmed, directing the user's attention exactly where you need it.
- **💬 Contextual Onboarding Dialogs**: Each tutorial step displays a title and message in a floating dialog, giving users clear, in-context guidance without leaving the screen.
- **⏭️ Step Navigation**: Built-in Previous / Next / Skip controls let users move at their own pace. Arrow-key keyboard navigation is supported out of the box.
- **🎨 Full Customization**: Control highlight corner radius, margin, background opacity, dialog animations, and animation duration. Swap in your own dialog template to match your app's design language.

## Getting Started

### Prerequisites

- .NET Framework or .NET Core with WPF support
- Visual Studio or any compatible IDE for WPF development

### Installation

Install the [GuideLine.WPF NuGet package](https://www.nuget.org/packages/GuideLine.WPF/):

```
Install-Package GuideLine.WPF
```

## Usage

### Step 1 - Add the GuideLine overlay to your XAML window

Place `GuideLine_View` as the **topmost element** inside your window's root layout panel. It renders as a transparent overlay that activates only when a tutorial is running.

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

Add keyboard navigation support in the code-behind:

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

### Step 2 - Define tutorial steps and start the walkthrough

Create `GuideLineStep` instances to describe each step of your tutorial. You can target UI elements by direct reference or by their `x:Name`. Group them into a `GuideLineItem` and hand them to `GuideLineManager` to orchestrate the flow.

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

### Highlight Appearance

Set `HighlightCornerRadius` and `HighlightMargin` on the `GuideLine_View` control to control how the spotlight cutout looks around highlighted elements.

### Animation

Set `AnimateDialog="True"` to enable smooth dialog transitions. Control the speed with `AnimationDuration` (e.g., `"0:0:0.3"` for 300 ms).

### Custom Dialog Template

The default dialog can be fully replaced with your own `ControlTemplate` to match your application's design system. See the default implementation as a starting point:

[Default GuideLine_Dialog_View template](https://github.com/V4SS3UR/GuideLine/blob/master/GuideLine/WPF/View/GuideLine_Dialog_View.xaml)

<p align="center">
        <img src="https://github.com/user-attachments/assets/92497ffe-fca8-47a7-82ae-57a095673231">      
</p>

## Contributing

Contributions are welcome! Please open an issue to discuss your idea or submit a pull request directly.

---

Give your WPF users the onboarding experience they deserve with **GuideLine.WPF**.
