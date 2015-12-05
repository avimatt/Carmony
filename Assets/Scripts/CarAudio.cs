using UnityEngine;
using System.Collections;

public class CarAudio : MonoBehaviour {
    private AudioSource m_audioSource;
    public AudioClip audioClip;
    private ArcadeVehicle m_car;
	// Use this for initialization
	void Start () {

        m_audioSource.clip = audioClip;
        m_audioSource.Play();
	}
    void Awake()
    {
        m_car = GetComponent<ArcadeVehicle>();
        m_audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        m_audioSource.volume = m_car.getSpeed() / 100;
    }
}
