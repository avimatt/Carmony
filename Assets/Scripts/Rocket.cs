using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rocket : MonoBehaviour {
    public Rigidbody rocketRigid;
    public float rocketSpeed = 0.0f;

    public GameObject targetCar;

    // Array of the RocketStops in the track
    public List<Transform> rocketStops;
    // currRocketStop is an index into the rocketStops array, indicating
    // which one the rocket has just passed.
    public int currRocketStop = 0;

    //Is the bomb exploding. set when within proximity of target
    bool isExploding;

	// Use this for initialization
	void Start () {
        this.setRocketStops();
        rocketRigid = GetComponent<Rigidbody>();
        this.SetRocketStopTrajectory(getNextRocketStop());

    }

    void FixedUpdate()
    {
        //set the angle of the rocket
        float angle = Mathf.Atan2(rocketRigid.velocity.normalized.z, rocketRigid.velocity.normalized.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(90,90 - angle,0);

        // If the rocket's current stop is equal to it's target's current stop
        // then that means the target is close enough, so set the trajectory towards
        // the target instead of another rocketstop
        int targetCheck = targetCar.GetComponent<CarState>().currCheckpoint - 1;
        if (targetCheck < 0)
            targetCheck = 0;
        if (this.currRocketStop == targetCheck)
        {
            this.SetRocketTrajectory(targetCar.transform);
            if (Vector3.Distance(transform.position,targetCar.transform.position) < 3 && !isExploding)
            {
                StartCoroutine("blowBomb");
            }
        }
    }

    IEnumerator blowBomb()
    {
        isExploding = true;
        MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = false;
        }
        targetCar.GetComponent<UserInteraction>().explosion.gameObject.SetActive(true);
        targetCar.GetComponent<ArcadeVehicle>().zeroSpeed();
        targetCar.GetComponent<UserInteraction>().startBombRaiseCar();
        AudioSource explosionSound = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
        explosionSound.Play();

        yield return new WaitForSeconds(2);
        targetCar.GetComponent<UserInteraction>().explosion.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    // Populate the rocketStops array
    void setRocketStops()
    {
        // Populate the array of rocketStops
        foreach (Transform child in Main.S.Map.GetComponent<Map>().rocketStopSystem.transform)
        {
            rocketStops.Add(child);
        }
        print(rocketStops.Count);
        // Set first rocketstop to the first one for now
        this.currRocketStop = 0;
    }

    // Gets the next index into the RocketStops array, and wraps around
    public int getNextRocketStop()
    {
        if (currRocketStop + 1 < rocketStops.Count)
            return currRocketStop + 1;
        else
            return 0;
    }

    // Set the currRocketStop to the next stop
    public void incrRocketStop()
    {
        this.currRocketStop = getNextRocketStop();
    }

    // Set the rocket's current stop to the input, which should
    // be the same as the rocketstop of the car that spawned the rocket
    // Also set the start position to that of the car that spawned it
    // Set the target car for this rocket
    public void InitializeRocket(int rocketStopIndex, Vector3 startPos, GameObject target) {
        this.currRocketStop = rocketStopIndex;
        startPos.y = 2.0f; // ensure we start at this height
        this.transform.position = startPos;
        this.targetCar = target;
    }

    // Set the rocket's trajectory to the target rocketstop
    public void SetRocketStopTrajectory(int targetStop)
    {
        SetRocketTrajectory(rocketStops[targetStop]);
    }

    // Set the rocket's trajectory to the target.
    void SetRocketTrajectory(Transform target)
    {
        // Get the rocket's position
        Vector3 rocketPos = this.transform.position;
        // Get the target's position
        Vector3 targetPos = target.position;
        // Set the target's y position (height) to be the same as the rocket (we don't want
        // the rocket to change height as it moves)
        targetPos.y = rocketPos.y;

        var heading = targetPos - rocketPos;
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction header

        // set the Rocket's velocity
        var rocketvel = rocketRigid.velocity;
        rocketvel = direction * rocketSpeed;
        rocketRigid.velocity = rocketvel;
    }
}
