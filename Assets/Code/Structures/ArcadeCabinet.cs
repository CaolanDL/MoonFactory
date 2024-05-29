using System.Collections;
using UnityEngine;
 
public class ArcadeCabinet : Structure
{
    public override void OnClicked(Vector3 mousePosition)
    {
        base.OnClicked(mousePosition);

        OpenInterfaceOnHUD(MenuData.Instance.ArcadeInterface, mousePosition);
    }
} 