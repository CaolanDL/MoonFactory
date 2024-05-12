using System;
using UnityEngine;

public enum TutorialEvent
{
    None,
    BeginTutorial,
    BuildButtonPressed,
    PowerOverlayPressed,
    LanderInterfaceOpened,
    LanderInterfaceClosed,
    StaticDrillSelected,
    StaticDrillInterfaceOpened,
    StaticDrillInterfaceClosed,
    StaticDrillPlaced,
    StaticDrillBuilt,
    SampleAnalyserBuilt,
    SampleAnalyserSelected,
    SampleAnalyserInterfaceOpened,
    SampleAnalyserRequest,  
    ResearchComplete,
    TechTreeOpened,
    TechTreeMoved,
    HopperUnlocked,
    CrusherBuilt,
    HopperBuilt,
    MachineInterfaceOpened,
    ConveyorBuilt,
    MachineInterfaceClosed,
    HopperInterfaceClosed,
    HopperInterfaceOpened,
    HopperDropdownItemSelected
}

public enum TutorialTag
{
    None,
    StaticDrillButtonPosition,
    StaticDrillPosition,
    StaticDrillInterfacePosition, 
    LanderPosition,
    LanderInterfacePosition,
    AnalyserButtonPosition,
    AnalyserPosition,
    AnalyserInterfacePosition,
    CrusherNodePosition,
    CrusherIndicatorPosition,
    ConveyorIndicatorPosition,
    CrusherPosition,
    MachineInterfacePosition,
    HopperPosition,
    HopperInterfacePosition,
    HopperIndicatorPosition
}

public class TutorialProxy
{
    public static bool IsActive = false;
    public static Action<TutorialEvent> Action { get; set; }
    public static Action<Vector3, TutorialTag> SetPopupPosition { get; set; }
}
