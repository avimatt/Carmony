using UnityEngine;
using System.Collections;
using InControl;

public class CarCollision : MonoBehaviour {

    private CarState m_carstate;
    //private CarUserControl m_carUserControl;
	private ArcadeVehicle m_arcadeVehicle;
    private Transform m_transform;
    float lastCollisionVibrate;
    public AudioClip crashClip;
    public AudioSource collisonAudioSource;

	public GameObject collisionZonePrefab;

	bool m_topInCollisionZone;
	bool m_bottomInCollisionZone;

    // Use this for initialization
    void Start () {
        m_arcadeVehicle = gameObject.GetComponentInParent<ArcadeVehicle>();
        m_carstate  = gameObject.GetComponentInParent<CarState>();
        m_transform  = gameObject.GetComponentInParent<Transform>();
		m_topInCollisionZone = false;
		m_bottomInCollisionZone = false;
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

			if(!inCollisionZone(m_arcadeVehicle.isBottomCar)){
		
				var collisionZone = Instantiate(collisionZonePrefab);
				collisionZone.transform.position = gameObject.transform.position;
			}

        }else if (coll.gameObject.layer == 0)
        {
            //do nothing
        }
        else
        {
            return;
        }
        if (gameObject.GetComponentInParent<UserInteraction>().goingDown || gameObject.GetComponentInParent<UserInteraction>().goingUp || gameObject.GetComponentInParent<UserInteraction>().goingToPoint)
        {
            return;
        }
        playOnCollision();
        applyForceBack(coll);
        //Vector3 newVel = gameObject.GetComponentInParent<Rigidbody>().velocity.normalized;
        //newVel.x = -newVel.x*5;
        //newVel.z = -newVel.z*5;
        //gameObject.GetComponent<Rigidbody>().velocity = newVel;
        //StartCoroutine("vibrateOnCollision");

    }

    void applyForceBack(Collision coll)
    {
        ContactPoint[] points = coll.contacts;
        Vector3 normal = points[0].normal;
        if (coll.gameObject.tag == "Player")
        {
            normal.Scale(new Vector3(10, 10,10));
        }
        else
        {
            normal.Scale(new Vector3(1000 + 30 * m_arcadeVehicle.getSpeed(), - 10 * m_arcadeVehicle.getSpeed(), 1000 + 30 * m_arcadeVehicle.getSpeed()));
        }
        print(normal);
        gameObject.GetComponent<Rigidbody>().AddForceAtPosition(normal, points[0].point);
    }

    void playOnCollision()
    {
        if (Time.time - lastCollisionVibrate > 1)
        {
            lastCollisionVibrate = Time.time;
            collisonAudioSource.clip = crashClip;
            collisonAudioSource.Play();
        }
    }
    IEnumerator vibrateOnCollision()
    {
        if (Time.time - lastCollisionVibrate > 1)
        {
            lastCollisionVibrate = Time.time;
			int first = m_arcadeVehicle.first;
			int second = m_arcadeVehicle.second;
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
        bool isBottomCar = m_arcadeVehicle.isBottomCar;
        string carStr = isBottomCar ? "Bottom Car" : "Top Car";
        float xPos = m_transform.position.x;
        float zPos = m_transform.position.z;
        if (!isBottomCar)
            Logger.S.writeFile(true,carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        else
            Logger.S.writeFile(false, carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());

    }

	bool inCollisionZone(bool bottomCar){
		return bottomCar ? Main.S.bottomInCollisionZone : Main.S.topInCollisionZone;
	}
}
