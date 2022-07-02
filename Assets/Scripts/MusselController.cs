using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusselController : MonoBehaviour
{
    public Transform upShell;
    public Transform downShell;

    public float maxAngle = 30;

    void Update()
    {
        float targetLeft = Input.GetKey(KeyCode.A) ? 1 : 0;
        float targetRight = Input.GetKey(KeyCode.D) ? 1 : 0;

        upShell.localEulerAngles = new Vector3(-maxAngle * (1 - targetLeft), 0, 0);
        downShell.localEulerAngles = new Vector3(maxAngle * (1 - targetRight), 0, 0);
    }
}
