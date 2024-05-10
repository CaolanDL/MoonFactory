using System;
using UnityEngine;

public enum TutorialEvent
{
    None,
    BeginTutorial,
    BuildButtonPressed,
    PowerOverlayPressed,
    StaticDrillSelected,
    StaticDrillInterfaceOpened,
    SampleAnalyserSelected,
    SampleAnalyserInterfaceOpened,
    LanderInterfaceOpened,
    TechTreeOpened
}

public enum TutorialTag
{
    SelectStaticDrill,
    OpenInterface
}

public class TutorialProxy
{  
    public static Action<TutorialEvent> Action { get; set; }
    public static Action<Vector3, TutorialTag> SetPopupPosition { get; set; }
}
