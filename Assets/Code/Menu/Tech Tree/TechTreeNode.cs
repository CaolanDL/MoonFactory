using System;
using TMPro;
using UnityEngine; 
using UnityEngine.UI;

public class TechTreeNode : MonoBehaviour
{
    TechTreeController techTree;

    [Space]
    [SerializeField] public StructureData Tech;
    [SerializeField] public int Cost;
    [SerializeField] public TechTreeConnection Connection;

    [Space(24)]
    [SerializeField] Image background;
    [SerializeField] Button button;
    [SerializeField] Image techSprite;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;

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
    }

    void SetDetails()
    {
        if(Tech == null) { return; }

        techSprite.sprite = Tech.sprite;
        nameText.text = Tech.screenname;
        costText.text = Cost.ToString();
    }

    public void TryUnlock()
    { 
        if(state != State.Available) { return; }

        var scienceManager = GameManager.Instance.ScienceManager;

        if(scienceManager.SciencePoints >= Cost)
        {
            scienceManager.RemovePoints(Cost); 
            ChangeState(State.Unlocked);
            Tech.Unlock();
            Connection.Unlock();
        }
    } 

    public void ChangeState(State state)
    {
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
}
