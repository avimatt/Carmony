﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using InControl;

public class CarCollision : MonoBehaviour {

    private CarState m_carstate;
    private CarUserControl m_carUserControl;
    private Transform m_transform;
    float lastCollisionVibrate;
    public AudioClip crashClip;
    public AudioSource collisonAudioSource;
    // Use this for initialization
    void Start () {
        m_carUserControl = gameObject.GetComponentInParent<CarUserControl>();
        m_carstate  = gameObject.GetComponentInParent<CarState>();
        m_transform  = gameObject.GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update () {
	
	}
    

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.layer == 8)
        {
            m_carstate.perfectRace = false;
            m_carstate.perfectLap = false;
            m_carstate.perfectCheckpoint = false;

            printCollisionData();
        }else if (coll.gameObject.layer == 0)
        {
            print("hit car");
        }
        StartCoroutine("vibrateOnCollision");

    }

    IEnumerator vibrateOnCollision()
    {
        if (Time.time - lastCollisionVibrate > 1)
        {
            lastCollisionVibrate = Time.time;
            int first = m_carUserControl.first;
            int second = m_carUserControl.second;
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(1f, 1f);
            playerBInput.Vibrate(1f, 1f);
            collisonAudioSource.clip = crashClip;
            collisonAudioSource.Play();
            yield return new WaitForSeconds(.5f);
            playerAInput.Vibrate(0f, 0f);
            playerBInput.Vibrate(0f, 0f);
        }


    }

    void printCollisionData()
    {
        bool isBottomCar = m_carUserControl.isBottomCar;
        string carStr = isBottomCar ? "Bottom Car" : "Top Car";
        float xPos = m_transform.position.x;
        float zPos = m_transform.position.z;
        print(carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        if (!isBottomCar)
            Logger.S.writeFile(true,carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        else
            Logger.S.writeFile(false, carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());

    }
}
