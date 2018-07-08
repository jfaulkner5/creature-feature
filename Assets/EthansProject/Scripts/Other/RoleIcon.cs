using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleIcon : MonoBehaviour {

    public GameObject icon1, icon2;

    public void SwitchIcons()
    {
        if (icon1.activeInHierarchy)
        {
            icon1.SetActive(false);
            icon2.SetActive(true);
        }
        else if (icon2.activeInHierarchy)
        {
            icon2.SetActive(false);
            icon1.SetActive(true);
        }
    }

}
