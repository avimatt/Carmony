using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using InControl;

public class UserInteraction : MonoBehaviour {

    
    public bool isCarBottom;
    public bool isBoosting;
    public float boostTimer;
	public float shrinkingTimer;
    public bool isShrinking;
    public bool isNormalizingUp;
    public bool isNormalizingDown;
    public bool isGrowing;
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

        if (isShrinking)
        {
            Vector3 newSize = gameObject.transform.localScale;
            newSize.x = newSize.x  - .05f;
            newSize.y = newSize.y - .05f;
            newSize.z = newSize.z  -.05f;
            if(newSize.x <= .5)
            {
                newSize.x = .5f;
                newSize.y = .5f;
                newSize.z = .5f;
                isShrinking = false;
            }
            gameObject.transform.localScale = newSize;
            
        }
        if (isNormalizingUp)
        {
            Vector3 newSize = gameObject.transform.localScale;
            newSize.x = newSize.x + .05f;
            newSize.y = newSize.y + .05f;
            newSize.z = newSize.z + .05f;
            if (newSize.x >= 1)
            {
                newSize.x = 1f;
                newSize.y = 1f;
                newSize.z = 1f;
                isNormalizingUp = false;
            }
            gameObject.transform.localScale = newSize;

        }
        if (isGrowing)
        {
            Vector3 newSize = gameObject.transform.localScale;
            newSize.x = newSize.x + .05f;
            newSize.y = newSize.y + .05f;
            newSize.z = newSize.z + .05f;
            if (newSize.x >= 2)
            {
                newSize.x = 2f;
                newSize.y = 2f;
                newSize.z = 2f;
                isGrowing = false;
            }
            gameObject.transform.localScale = newSize;

        }
        if (isNormalizingDown)
        {
            Vector3 newSize = gameObject.transform.localScale;
            newSize.x = newSize.x - .05f;
            newSize.y = newSize.y - .05f;
            newSize.z = newSize.z - .05f;
            if (newSize.x <= 1)
            {
                newSize.x = 1f;
                newSize.y = 1f;
                newSize.z = 1f;
                isNormalizingDown = false;
            }
            gameObject.transform.localScale = newSize;

        }
        if ((playerBInput.DPad.Up.WasPressed || playerBInput.DPad.Down.WasPressed)){
			Vector3 newSize = gameObject.transform.localScale;
            if (newSize.x == 1 && (playerBInput.DPad.Up.WasPressed))
            {
                isGrowing = true;
            } else if (newSize.x == 1 && (playerBInput.DPad.Down.WasPressed)) {
                isShrinking = true;
            }
            else if (newSize.x == .5f && playerBInput.DPad.Up.WasPressed) {
                isNormalizingUp = true;
            } else if (newSize.x == 2f && playerBInput.DPad.Down.WasPressed)
            {
                isNormalizingDown = true;
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
