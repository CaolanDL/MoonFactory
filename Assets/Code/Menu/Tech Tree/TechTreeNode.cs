using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechTreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TechTreeController techTree;

    [Space]
    [SerializeField] public StructureData Tech;
    [SerializeField] public int Cost;
    [SerializeField] public TechTreeConnection Connection;
    [SerializeField] public List<TechTreeNode> SubNodes;
    TechTreeNode parentNode; 
    bool isSubnode = false;

    [Space(24)]
    [SerializeField] Image background;
    [SerializeField] Button button;
    [SerializeField] Image techSprite;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;

    [SerializeField] GameObject ToolTipPrefab;
    GameObject tooltip;

    [NonSerialized] public State state = State.Locked;

    public enum State
    {
        Unlocked,
        Available,
        Locked
    }

    private void Awake()
    {
        techTree = GetComponentInParent<TechTreeController>();
        SetDetails();
        ChangeState(State.Locked);

        foreach(var node in SubNodes)
        {
            node.isSubnode = true;
            node.parentNode = this;
        }
    }

    void SetDetails()
    {
        if(Tech == null) { return; }

        techSprite.sprite = Tech.sprite;
        nameText.text = Tech.screenname;
        costText.text = Cost.ToString();
    }

    public void OnPressed()
    {
        if(isSubnode) { parentNode.OnPressed(); return; }
        TryUnlock();
    }

    public void TryUnlock()
    {  
        if(state != State.Available) { return; }

        var scienceManager = GameManager.Instance.ScienceManager;

        if (scienceManager.SciencePoints >= Cost)
        {
            scienceManager.RemovePoints(Cost); 
            Tech.Unlock();
            Connection?.Unlock();
            GameManager.Instance.HUDManager.UpdateConstructionMenu();

            foreach (var node in SubNodes)
            {
                node.TryUnlock();
            }

            ChangeState(State.Unlocked);

            if(TutorialProxy.IsActive && Tech.name == "Crusher")
            {
                TutorialProxy.Action?.Invoke(TutorialEvent.HopperUnlocked);
            }
        }
    } 

    public void ChangeState(State state)
    {
        foreach(var node in SubNodes)
        {
            node.ChangeState(state);
        }

        background.color = techTree.StateColors[state];

        if (state == State.Locked)
        {
            button.interactable = false;
        }
        else if(state == State.Available)
        {
            button.interactable = true;
        }
        else if (state == State.Unlocked)
        {
            costText.text = "Unlocked";
            button.interactable = false;
        }

        this.state = state;
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
        if (tooltip != null) return;

        tooltip = Instantiate(ToolTipPrefab, GameManager.Instance.HUDManager.transform);
        tooltip.GetComponent<StructureTooltip>().SetDetails(Tech);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tooltip);
    }
}
