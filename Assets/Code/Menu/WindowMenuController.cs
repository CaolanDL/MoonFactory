using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMenuController : MonoBehaviour
{
    [SerializeField] Object application;

    public void CloseMenu()
    {
        Destroy(application);
        Destroy(gameObject);
    }
}
