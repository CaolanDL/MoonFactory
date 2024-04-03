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

    public virtual void Init(Entity entity, Vector3 screenPosition) { }

    public virtual void UpdateUI() { }
     
    public void SetDetails(Sprite sprite, string name, string description)
    {
        spriteImage.sprite = sprite; 
        nameText.SetText(name);
        descriptionText.SetText(description);
        Canvas.ForceUpdateCanvases();
    }

    public void SetDetails(string name, string description)
    { 
        nameText.SetText(name);
        descriptionText.SetText(description);
        Canvas.ForceUpdateCanvases();
    }

    public void SetName(string name)
    {
        nameText.SetText(name);  
    }

    public void SetSprite(Sprite sprite)
    {
        spriteImage.sprite = sprite; 
    }

    public void CloseInterface()
    {
        OnCloseInterface();
        GameManager.Instance.HUDController.CloseInterface(); 
    }

    public virtual void OnCloseInterface()
    {

    }
}
