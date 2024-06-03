using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class TutorialSequencer : MonoBehaviour
{
    [SerializeField] List<TutorialPopup> popupSequence;

    [SerializeField] public Material indicatorMaterial;

    [SerializeField] Mesh staticDrill;
    [SerializeField] TinyTransform staticDrill_transform;
    [SerializeField] Mesh analyser;
    [SerializeField] TinyTransform analyser_transform;
    [SerializeField] Mesh crusher;
    [SerializeField] TinyTransform crusher_transform;
    [SerializeField] Mesh hopper;
    [SerializeField] TinyTransform hopper_transform;
    [SerializeField] Mesh conveyor;
    [SerializeField] TinyTransform conveyor_transform;

    int activePopupIndex = 0;

    TutorialPopup CurrentPopup => popupSequence[activePopupIndex];

    bool RenderCrusherIndicator = false;
    bool RenderHopperIndicator = false;
    bool RenderAnalyserIndicator = false;
    bool RenderDrillIndicator = false;
    bool RenderConveyorIndicator = false;

    private void Awake()
    {
        foreach (TutorialPopup popup in popupSequence)
        {
            popup.gameObject.SetActive(false);
        }
        SubscribeEvents();
    }  
     
    private void OnEnable() => SubscribeEvents();

    private void OnDisable() => UnsubscribeEvents();

    private void OnDestroy() => UnsubscribeEvents();

    bool isSubscribed = false;
    void SubscribeEvents()
    {
        if (isSubscribed) { return; }
        isSubscribed = true;
        TutorialProxy.TutorialSequencer = this;
        TutorialProxy.IsActive = true;
        TutorialProxy.Action += GameEvent;
    }
    void UnsubscribeEvents()
    {
        if (!isSubscribed) { return; }
        isSubscribed = false;
        TutorialProxy.TutorialSequencer = null;
        TutorialProxy.IsActive = false;
        TutorialProxy.Action -= GameEvent;
    }

    private void Update() => UpdatePopopPositions(); 

    void GameEvent(TutorialEvent @event)
    {
        if (@event == TutorialEvent.BeginTutorial) { BeginTutorial(); }

        if (@event == TutorialEvent.StaticDrillSelected) { RenderDrillIndicator = true; }
        if (@event == TutorialEvent.StaticDrillBuilt) { RenderDrillIndicator = false; }
        if (@event == TutorialEvent.StaticDrillInterfaceClosed) { RenderAnalyserIndicator = true; }
        if (@event == TutorialEvent.SampleAnalyserBuilt) { RenderAnalyserIndicator = false; } 

        if (@event == TutorialEvent.HopperUnlocked) { RenderHopperIndicator = true; }
        if (@event == TutorialEvent.HopperBuilt) { RenderHopperIndicator = false; }
        if (@event == TutorialEvent.HopperInterfaceClosed) { RenderCrusherIndicator = true; } 
        if (@event == TutorialEvent.CrusherBuilt) { RenderCrusherIndicator = false; }
        if (@event == TutorialEvent.MachineInterfaceClosed) { RenderConveyorIndicator = true;  }
        if (@event == TutorialEvent.ConveyorBuilt) { RenderConveyorIndicator = false; }
    }

    public void BeginTutorial() => popupSequence[0].gameObject.SetActive(true); 

    public void FinishTutorial() => Destroy(gameObject);  
    public void SkipTutorial() => Destroy(gameObject); 

    public void AdvanceTutorial()
    {
        popupSequence[activePopupIndex].gameObject.SetActive(false);
        activePopupIndex++;
        if (activePopupIndex >= popupSequence.Count) { FinishTutorial(); return; }
        popupSequence[activePopupIndex].gameObject.SetActive(true);
    }

    public bool IsBuildingAllowed(StructureData structureData)
    {
        if (CurrentPopup.indicatedStructure == structureData) return true; 
        return false;
    } 

    internal bool IsPlacementCorrect(int2 pos, sbyte rot)
    {
        if (CurrentPopup.indicatedLocation.Equals(pos) && CurrentPopup.indicatedRotation == rot) return true;
        return false;
    }

    void UpdatePopopPositions()
    {
        //Matrix4x4 mtx;
        //Material material = indicatorMaterial;

/*        if (RenderDrillIndicator)
        {
            mtx = Matrix4x4.TRS(staticDrill_transform.position.ToVector3(), staticDrill_transform.rotation.ToQuaternion(), Vector3.one);
            Graphics.DrawMesh(staticDrill, mtx, material, 0);
        }
        if (RenderAnalyserIndicator)
        {
            mtx = Matrix4x4.TRS(analyser_transform.position.ToVector3(), analyser_transform.rotation.ToQuaternion(), Vector3.one);
            Graphics.DrawMesh(analyser, mtx, material, 0);
        }*/
        if (RenderHopperIndicator)
        {
            //mtx = Matrix4x4.TRS(hopper_transform.position.ToVector3(), hopper_transform.rotation.ToQuaternion(), Vector3.one);
            //Graphics.DrawMesh(hopper, mtx, material, 0);
            TutorialProxy.SetPopupPosition?.Invoke(hopper_transform.position.ToVector3().ToScreenPosition(), TutorialTag.HopperIndicatorPosition);
        }
        if (RenderCrusherIndicator)
        {
            //mtx = Matrix4x4.TRS(crusher_transform.position.ToVector3(), crusher_transform.rotation.ToQuaternion(), Vector3.one);
            //Graphics.DrawMesh(crusher, mtx, material, 0);
            TutorialProxy.SetPopupPosition?.Invoke(crusher_transform.position.ToVector3().ToScreenPosition(), TutorialTag.CrusherIndicatorPosition);
        }
        if (RenderConveyorIndicator)
        {
            //mtx = Matrix4x4.TRS(conveyor_transform.position.ToVector3(), crusher_transform.rotation.ToQuaternion(), Vector3.one);
            //Graphics.DrawMesh(conveyor, mtx, material, 0);
            TutorialProxy.SetPopupPosition?.Invoke(conveyor_transform.position.ToVector3().ToScreenPosition(), TutorialTag.ConveyorIndicatorPosition);
        }
    }

}
