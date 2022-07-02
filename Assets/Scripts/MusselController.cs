using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusselController : MonoBehaviour
{
    public Transform upShell;
    public Transform downShell;



    public float maxAngle = 30;

    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    float timeSinceLastLeft;
    float timeSinceLastRight;

    float thrust = 0;

    void Update()
    {
        KeyCode leftKey = KeyCode.A;
        KeyCode rightKey = KeyCode.D;

        float targetLeft = Input.GetKey(leftKey) ? 1 : 0;
        float targetRight = Input.GetKey(rightKey) ? 1 : 0;

        float time = Time.time;

        if (Input.GetKeyDown(leftKey))
            timeSinceLastLeft = time;

        if (Input.GetKeyDown(rightKey))
            timeSinceLastRight = time;



        if (time - timeSinceLastLeft < 0.5f &&
            time - timeSinceLastRight < 0.5f)
            thrust = 1;

        thrust -= Time.deltaTime;
        thrust = Mathf.Clamp01(thrust);

        upShell.localEulerAngles = new Vector3(-maxAngle * (1 - targetLeft), 0, 0);
        downShell.localEulerAngles = new Vector3(maxAngle * (1 - targetRight), 0, 0);

        rb.AddForce(-transform.forward * 10f * thrust, ForceMode.Acceleration);
    }
}
