using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using InControl;

public class UserInteraction : MonoBehaviour {

    
    public bool isCarBottom;
    public bool isBoosting;
    public float boostTimer;
	public float shrinkingTimer;
	// Use this for initialization
	void Start () {
        isCarBottom = gameObject.GetComponentInParent<CarUserControl>().isBottomCar;
        boostTimer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        // Check if car is done and if so ignore input
		if (!isCarBottom)
        {
            if (Main.S.carTopDone)
                return;
        }
        else
        {
            if (Main.S.carBottomDone)
                return;
        }


        CarUserControl userControl = gameObject.GetComponentInParent<CarUserControl>();
        // Prevent players from using reset right away
		if (Time.time < 1)
            return;
		// If car has no controllers attached to it
        if (userControl.first >= InputManager.Devices.Count)
            return;

		// Get player controller object
        var playerAInput = InputManager.Devices[userControl.first];
        var playerBInput = InputManager.Devices[userControl.second];

		if((playerAInput.LeftBumper || playerBInput.LeftBumper) && Time.time - shrinkingTimer > .25){
			Vector3 newSize = gameObject.transform.localScale;
			print ("changing size");
			if (newSize.x == 1){
				newSize.x = newSize.x/2;
				newSize.y = newSize.y/2;
				newSize.z = newSize.z/2;
				gameObject.transform.localScale = newSize;
			}else{
				newSize.x = 1;
				newSize.y = 1;
				newSize.z = 1;
				gameObject.transform.localScale = newSize;

			}
			shrinkingTimer = Time.time;
		}

		// Turn off boost
        if (isBoosting && Time.time - boostTimer > 5)
            isBoosting = false;

        bool resetting = false;
		if ((playerAInput.RightBumper || playerBInput.RightBumper) && gameObject.GetComponentInParent<CarState>().currLap != 0)
        {
            resetting = true;
        }
        if (resetting)
        {
            Vector3 newPos = new Vector3();
            Transform checkPoint;
            Quaternion newRotation;
			// Find the location and rotation of the prev checkpoint
            if (gameObject.GetComponentInParent<CarState>().currCheckpoint != 0)
            {
                checkPoint = gameObject.GetComponentInParent<CarState>().checkpoints[gameObject.GetComponentInParent<CarState>().currCheckpoint - 1];
                newRotation = gameObject.GetComponentInParent<CarState>().checkpoints[gameObject.GetComponentInParent<CarState>().currCheckpoint - 1].rotation;
            }
            else
            {
                checkPoint = gameObject.GetComponentInParent<CarState>().checkpoints[gameObject.GetComponentInParent<CarState>().checkpoints.Count - 1];
                newRotation = gameObject.GetComponentInParent<CarState>().checkpoints[gameObject.GetComponentInParent<CarState>().checkpoints.Count - 1].rotation;
            }
            newPos.x = checkPoint.position.x;
            newPos.y = checkPoint.position.y;
            newPos.z = checkPoint.position.z;

			// Adjust angle to face correct direction
            Vector3 Angles = newRotation.eulerAngles;
            Angles.y += 90;
            newRotation = Quaternion.Euler(Angles.x, Angles.y, Angles.z);
            //newRotation.y += .9f;

			// Move car to correct spot
            gameObject.GetComponentInParent<Transform>().position = newPos;
            gameObject.GetComponentInParent<Transform>().rotation = newRotation;
            //this is only lowering him to ~1-5 right now. could polish this.
            gameObject.GetComponentInParent<CarController>().zeroSpeed();
        }
    }

    public void startBoost()
    {
        isBoosting = true;
        boostTimer = Time.time;
    }
}
