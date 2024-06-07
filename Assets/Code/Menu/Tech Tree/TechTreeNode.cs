using TMPro; 
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechTreeNode : TreeNode
{ 
    [Space]
    [SerializeField] public StructureData Tech;
    [Space(24)]

    [SerializeField] Image techSprite;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;

    [SerializeField] GameObject ToolTipPrefab;
    GameObject tooltip;
/*

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
    }*/

    public override void SetDetails()
    {
        if(Tech == null) { return; }

        techSprite.sprite = Tech.sprite;
        nameText.text = Tech.screenname;
        costText.text = Cost.ToString();
    }

/*    public override void OnPressed()
    {
        if(isSubnode) { parentNode.OnPressed(); return; }
        var success = TryUnlock();
        if (success && !isSubnode) AudioManager.Instance.PlaySound(AudioData.Instance.UI_ScienceNodeUnlocked);
        else;// Play failed to unlock sound 
    }*/
/*
    public override bool TryUnlock()
    {  
        if(state != State.Available) { return false; }

        var scienceManager = GameManager.Instance.ScienceManager;

        if (scienceManager.SciencePoints >= Cost)
        {
            scienceManager.RemovePoints(Cost); 

            Connection?.Unlock();
            

            foreach (var node in SubNodes)
            {
                node.TryUnlock();
            }

            ChangeState(State.Unlocked); 
            return true;    
        }

        return false;
    }*/

    public override void OnUnlocked()
    {
        Tech.Unlock();
        GameManager.Instance.HUDManager.UpdateConstructionMenu();

        if (TutorialProxy.IsActive && Tech.name == "Crusher")
        {
            TutorialProxy.Action?.Invoke(TutorialEvent.HopperUnlocked);
        }

        costText.text = "Unlocked";
    }

/*    public override void ChangeState(State state)
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
    }*/

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null) return;

        tooltip = Instantiate(ToolTipPrefab, GameManager.Instance.HUDManager.transform);
        tooltip.GetComponent<StructureTooltip>().SetDetails(Tech);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tooltip);
    }
}