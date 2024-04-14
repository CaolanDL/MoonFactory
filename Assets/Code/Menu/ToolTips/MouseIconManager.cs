using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIconManager : MonoBehaviour
{
    [SerializeField] GameObject CancelIcon;
    [SerializeField] GameObject BuildIcon;

    Icon activeIcon = Icon.None;

    public enum Icon
    {
        None,
        Cancel,
        Build
    }

    Dictionary<Icon, GameObject> MouseIcons = new();

    private void Awake()
    {
        CancelIcon.SetActive(false);
        MouseIcons.Add(Icon.Cancel, CancelIcon);
        MouseIcons.Add(Icon.Build, BuildIcon);

        DisableAllIcons();
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    void DisableAllIcons()
    {
        foreach (var kvp in MouseIcons)
        {
            MouseIcons[kvp.Key].SetActive(false);
        }
    }

    public void SetActiveIcon(Icon icon) 
    { 
        if(activeIcon == icon) return;

        DisableAllIcons();

        activeIcon = icon;

        if (icon == Icon.None) return;

        MouseIcons[icon].SetActive(true); 
    }
}
