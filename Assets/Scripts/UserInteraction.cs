using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using InControl;

public class UserInteraction : MonoBehaviour {

    private CarController m_car;
    private CarUserControl m_carUser;
    private CarState m_carstate;
    private Transform m_transform;
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
    public float carryHeight;
    public Quaternion initalRotation;

    public GameObject oilPrefab;
    public Camera backCamera;
    public Camera frontCamera;

    public GameObject rocketPrefab;

    public bool quickStart;

    public ParticleSystem explosion;

    void Awake()
    {
        m_carUser = gameObject.GetComponentInParent<CarUserControl>();
        m_car = gameObject.GetComponentInParent<CarController>();
        m_carstate = gameObject.GetComponentInParent<CarState>();
        m_transform = gameObject.GetComponentInParent<Transform>();


    }
    // Use this for initialization
    void Start () {
        isCarBottom = m_carUser.isBottomCar;
        boostTimer = Time.time;
        quickStart = true;
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
        // Prevent players from using reset right away
		if (!Main.S.getRaceStarted() && !Main.S.practicing)
            return;
		// If car has no controllers attached to it
        if (m_carUser.first >= InputManager.Devices.Count)
            return;


		// Get player controller object
        var playerAInput = InputManager.Devices[m_carUser.first];
        var playerBInput = InputManager.Devices[m_carUser.second];

        if (playerAInput.RightStickY < 0 || playerBInput.RightStickY < 0)
        {
            backCamera.enabled = true;
        }
        else
        {
            backCamera.enabled = false;
        }
        if (playerAInput.RightStickButton.WasPressed || playerBInput.RightStickButton.WasPressed)
        {
            if (frontCamera.isActiveAndEnabled)
            {
                frontCamera.enabled = false;
            }
            else
            {
                frontCamera.enabled = true;
            }
        }
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
        /*
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
        */
        if (playerAInput.LeftBumper && playerBInput.LeftBumper && (playerAInput.LeftBumper.WasPressed || playerBInput.LeftBumper.WasPressed) &&  Main.S.practicing)
        {
            m_carUser.playerSwap();
        }

        bool resetting = false;
		if ((playerAInput.RightBumper.WasPressed || playerBInput.RightBumper.WasPressed) && m_carstate.currLap != 0)
        {
            resetting = true;
        }
        if (resetting)
        {
            if (!isCarBottom)
            {
                m_carstate.resets++;
            }
            else
            {
                m_carstate.resets++;
            }
            Logger.S.writeFile(!isCarBottom, "Reset To Before " + m_carstate.currCheckpoint + " at: " + Main.S.getGameTime());

            startReset();
        }
    }

    public void startReset()
    {
        carryHeight = 15;
        Vector3 newPos = new Vector3();
        Transform checkPoint;
        Quaternion newRotation;
        // Find the location and rotation of the prev checkpoint
        int index = 0;
        if (m_carstate.currCheckpoint != 0)
        {
            index = m_carstate.currCheckpoint - 1;
            checkPoint = m_carstate.checkpoints[m_carstate.currCheckpoint - 1];
            newRotation = m_carstate.checkpoints[m_carstate.currCheckpoint - 1].rotation;
        }
        else
        {
            index = m_carstate.checkpoints.Count - 1;
            checkPoint = m_carstate.checkpoints[m_carstate.checkpoints.Count - 1];
            newRotation = m_carstate.checkpoints[m_carstate.checkpoints.Count - 1].rotation;
        }

        newPos.x = checkPoint.position.x;
        newPos.y = checkPoint.position.y;
        newPos.z = checkPoint.position.z;

        if (isCarBottom)
        {
            newPos.x = m_carstate.bottomResetLocation[index].position.x;
            newPos.z = m_carstate.bottomResetLocation[index].position.z;
        }
        else
        {
            newPos.x = m_carstate.topResetLocation[index].position.x;
            newPos.z = m_carstate.topResetLocation[index].position.z;
        }

        targetLocation = newPos;
        targetRotation = newRotation;
        initalRotation = m_transform.rotation;

        // Move car to correct spot
        goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        m_car.zeroSpeed();
    }

