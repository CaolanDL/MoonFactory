using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableArea : UIMouseHover
{ 
    Vector2 offset = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        var inputActions = GameManager.Instance.PlayerInputManager.inputActions;

        if (inputActions == null) return;

        var mouseScreenPos = inputActions.UIControls.Point.ReadValue<Vector2>(); 

        if (inputActions.UIControls.Click.IsPressed() && IsMouseOver)
        {
            var mouseDelta = inputActions.UIControls.PointDelta.ReadValue<Vector2>();

            transform.position += new Vector3(mouseDelta.x, mouseDelta.y, 0);
        }

        var zoom = inputActions.UIControls.Scroll.ReadValue<float>();
        if (Mathf.Abs(zoom) > 0 && IsMouseOver)
        {
            var localScreenPos = transform.parent.InverseTransformPoint(mouseScreenPos);

            var newScale = transform.localScale + (Vector3.one * zoom);

            newScale = ClampScale(newScale, 0.3f, 1.2f);

            ScaleAround(gameObject, localScreenPos, newScale); 
        }
    }

    public Vector3 ClampScale(Vector3 scale, float min, float max)
    {
        var xScale = scale.x;

        xScale = Mathf.Clamp(xScale, min, max);

        return new Vector3(xScale, xScale, xScale);
    }

    // https://forum.unity.com/threads/scale-around-point-similar-to-rotate-around.232768/
    public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.localPosition = FP;
    }
}
