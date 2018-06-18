using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuaternions : MonoBehaviour {
    public Quaternion quat;
    private void Update()
    {
        quat = transform.rotation;
    }
}
