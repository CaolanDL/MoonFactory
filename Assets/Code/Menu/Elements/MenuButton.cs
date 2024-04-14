using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    [SerializeField] UnityEvent OnPressed = new UnityEvent();

    public void WhenPressed()
    {
        OnPressed.Invoke();
    }
}
