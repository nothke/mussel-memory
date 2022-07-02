using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusselController : MonoBehaviour
{
    public Transform upShell;
    public Transform downShell;

    public const float maxAngle = 30;

    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    float thrust = 0;
    public float thrustMult = 1;
    float smooth = 0.1f;
    public float torqueMult = 1.0f;

    public Shell left;
    public Shell right;

    [System.Serializable]
    public class Shell
    {
        public Rigidbody rb;

        public KeyCode thrustKey;
        public float side = 1;

        public Transform t;
        public float timeSinceLast;

        public Transform forcePoint;

        public float open = 0;
        public float velo = 0;

        public bool thrusting = false;
        public float thrust;

        const float smooth = 0.1f;

        public void Update()
        {
            float time = Time.time;

            float target = Input.GetKey(thrustKey) ? 1 : 0;

            if (Input.GetKeyDown(thrustKey))
                timeSinceLast = time;

            thrusting = time - timeSinceLast < 0.3f && Input.GetKey(thrustKey);
            thrust = thrusting ? 1 : 0;

            open = Mathf.SmoothDamp(open, target, ref velo, smooth);

            t.localEulerAngles = new Vector3(maxAngle * (1 - open) * side, 0, 0);
        }
    }




    void Update()
    {
        left.Update();
        right.Update();

        if (left.thrusting && right.thrusting)
            thrust = 1;

        thrust -= Time.deltaTime;
        thrust = Mathf.Clamp01(thrust);


        rb.AddRelativeTorque(
            Vector3.right * (left.thrust - right.thrust)
            * torqueMult, ForceMode.Acceleration);

        rb.AddForce(transform.forward * 10f * thrust * thrustMult, ForceMode.Acceleration);
    }
}
