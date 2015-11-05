using UnityEngine;
using System.Collections;

public class CarWheel : MonoBehaviour
{
    public ParticleSystem skidParticles;
    public bool skidding { get; private set; }

    private WheelCollider m_WheelCollider;

    private void Start()
    {
        skidParticles.Stop();
        m_WheelCollider = GetComponent<WheelCollider>();
    }

    public void EmitTyreSmoke()
    {
        skidParticles.transform.position = transform.position - transform.up*m_WheelCollider.radius;
        skidParticles.Emit(1);
    }
}
