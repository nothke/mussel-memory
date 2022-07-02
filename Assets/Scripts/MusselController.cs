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
    public float thrustMult = 1;

    float smooth = 0.1f;

    public float torqueMult = 1.0f;

    [System.Serializable]
    public class Shell
    {
        public Transform t;
        public float timeSinceLast;
    }

    float leftOpen = 0;
    float leftVelo = 0;
    float rightOpen = 0;
    float rightVelo = 0;

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

        bool thrustingLeft = time - timeSinceLastLeft < 0.3f && Input.GetKey(leftKey);
        bool thrustingRight = time - timeSinceLastRight < 0.3f && Input.GetKey(rightKey);

        float thrustLeft = thrustingLeft ? 1 : 0;
        float thrustRight = thrustingRight ? 1 : 0;

        if (thrustingLeft && thrustingRight)
            thrust = 1;

        thrust -= Time.deltaTime;
        thrust = Mathf.Clamp01(thrust);

        leftOpen = Mathf.SmoothDamp(leftOpen, targetLeft, ref leftVelo, smooth);
        rightOpen = Mathf.SmoothDamp(rightOpen, targetRight, ref rightVelo, smooth);

        upShell.localEulerAngles = new Vector3(-maxAngle * (1 - leftOpen), 0, 0);
        downShell.localEulerAngles = new Vector3(maxAngle * (1 - rightOpen), 0, 0);

        rb.AddRelativeTorque(Vector3.right * (thrustLeft - thrustRight) * torqueMult, ForceMode.Acceleration);

        rb.AddForce(-transform.forward * 10f * thrust * thrustMult, ForceMode.Acceleration);
    }
}