    public void moveToNextCheckpoint()
    {
        moveToCheckpoint(m_carstate.currCheckpoint);
    }

    public void moveToStart()
    {
        carryHeight = 30;
        Transform checkPoint;
        // Find the location and rotation of the prev checkpoint
        if (isCarBottom)
            checkPoint = Main.S.Map.GetComponent<Map>().playerBottomStart.transform;
        else
            checkPoint = Main.S.Map.GetComponent<Map>().playerTopStart.transform;

        targetRotation = m_carstate.checkpoints[0].rotation;

        setTargetLocation(checkPoint);

        initalRotation = m_transform.rotation;

        // Move car to correct spot
        goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        m_car.zeroSpeed();
    }

    public void moveToCheckpoint(int checkpointToGo)
    {
        carryHeight = 10;
        Transform checkPoint;
        Quaternion newRotation;
        // Find the location and rotation of the prev checkpoint

        checkPoint = m_carstate.checkpoints[checkpointToGo];
        newRotation = m_carstate.checkpoints[checkpointToGo].rotation;

        setTargetLocation(checkPoint);
        targetRotation = newRotation;
        initalRotation = m_transform.rotation;

        // Move car to correct spot
        goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        m_car.zeroSpeed();
    }
    void setTargetLocation(Transform target)
    {
        Vector3 newPos = new Vector3();
        newPos.x = target.position.x;
        newPos.y = target.position.y;
        newPos.z = target.position.z;
        targetLocation = newPos;
    }


    //immediatly move the car to above the last checkpoint and immediatly rotate to correct direction
    public void hardReset()
    {
        Transform checkPoint;
        Quaternion newRotation;
        // Find the location and rotation of the prev checkpoint
        if (m_carstate.currCheckpoint != 0)
        {
            checkPoint = m_carstate.checkpoints[m_carstate.currCheckpoint - 1];
            newRotation = m_carstate.checkpoints[m_carstate.currCheckpoint - 1].rotation;
        }
        else
        {
            checkPoint = m_carstate.checkpoints[m_carstate.checkpoints.Count - 1];
            newRotation = m_carstate.checkpoints[m_carstate.checkpoints.Count - 1].rotation;
        }


        setTargetLocation(checkPoint);

        targetRotation = newRotation;
        initalRotation = m_transform.rotation;

        // Adjust angle to face correct direction
        Vector3 Angles = newRotation.eulerAngles;
        Angles.y += 90;
        newRotation = Quaternion.Euler(Angles.x, Angles.y, Angles.z);

        // Move car to correct spot
        m_transform.position = targetLocation;
        m_transform.rotation = newRotation;
        //this is only lowering him to ~1-5 right now. could polish this.
        m_car.zeroSpeed();
    }
    
