using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlatformWarning : MonoBehaviour
{
    public void Continue()
    {
        GameManager.Instance.OpenMainMenu();
        Destroy(gameObject);
    }
}
