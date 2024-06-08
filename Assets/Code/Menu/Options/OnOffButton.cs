 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class OnOffButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image OnGraphic;
    [SerializeField] Image OffGraphic;

    /// <summary>
    /// True: On | False: Off
    /// </summary>
    public bool state;

    public UnityEvent<bool> OnPressed;

    private void Awake()
    { 
        SetState(state);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetState(!state);

        OnPressed?.Invoke(state);
    }

    void SetState(bool newState)
    {
        if (newState == false) //On
        {
            OnGraphic.gameObject.SetActive(false);
            OffGraphic.gameObject.SetActive(true);
            state = false;
        }
        if (newState == true) //Off
        {
            OnGraphic.gameObject.SetActive(true);
            OffGraphic.gameObject.SetActive(false);
            state = true;
        }
    }
}
