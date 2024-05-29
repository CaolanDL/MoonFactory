using UnityEngine;
using UnityEngine.UI;

public class BlinkIntermittent : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Image image;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
    }

    float blinkTime = 0f;
    float showTime = 1f;
    [SerializeField] float blinkDuration = 0.25f;
    [SerializeField] float showDuration = 1.2f;

    // Update is called once per frame
    void Update()
    {
        if (showTime <= 0 && blinkTime > 0)
        {
            blinkTime -= Time.deltaTime / blinkDuration;
            if (blinkTime < 0)
            {
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                if (image != null) image.enabled = true;
                showTime = 1f;
            }
        }
        if (showTime > 0 && blinkTime <= 0)
        {
            showTime -= Time.deltaTime / showDuration;
            if (showTime < 0)
            {
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                if (image != null) image.enabled = false;
                blinkTime = 1f;
            }
        }

    }
}
