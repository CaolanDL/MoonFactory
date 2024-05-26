using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    [SerializeField] TMP_Text NameText;
    [SerializeField] Image Sprite;

    float FadeDuration = 1.0f;
    float AliveDuration = 5.0f; 
    float _fadeTime = 1.0f; 
    float _aliveTime = 1.0f;

    PopupManager popupManager;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(PopupManager parentManager)
    {
        popupManager = parentManager; 
    }

    public void SetDetails(string name, Sprite sprite)
    {
        NameText.text = name;
        Sprite.sprite = sprite;
    }

    private void Update()
    { 
        if(_aliveTime >  0)
        {
            _aliveTime -= Time.deltaTime / AliveDuration;
            return;
        } 

        _fadeTime -= Time.deltaTime / FadeDuration;
        canvasGroup.alpha = _fadeTime;
        if(_fadeTime < 0f)
        {
            Destroy(gameObject);
        }
    }
}
