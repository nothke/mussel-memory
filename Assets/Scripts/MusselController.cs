using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusselController : MonoBehaviour
{
    public static MusselController e;
    void Awake() { e = this; }

    public Transform upShell;
    public Transform downShell;

    public const float maxAngle = 30;

    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    float thrust = 0;
    public float thrustMult = 1;
    public float torqueMult = 1.0f;

    public Shell left;
    public Shell right;
    public Transform meat;

    public float muscleScale = 1.0f;

    public AudioClip[] dashClips;
    public AudioClip[] openClips;

    public float dashVolume = 1.0f;

    public UnityEngine.Audio.AudioMixerGroup group;

    public ParticleSystem[] prcticles;

    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 2.0f;
    public LayerMask groundRaycastMask = -1;

    public float raycastDistance = 0.3f;

    [System.Serializable]
    public class Shell
    {
        public Rigidbody rb;

        public KeyCode thrustKey;
        public float side = 1;

        public Transform t;
        public float timeSinceLast;

        public Transform forcePoint;
        public Transform meatPoint;

        public float open = 0;
        public float velo = 0;

        public bool thrusting = false;
        public float thrust;

        const float smooth = 0.1f;




        public void Update()
        {
            float time = Time.time;

            float thrustTarget = Input.GetKey(thrustKey) ? 1 : 0;

            if (Input.GetKeyDown(thrustKey))
            {
                timeSinceLast = time;
                e.dashClips.Play2D(e.dashVolume, mixerGroup: e.group);
            }

            if (Input.GetKeyUp(thrustKey))
            {
                e.openClips.Play2D(e.dashVolume, mixerGroup: e.group);
            }



            thrusting = time - timeSinceLast < 0.3f && Input.GetKey(thrustKey);
            thrust = thrusting ? 1 : 0;

            open = Mathf.SmoothDamp(open, thrustTarget, ref velo, smooth);

            t.localEulerAngles = new Vector3(maxAngle * (1 - open) * side, 0, 0);
        }
    }

    bool IsGrounded()
    {

        Vector3 origin = transform.position;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit,
            raycastDistance, e.groundRaycastMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(origin, Vector3.down * raycastDistance, Color.green);
            return false;
        }
    }

    private void FixedUpdate()
    {
        left.Update();
        right.Update();

        if (left.thrusting && right.thrusting)
            thrust = 1;

        thrust -= Time.deltaTime;
        thrust = Mathf.Clamp01(thrust);


        float rotH = Input.GetAxis("Horizontal");

        rb.AddRelativeTorque(
            ((Vector3.right * (left.thrust - right.thrust)) +
            (Vector3.up * rotH))
            * torqueMult,
            ForceMode.Acceleration);

        rb.AddForce(transform.forward * thrust * thrustMult, ForceMode.Acceleration);
    }

    private void Update()
    {
        meat.transform.position = (left.meatPoint.position + right.meatPoint.position) * 0.5f;
        float d = Vector3.Distance(left.meatPoint.position, right.meatPoint.position) * muscleScale;
        meat.transform.localScale = new Vector3(1, d, 1);

        foreach (var prc in prcticles)
        {
            var emission = prc.emission;
            emission.rateOverTimeMultiplier = thrust * 100;
        }

        bool jompkeyPressed = Input.GetKeyDown(jumpKey);

        bool goingUp = rb.velocity.y > 0;

        if (IsGrounded() && !goingUp && jompkeyPressed)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
}
