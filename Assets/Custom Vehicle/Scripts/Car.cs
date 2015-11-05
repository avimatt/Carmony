using UnityEngine;
using System.Collections;
using Trails;
using InControl;

public enum CarTraction { AllWheels, RearWheels, FrontWheels };

public class Car : MonoBehaviour
{
    public bool isControllable = true;

    public CarTraction tractionType = CarTraction.AllWheels;
    public CarTraction brakeType = CarTraction.RearWheels;

    public float maxMpeed = 10f;
    public float accelerationPower = 0.2f;
    public float slipSideLimit = 0.15f;
    public float slipFwdLimit = 0.2f;

    public float steer = 0f;
    public float steerForce = 2f;

    public float steerMinAngle = 20;
    public float steerMaxAngle = 45;

    public float brakeMaxForce = 2000f;
    public float brakeLerp = 0.1f;

    public Transform centerOfMass;

    public Transform rearTireArmsLookat;
    public Transform frontTireArmsLookat;

    public AudioSource audioTireSkid;
    public AudioSource[] audioBodyCollisions;

    private Rigidbody m_rigidBody;
    private AudioSource m_AudioSource;

    [SerializeField]
    private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [SerializeField]
    private GameObject[] m_WheelMeshes = new GameObject[4];
    [SerializeField]
    private GameObject[] m_TireAxes = new GameObject[4];
    [SerializeField]
    private GameObject[] m_TireArms = new GameObject[4];
    [SerializeField]
    private Transform[] m_TireSpringArms = new Transform[4];
    [SerializeField]
    private Transform[] m_TireSpring = new Transform[4];
    [SerializeField]
    private Transform[] m_LookatTireSpring = new Transform[4];

    private CarWheel[] m_WheelEffects = new CarWheel[4];
    private TrailEmitter[] m_WheelTrailemitter = new TrailEmitter[4];
    private float m_acceleration = 0f;
    private float m_motorTorque = 0f;
    private bool m_brake = false;

    private float m_Speed = 0f;
    private float m_BrakeForce = 0f;

    private float m_pitch = 1.5f;

    public float GetSpeed()
    {
        return m_Speed;
    }

    void OnCollisionEnter(Collision other)
    {
        int i = Random.Range(0,audioBodyCollisions.Length);
        audioBodyCollisions[i].pitch = m_pitch;
        audioBodyCollisions[i].Play();
    }

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.centerOfMass = centerOfMass.localPosition;

        m_AudioSource = GetComponent<AudioSource>();

