using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModularInterface : MonoBehaviour
{
    [SerializeField] Image spriteImage;

    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] TextMeshProUGUI descriptionText;

    private void Awake()
    {
        Canvas.ForceUpdateCanvases();
    }

    public void SetDetails(Sprite sprite, string name, string description)
    {
        spriteImage.sprite = sprite; 
        nameText.SetText(name);
        descriptionText.SetText(description);
        Canvas.ForceUpdateCanvases();
    }

    public void CloseInterface()
    {
        GameManager.Instance.HUDController.CloseActiveInterface();
        Destroy(gameObject);
    }
}
