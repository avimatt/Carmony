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

    public bool goingUp;
    public bool goingToPoint;
    public bool goingDown;
    public Vector3 targetLocation;
    public Quaternion targetRotation;
    public float carrySpeed;
    public Quaternion initalRotation;

    public GameObject oilPrefab;
	// Use this for initialization
	void Start () {
        isCarBottom = gameObject.GetComponentInParent<CarUserControl>().isBottomCar;
        boostTimer = Time.time;
	}
	
    public void placeOilSpill()
    {

        Vector3 newLocation = gameObject.GetComponentInChildren<Camera>().GetComponent<Transform>().position;
        newLocation.y = oilPrefab.transform.position.y;
        GameObject go = Instantiate(oilPrefab, newLocation, oilPrefab.transform.rotation) as GameObject;
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
		if (!Main.S.getRaceStarted())
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
        if (playerBInput.DPad.Left.WasPressed)
        {
            moveToNextCheckpoint();
        }
        if (playerBInput.DPad.Right.WasPressed)
        {
            placeOilSpill();
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
            startReset();
        }
    }

    public void startReset()
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
        targetLocation = newPos;
        targetRotation = newRotation;
        initalRotation = gameObject.GetComponentInParent<Transform>().rotation;

        // Move car to correct spot
        goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        gameObject.GetComponentInParent<CarController>().zeroSpeed();
    }

    public void moveToNextCheckpoint()
    {
        moveToCheckpoint(gameObject.GetComponentInParent<CarState>().currCheckpoint);
    }

    public void moveToCheckpoint(int checkpointToGo)
    {
        Vector3 newPos = new Vector3();
        Transform checkPoint;
        Quaternion newRotation;
        // Find the location and rotation of the prev checkpoint

        checkPoint = gameObject.GetComponentInParent<CarState>().checkpoints[checkpointToGo];
        newRotation = gameObject.GetComponentInParent<CarState>().checkpoints[checkpointToGo].rotation;

        newPos.x = checkPoint.position.x;
        newPos.y = checkPoint.position.y;
        newPos.z = checkPoint.position.z;
        targetLocation = newPos;
        targetRotation = newRotation;
        initalRotation = gameObject.GetComponentInParent<Transform>().rotation;

        // Move car to correct spot
        goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        gameObject.GetComponentInParent<CarController>().zeroSpeed();
    }

    //immediatly move the car to above the last checkpoint and immediatly rotate to correct direction
    public void hardReset()
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
        targetLocation = newPos;
        targetRotation = newRotation;
        initalRotation = gameObject.GetComponentInParent<Transform>().rotation;

        // Adjust angle to face correct direction
        Vector3 Angles = newRotation.eulerAngles;
        Angles.y += 90;
        newRotation = Quaternion.Euler(Angles.x, Angles.y, Angles.z);

        // Move car to correct spot
        gameObject.GetComponentInParent<Transform>().position = newPos;
        gameObject.GetComponentInParent<Transform>().rotation = newRotation;
        //this is only lowering him to ~1-5 right now. could polish this.
        gameObject.GetComponentInParent<CarController>().zeroSpeed();
    }
    
    //called in update of carController
    //moves the car, up,to the last checkpoint,then down while rotating to the correct direction
    public void moveToLocation()
    {
        if (goingUp)
        {
            //Move up to y = 10;
            gameObject.GetComponentInParent<CarController>().zeroSpeed();

            Vector3 newPos2 = gameObject.GetComponentInParent<Transform>().position;
            newPos2.y += .2f;

            gameObject.GetComponentInParent<Transform>().rotation = initalRotation;

            if (newPos2.y >= 10)
            {
                goingUp = false;
                goingToPoint = true;
                newPos2.y = 10;
                carrySpeed = (float)Vector3.Distance(gameObject.GetComponentInParent<Transform>().position,targetLocation)/75;
            }
            gameObject.GetComponentInParent<Transform>().position = newPos2;

            Vector3 newRotation = gameObject.GetComponentInParent<Transform>().rotation.eulerAngles;
            gameObject.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, newRotation.y, 0); ;
            gameObject.GetComponentInParent<Transform>().position = newPos2;



        }
        else if (goingToPoint)
        {
            
            Vector3 newPos = gameObject.GetComponentInParent<Transform>().position;
            //stop momentum and falling
            gameObject.GetComponentInParent<CarController>().zeroSpeed();
            newPos.y = 10; 

            //Chose X direction to go;
            if ((newPos.x - targetLocation.x) > 2*carrySpeed)
            {
                newPos.x -= carrySpeed;
                print("decreasing x");
            }else if ((newPos.x - targetLocation.x ) < -2*carrySpeed)
            {
                newPos.x += carrySpeed;
                print("increasing x");

            }

            //chose z direction to go
            if ((newPos.z - targetLocation.z) > 2 * carrySpeed)
            {
                newPos.z -= carrySpeed;
                print("decreasing z");

            }
            else if ((newPos.z - targetLocation.z) < -2*carrySpeed)
            {
                newPos.z += carrySpeed;
                print("increasing z");
            }

            Vector3 Angles = gameObject.GetComponentInParent<Transform>().rotation.eulerAngles;
            bool rotationDone = false;
            //Chose whether to rotate or not
            float rotateQuantity = 2;
            print(initalRotation.eulerAngles.y % 360 + " " + (targetRotation.eulerAngles.y+90) % 360);
            float initialAngle = initalRotation.eulerAngles.y % 360;
            float targetAngle = (targetRotation.eulerAngles.y + 90) % 360;
            if (targetAngle < 180 && initialAngle < targetAngle+180 && initialAngle > targetAngle)
            {

                rotateQuantity = -rotateQuantity;
            }else if (targetAngle > 180 && (initialAngle > targetAngle || initialAngle < targetAngle - 180))
            {
                rotateQuantity = -rotateQuantity;
            }
            if (Mathf.Abs(gameObject.GetComponentInParent<Transform>().rotation.eulerAngles.y % 360 - (targetRotation.eulerAngles.y + 90) % 360) > 3)
            {
                Angles.y += rotateQuantity;
                gameObject.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, Angles.y, 0);
            }
            else
            {
                //if rotation is done, stabalize
                rotationDone = true;
                Vector3 Angles2 = targetRotation.eulerAngles;
                Angles2.y += 90;
                
                gameObject.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, Angles2.y, 0);

            }

            if ((Mathf.Abs(newPos.x - targetLocation.x) <= 2*carrySpeed  && Mathf.Abs(newPos.z - targetLocation.z) <= carrySpeed*2) && rotationDone)
            {
                newPos.x = targetLocation.x;
                newPos.z = targetLocation.z;
                goingToPoint = false;
                goingDown = true;
            }
            gameObject.GetComponentInParent<Transform>().position = newPos;

        }
        else if (goingDown)
        {
            //Do not allow rotation until close to hitting the ground.
            Vector3 Angles2 = targetRotation.eulerAngles;
            Angles2.y += 90;
            gameObject.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, Angles2.y, 0);
            if (gameObject.GetComponentInParent<Transform>().position.y < 1)
            {
                goingDown = false;
            }
        }
    }

    public void startBoost()
    {
        isBoosting = true;
        boostTimer = Time.time;
    }
}
