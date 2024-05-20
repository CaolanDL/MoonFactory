using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreenSequence : MonoBehaviour
{
    [SerializeField] float FadeDuration = 0.5f;
    [SerializeField] float VisibleDuration = 2f;
    [SerializeField] float HiddenDuration = 0.5f;
    [SerializeField] List<Image> screens = new();

    int screenindex = 0;

    [SerializeField] Canvas Canvas;

    private void Awake()
    {
        GameManager.Instance.SpawnMainMenuCamera();

        Canvas = gameObject.GetComponent<Canvas>(); 
        Canvas.worldCamera = GameManager.Instance._mainMenuCamera;

        if (screens.Count == 0)
        {
            ExitSplashScreen();
            return;
        }

        foreach (var screen in screens)
        {
            screen.color = new Color(1,1,1,0);
        }
    }

    State fadeState = State.fadein;

    enum State
    {
        fadein,
        fadeout,
        holdOn,
        holdOff,
    }

    float timer = 0;

    private void OnDisable()
    {
        GameManager.Instance.DestroyMainMenuCamera();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SetCurrentScreenAlpha(0);
            screenindex++;
            fadeState= State.fadein;
        }

        if(screenindex >= screens.Count)
        {
            ExitSplashScreen();
            return;
        }

        var a = GetCurrentScreenAlpha();

        if (fadeState == State.fadein)
        {
            if (a < 1f) SetCurrentScreenAlpha(a + Time.deltaTime);
            if (a >= 1f) fadeState = State.holdOn; 
        }

        if (fadeState == State.holdOn || fadeState == State.holdOff) { timer += Time.deltaTime; }
        if (fadeState == State.holdOn && timer >= VisibleDuration)
        {
            fadeState = State.fadeout;
            timer = 0;
        }

        if(fadeState == State.fadeout)
        {
            if (a > 0f) SetCurrentScreenAlpha(a - Time.deltaTime);
            if (a <= 0f) fadeState = State.holdOff;
        }

        if(fadeState == State.holdOff && timer >= HiddenDuration)
        {
            fadeState = State.fadein;
            screenindex++;
            timer = 0;
        } 
    }

    float GetCurrentScreenAlpha()
    {
        return screens[screenindex].color.a;
    }
    void SetCurrentScreenAlpha(float alpha)
    {
        screens[screenindex].color = new Color(1, 1, 1, alpha);
    }

    void ExitSplashScreen()
    {
        GameManager.Instance.DestroyMainMenuCamera();
        GameManager.Instance.OpenMainMenu();
        Destroy(gameObject);
        return;
    }
}
