using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using InControl;

public class CarCollision : MonoBehaviour {

    float lastCollisionVibrate;
    public AudioClip crashClip;
    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
	
	}
    

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.layer == 0)
        {
            coll.gameObject.GetComponentInParent<CarState>().perfectRace = false;
            coll.gameObject.gameObject.GetComponentInParent<CarState>().perfectLap = false;
            coll.gameObject.gameObject.GetComponentInParent<CarState>().perfectCheckpoint = false;

            printCollisionData(coll);
            StartCoroutine("vibrateOnCollision", coll);

        }
    }

    IEnumerator vibrateOnCollision(Collision coll)
    {
        if (Time.time - lastCollisionVibrate > 1)
        {
            lastCollisionVibrate = Time.time;
            int first = coll.gameObject.gameObject.GetComponentInParent<CarUserControl>().first;
            int second = coll.gameObject.gameObject.GetComponentInParent<CarUserControl>().second;
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(1f, 1f);
            playerBInput.Vibrate(1f, 1f);
            gameObject.GetComponent<AudioSource>().clip = crashClip;
            gameObject.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(.5f);
            playerAInput.Vibrate(0f, 0f);
            playerBInput.Vibrate(0f, 0f);
        }


    }

    void printCollisionData(Collision coll)
    {
        bool isBottomCar = coll.gameObject.gameObject.GetComponentInParent<CarUserControl>().isBottomCar;
        string carStr = isBottomCar ? "Bottom Car" : "Top Car";
        float xPos = coll.gameObject.gameObject.GetComponentInParent<Transform>().position.x;
        float zPos = coll.gameObject.gameObject.GetComponentInParent<Transform>().position.z;
        print(carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        if (!isBottomCar)
            Logger.S.writeFile(true,carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        else
            Logger.S.writeFile(false, carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());

    }
}
