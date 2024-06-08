using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public TechTreeController techTree;
     
    [SerializeField] public int Cost = 10;

    [SerializeField] public TechTreeConnection Connection;
    [SerializeField] public List<TreeNode> SubNodes;
    [NonSerialized] public TreeNode parentNode;
    [NonSerialized] public bool isSubnode = false;

    [SerializeField] public Button button; 
    [SerializeField] public Image background; 

    [NonSerialized] public State state = State.Locked;

    public enum State
    {
        Unlocked,
        Available,
        Locked
    }

    public virtual void OnPointerEnter(PointerEventData eventData) { }
    public virtual void OnPointerExit(PointerEventData eventData) { }

    private void OnEnable()
    {
        techTree = GetComponentInParent<TechTreeController>();
    }

    private void Awake()
    {
        techTree = GetComponentInParent<TechTreeController>();
        //Debug.Log(techTree);
        SetDetails();
        ChangeState(State.Locked);

        foreach (var node in SubNodes)
        {
            node.isSubnode = true;
            node.parentNode = this;
        }
    }

    public virtual void SetDetails()
    {

    }

    public virtual void OnPressed()
    {
        if (isSubnode) { parentNode.OnPressed(); return; }
        var success = TryUnlock();
        if (success && !isSubnode) AudioManager.Instance.PlaySound(AudioData.Instance.UI_ScienceNodeUnlocked);
        else;
    }

    public virtual bool TryUnlock()
    {
        if (state != State.Available) { return false; }

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

            OnUnlocked();

            return true;
        }

        return false;
    }

    public virtual void OnUnlocked()
    {

    }

    public virtual void ChangeState(State state)
    {
        foreach (var node in SubNodes)
        {
            node.ChangeState(state);
        }
         
        background.color = techTree.StateColors[state];

        if (state == State.Locked)
        {
            button.interactable = false;
        }
        else if (state == State.Available)
        {
            button.interactable = true;
        }
        else if (state == State.Unlocked)
        { 
            button.interactable = false;
        }

        this.state = state;
    }
}
