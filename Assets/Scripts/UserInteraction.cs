using UnityEngine;
using System.Collections;
using InControl;

public class UserInteraction : MonoBehaviour {

	private ArcadeVehicle m_arcadeVehicle;
    private CarState m_carstate;
    private Transform m_transform;

	[Header("Set in Inspector")]
	public float 			boostAccel;
	public GameObject 		oilPrefab;
	public Camera 			backCamera;
	public Camera 			frontCamera;
	public GameObject 		rocketPrefab;
	public ParticleSystem 	explosion;

	[Header("Calculated Dynamically")]
	public float 			carrySpeed;
	public float 			carryHeight;
	public Vector3 			targetLocation;
	public Quaternion 		targetRotation;
	public Quaternion 		initalRotation;
    
	public bool 			isCarBottom;
    public bool 			isShrinking;
	public bool 			isGrowing;
    public bool 			isNormalizingUp;
    public bool 			isNormalizingDown;
	public bool 			goingUp;
	public bool 			goingToPoint;
	public bool 			goingDown;
	public bool 			quickStart;

    void Awake()
    {
        m_arcadeVehicle = gameObject.GetComponentInParent<ArcadeVehicle>();
        m_carstate = gameObject.GetComponentInParent<CarState>();
        m_transform = gameObject.GetComponentInParent<Transform>();		
    }

    // Use this for initialization
    void Start () {
		isCarBottom = m_arcadeVehicle.isBottomCar;
        quickStart = true;
	}

    // Update is called once per frame
    void Update () {
        // Check if car is done and if so ignore input
		if (m_arcadeVehicle.SetNuetral ())
			return;
        
        // Prevent players from using reset right away
		if (!Main.S.getRaceStarted() && !Main.S.practicing)
            return;

		// If car has no controllers attached to it
		if (m_arcadeVehicle.first >= InputManager.Devices.Count)
            return;
		
		// Get player controller object
		InputDevice playerAInput = InputManager.Devices[m_arcadeVehicle.first];
		InputDevice playerBInput = InputManager.Devices[m_arcadeVehicle.second];

		// Check for input to change camera angles
		CheckAdustCamera(playerAInput, playerBInput);

		// Do the animations for shrinking and growing the car
		HandleCarSizeChange ();
       
		// In the practice map check for player swapping
        if (playerAInput.LeftBumper && playerBInput.LeftBumper && (playerAInput.LeftBumper.WasPressed || playerBInput.LeftBumper.WasPressed) &&  Main.S.practicing)
        {
			m_arcadeVehicle.playerSwap();
        }

		// Check for resseting input
		// TODO: understand the resseting and jumping checkponts code
        bool resetting = false;
		if ((playerAInput.RightBumper.WasPressed || playerBInput.RightBumper.WasPressed) && m_carstate.currLap != 0)
        {
            resetting = true;
        }
        if (resetting)
        {
            m_carstate.resets++;
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
        m_arcadeVehicle.zeroSpeed();
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
        m_arcadeVehicle.zeroSpeed();
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
        m_arcadeVehicle.zeroSpeed();
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
        m_arcadeVehicle.zeroSpeed();
    }
    
    // called in update of arcadeVehicle
    // moves the car, up,to the last checkpoint,then down while rotating to the correct direction
    public void moveToLocation()
    {
        if (goingUp)
        {
            if (Main.S.carsReady != 2 && quickStart)
                CarmonyGUI.S.fadeOut();
            //Move up to y = 10;
            m_arcadeVehicle.zeroSpeed();

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
            m_arcadeVehicle.zeroSpeed();
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
            if (m_transform.position.y < .8)
            {
                goingDown = false;
                carrySpeed = 0;
                m_arcadeVehicle.zeroSpeed();
                Main.S.setCarReady();
            }
        }
    }

	void CheckAdustCamera(InputDevice playerAInput, InputDevice playerBInput){
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
	}
	
	/****************************************************
	 * 				  POWER UP METHODS					*
	 *****************************************************/

	/**************** Oil Spill Methods ****************/

	public void placeOilSpill()
	{
		Vector3 newLocation = gameObject.GetComponentInChildren<Camera>().GetComponent<Transform>().position;
		newLocation.y = oilPrefab.transform.position.y;
		GameObject go = Instantiate(oilPrefab, newLocation, oilPrefab.transform.rotation) as GameObject;
		go.GetComponent<OilSpill>().isFromTop = !isCarBottom;
	}
	
	/**************** Boost Methods ****************/

    public void startBoost()
    {
        StartCoroutine("boostPowerup", m_arcadeVehicle);
    }

	// Set flags for the update method to handle shrinking and growing
    IEnumerator boostPowerup(ArcadeVehicle vehicle)
    {
        isShrinking = true;
		m_arcadeVehicle.accel += boostAccel;
        m_arcadeVehicle.accelPointRel.z = 1;
        yield return new WaitForSeconds(5f);
        m_arcadeVehicle.accelPointRel.z = 0;
        isNormalizingUp = true;
		m_arcadeVehicle.accel -= boostAccel;
	}

	void HandleCarSizeChange(){
		if (isShrinking)
		{
			Vector3 newSize = gameObject.transform.localScale;
			newSize.x = newSize.x  - .1f;
			newSize.y = newSize.y - .05f;
			newSize.z = newSize.z  -.15f;
			if(newSize.y <= .5)
			{
				newSize.x = 1f;
				newSize.y = .5f;
				newSize.z = 1.5f;
				isShrinking = false;
			}
			gameObject.transform.localScale = newSize;	
		}
		if (isNormalizingUp)
		{
			Vector3 newSize = gameObject.transform.localScale;
			newSize.x = newSize.x + .1f;
			newSize.y = newSize.y + .05f;
			newSize.z = newSize.z + .15f;
			if (newSize.y >= 1)
			{
				newSize.x = 2f;
				newSize.y = 1f;
				newSize.z = 3f;
				isNormalizingUp = false;
			}
			gameObject.transform.localScale = newSize;
		}
		if (isGrowing)
		{
			Vector3 newSize = gameObject.transform.localScale;
			newSize.x = newSize.x + .1f;
			newSize.y = newSize.y + .05f;
			newSize.z = newSize.z + .15f;
			if (newSize.y >= 2)
			{
				newSize.x = 4f;
				newSize.y = 2f;
				newSize.z = 6f;
				isGrowing = false;
			}
			gameObject.transform.localScale = newSize;
		}
		if (isNormalizingDown)
		{
			Vector3 newSize = gameObject.transform.localScale;
			newSize.x = newSize.x - .1f;
			newSize.y = newSize.y - .05f;
			newSize.z = newSize.z - .15f;
			if (newSize.y <= 1)
			{
				newSize.x = 2f;
				newSize.y = 1f;
				newSize.z = 3f;
				isNormalizingDown = false;
			}
			gameObject.transform.localScale = newSize;
		}
	}

	/**************** Rocket Methods ****************/

	// Spawn a rocket with the passed in parameters
	public void spawnRocket(int rocketStop, Vector3 startPos, GameObject target)
	{
		var rocketgo = Instantiate(rocketPrefab);
		rocketgo.GetComponent<Rocket>().InitializeRocket(rocketStop, startPos, target);
	}

	public void startBombRaiseCar()
	{
		Vector3 newVel = gameObject.GetComponent<Rigidbody>().velocity;
		newVel.x = 0;
		newVel.z = 0;
		newVel.y = 10;
		gameObject.GetComponent<Rigidbody>().velocity = newVel;
	}
}
