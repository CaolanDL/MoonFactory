using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPopoutMenu : MonoBehaviour
{
    [SerializeField] List<PopOutMenu> popouts;

    bool isHiding = true;
     
    public void Toggle()
    {
        isHiding = !isHiding;
        foreach (var popout in popouts)
        {
            if(isHiding)
            {
                popout.Hide();
            }
            else
            {
                popout.Show();
            }
        }
    }
}