    //called in update of carController
    //moves the car, up,to the last checkpoint,then down while rotating to the correct direction
    public void moveToLocation()
    {
        if (goingUp)
        {
            if (Main.S.carsReady != 2 && quickStart)
                CarmonyGUI.S.fadeOut();
            //Move up to y = 10;
            m_car.zeroSpeed();

            Vector3 newPos2 = m_transform.position;
            newPos2.y += .2f;

            m_transform.rotation = initalRotation;

            if (newPos2.y >= carryHeight)
            {
                goingUp = false;
                goingToPoint = true;
                newPos2.y = carryHeight;
                if (carrySpeed == 0)
                    carrySpeed = (float)Vector3.Distance(m_transform.position,targetLocation)/75;
            }
            m_transform.position = newPos2;

            Vector3 newRotation = m_transform.rotation.eulerAngles;
            m_transform.rotation = Quaternion.Euler(0, newRotation.y, 0); ;
            m_transform.position = newPos2;
        }
        else if (goingToPoint)
        {
            if (Main.S.carsReady != 2 && quickStart)
            {
                Vector3 newLoc = targetLocation;
                newLoc.y += 30;
                m_transform.position = newLoc;

                m_transform.rotation = Quaternion.Euler(0,targetRotation.y + 90,0);
                goingToPoint = false;
                goingDown = true;
                return;
            }


            Vector3 newPos = m_transform.position;
            //stop momentum and falling
            m_car.zeroSpeed();
            newPos.y = carryHeight;

            //Chose X direction to go;
            if ((newPos.x - targetLocation.x) > 2 * carrySpeed)
            {
                newPos.x -= carrySpeed;
            }
            else if ((newPos.x - targetLocation.x) < -2 * carrySpeed)
            {
                newPos.x += carrySpeed;
            }

            //chose z direction to go
            if ((newPos.z - targetLocation.z) > 2 * carrySpeed)
            {
                newPos.z -= carrySpeed;
            }
            else if ((newPos.z - targetLocation.z) < -2*carrySpeed)
            {
                newPos.z += carrySpeed;
            }

            Vector3 Angles = m_transform.rotation.eulerAngles;
            bool rotationDone = false;
            //Chose whether to rotate or not
            float rotateQuantity = 2;
            float initialAngle = initalRotation.eulerAngles.y % 360;
            float targetAngle = (targetRotation.eulerAngles.y + 90) % 360;
            if (targetAngle < 180 && initialAngle < targetAngle+180 && initialAngle > targetAngle)
            {

                rotateQuantity = -rotateQuantity;
            }else if (targetAngle > 180 && (initialAngle > targetAngle || initialAngle < targetAngle - 180))
            {
                rotateQuantity = -rotateQuantity;
            }
            if (Mathf.Abs(m_transform.rotation.eulerAngles.y % 360 - (targetRotation.eulerAngles.y + 90) % 360) > 3)
            {
                Angles.y += rotateQuantity;
                m_transform.rotation = Quaternion.Euler(0, Angles.y, 0);
            }
            else
            {
                //if rotation is done, stabalize
                rotationDone = true;
                Vector3 Angles2 = targetRotation.eulerAngles;
                Angles2.y += 90;

                m_transform.rotation = Quaternion.Euler(0, Angles2.y, 0);

            }

            if ((Mathf.Abs(newPos.x - targetLocation.x) <= 2*carrySpeed  && Mathf.Abs(newPos.z - targetLocation.z) <= carrySpeed*2) && rotationDone)
            {
                newPos.x = targetLocation.x;
                newPos.z = targetLocation.z;
                goingToPoint = false;
                goingDown = true;
            }
            m_transform.position = newPos;

        }
        else if (goingDown)
        {
            if (Main.S.carsReady != 2 && quickStart)
                CarmonyGUI.S.fadeIn();

            //Do not allow rotation until close to hitting the ground.
            Vector3 Angles2 = targetRotation.eulerAngles;
            Angles2.y += 90;
            m_transform.rotation = Quaternion.Euler(0, Angles2.y, 0);
            if (m_transform.position.y < .1)
            {
                goingDown = false;
                carrySpeed = 0;
                m_car.zeroSpeed();
                Main.S.setCarReady();
            }
        }
    }

    public void startBoost()
    {
        StartCoroutine("boostPowerup");
    }

    IEnumerator boostPowerup()
    {
        isShrinking = true;
        yield return new WaitForSeconds(5f);
        isNormalizingUp = true;
    }

    // Spawn a rocket with the passed in parameters
    public void spawnRocket(int rocketStop, Vector3 startPos, GameObject target)
    {
        var rocketgo = Instantiate(rocketPrefab);
        rocketgo.GetComponent<Rocket>().InitializeRocket(rocketStop, startPos, target);
    }
}