        for (int i = 0; i < 4; i++)
        {
            m_WheelEffects[i] = m_WheelColliders[i].GetComponent<CarWheel>();
            m_WheelTrailemitter[i] = m_WheelColliders[i].GetComponent<TrailEmitter>();
        }
    }
    
    public void StopSounds()
    {
        if (m_AudioSource == null)
            m_AudioSource = GetComponent<AudioSource>();

        m_AudioSource.Stop();
        audioTireSkid.Stop();
    }

    public void StartSounds()
    {
        if (m_AudioSource == null)
            m_AudioSource = GetComponent<AudioSource>();

        m_AudioSource.Play();
    }

    void Update()
    {
        UpdateWheelMeshes();

        float rpm = Mathf.Abs(m_WheelColliders[2].rpm) / 60;

        float inn = 2f / 100;
        m_pitch = 1 + (inn * ((rpm * 100) / 15.5f));
        //Debug.Log("rpm " + rpm);

        if (m_AudioSource.pitch < m_pitch)
            m_AudioSource.pitch = m_pitch;
        else
            m_AudioSource.pitch = Mathf.Lerp(m_AudioSource.pitch, m_pitch, 2f * Time.deltaTime);

        if (m_brake)
        {
            m_BrakeForce = Mathf.Lerp(m_BrakeForce, brakeMaxForce, brakeLerp * Time.deltaTime);
            Brake(m_BrakeForce);
            // Debug.Log("m_BrakeForce " + m_BrakeForce);
            //Debug.Log("GetSpeed() " + GetSpeed());

            if (rpm < 1f || GetSpeed() < 4f)
            {
                m_motorTorque = 0f;
            }
        }
    }

    void FixedUpdate()
    {
		var playerA = InputManager.Devices [0];
        float steering = playerA.LeftStickX;
		m_acceleration = playerA.RightTrigger + (-1 * playerA.LeftTrigger);

        /*if (Input.GetButton("Fire1"))
            m_acceleration = 1f;
        else if (Input.GetButton("Fire2"))
            m_acceleration = -1f;

        if (m_acceleration < 0)
            m_acceleration = -0.5f;*/

        if (isControllable)
        {

            Accelerate(m_acceleration);
            Steer(steering);

            /*if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire3"))
            {
                m_brake = true;
            }
            if (Input.GetButtonUp("Jump") || Input.GetButtonUp("Fire3"))
            {
                m_brake = false;
                Brake(0);
                m_BrakeForce = 0f;
            }*/
        }

        Vector3 locVel = transform.InverseTransformDirection(m_rigidBody.velocity);
        m_Speed = locVel.z;

        float spd = Mathf.Abs(m_Speed);

        

        CheckForWheelSpin();
    }

    // checks if the wheels are spinning and is so does three things
    // 1) emits particles
    // 2) plays tiure skidding sounds
    // 3) leaves skidmarks on the ground
    // these effects are controlled through the WheelEffects class
    private void CheckForWheelSpin()
    {
        bool playAudioSkid = false;

        // loop through all wheels
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            m_WheelColliders[i].GetGroundHit(out wheelHit);

            float sideSlip = Mathf.Abs(wheelHit.sidewaysSlip);

            // is the tire slipping above the given threshhold
            if ((Mathf.Abs(wheelHit.forwardSlip) >= slipFwdLimit && m_brake) || sideSlip >= slipSideLimit)
            {
                m_WheelEffects[i].EmitTyreSmoke();
                m_WheelTrailemitter[i].NewTrail(m_WheelMeshes[i].transform, m_WheelColliders[i].radius);

                playAudioSkid = true;

                continue;
            }

            // end the trail generation
            m_WheelTrailemitter[i].EndTrail();
        }

        if (playAudioSkid)
            audioTireSkid.Play();
        else
            audioTireSkid.Stop();
    }

    private void UpdateWheelMeshes()
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion rotCol;
            Vector3 posCol;
            m_WheelColliders[i].GetWorldPose(out posCol, out rotCol);

            Vector3 pos = new Vector3(posCol.x, Mathf.Lerp(m_WheelMeshes[i].transform.position.y, posCol.y, 20f * Time.deltaTime), posCol.z);
            
            // Set wheel position
            m_WheelMeshes[i].transform.position = pos;
            m_TireAxes[i].transform.position = pos;

            // Rotation
            m_WheelMeshes[i].transform.rotation = rotCol;

            //float rot = m_WheelColliders[i].rpm / 60 * 360 * Time.deltaTime;
        }

        ////// Arms
        m_TireArms[0].transform.LookAt(frontTireArmsLookat);
        m_TireArms[1].transform.LookAt(frontTireArmsLookat);
        m_TireArms[2].transform.LookAt(rearTireArmsLookat);
        m_TireArms[3].transform.LookAt(rearTireArmsLookat);

        ///// Springs and spring arms
        for (int i = 0; i < 4; i++)
        {
            m_TireSpringArms[i].transform.LookAt(m_LookatTireSpring[i]);

            float distance = Vector3.Distance(m_TireSpringArms[i].transform.position, m_LookatTireSpring[i].transform.position);
            m_TireSpring[i].localScale = new Vector3(m_TireSpring[i].localScale.x, m_TireSpring[i].localScale.y, distance * 3f);
        }
    }

    public void Steer(float steering)
    {
        float speedSteerAngle = steerMaxAngle;

        if (Mathf.Abs(GetSpeed()) > 0.1f)
        {
            speedSteerAngle = steerMaxAngle / (GetSpeed() / 5);

            if (speedSteerAngle < steerMinAngle)
                speedSteerAngle = steerMinAngle;

            if (speedSteerAngle > steerMaxAngle)
                speedSteerAngle = steerMaxAngle;
        }

        //Debug.Log("speedSteerAngle " + speedSteerAngle);

        // Steer
        if (Mathf.Abs(steering) > 0)
        {
            steer = Mathf.Lerp(steer, steering * speedSteerAngle, steerForce);
        }
        // Steer back to normal
        else if (Mathf.Abs(steer) > 0.3)
        {
            steer = Mathf.Lerp(steer, 0, steerForce);
        }
        else
            steer = 0;

        m_WheelColliders[0].steerAngle = steer;
        m_WheelColliders[1].steerAngle = steer;
    }

    public void Brake(float brakeTorque)
    {
        switch (brakeType)
        {
            default:
            case CarTraction.AllWheels:
            {
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].brakeTorque = brakeTorque;
                }
                break;
            }

            case CarTraction.FrontWheels:
            {
                m_WheelColliders[0].brakeTorque = brakeTorque;
                m_WheelColliders[1].brakeTorque = brakeTorque;
                break;
            }

            case CarTraction.RearWheels:
            {
                m_WheelColliders[2].brakeTorque = brakeTorque;
                m_WheelColliders[3].brakeTorque = brakeTorque;
                break;
            }
        }
    }

    public void Accelerate(float acceleration)
    {
        m_motorTorque = Mathf.Lerp(m_motorTorque, acceleration * maxMpeed, accelerationPower);

        //Debug.Log("m_motorTorque " + m_motorTorque);

        switch (tractionType)
        {
            default:
            case CarTraction.AllWheels:
            {
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].motorTorque = m_motorTorque;
                }
                break;
            }

            case CarTraction.FrontWheels:
            {
                m_WheelColliders[0].motorTorque = m_motorTorque;
                m_WheelColliders[1].motorTorque = m_motorTorque;
                break;
            }

            case CarTraction.RearWheels:
            {
                m_WheelColliders[2].motorTorque = m_motorTorque;
                m_WheelColliders[3].motorTorque = m_motorTorque;
                break;
            }
        }
    }
}
