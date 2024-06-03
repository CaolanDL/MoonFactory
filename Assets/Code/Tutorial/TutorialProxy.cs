using System;
using Unity.Mathematics;
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

public static class TutorialProxy
{
    public static TutorialSequencer TutorialSequencer;
    public static bool IsActive = false;
    public static Action<TutorialEvent> Action;
    public static Action<Vector3, TutorialTag> SetPopupPosition;
    public static bool IsBuildingAllowed(StructureData structureData)
    {
        if(!IsActive) return true;
        if(TutorialSequencer == null) return true; 
        return TutorialSequencer.IsBuildingAllowed(structureData);
    }
    public static bool IsPlacementCorrect(int2 pos, sbyte rot)
    {
        if (!IsActive) return true;
        if (TutorialSequencer == null) return true;
        return TutorialSequencer.IsPlacementCorrect(pos, rot);
    }
}
