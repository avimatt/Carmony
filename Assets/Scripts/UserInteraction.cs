using UnityEngine;

using UnityEngine.UI;
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
    public Camera mainCamera;
	public GameObject 		rocketPrefab;
    public GameObject portalPrefab;
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
	public bool             portalTransport;
    public float            bombTimer;
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
		if ((playerAInput.RightBumper.WasPressed || playerBInput.RightBumper.WasPressed) && m_carstate.currLap != 0)
        {
            m_carstate.resets++;
            Logger.S.writeFile(!isCarBottom, "Reset To Before " + m_carstate.currCheckpoint + " at: " + Main.S.getGameTime());

            startReset();
        }
    }

	/// <summary>
	/// Set target to the previous checkpoint
	/// </summary>
    public void startReset()
    {

        carryHeight = 15;
		Transform checkPoint;
        
		Vector3 tempTargetLocation = new Vector3();
        Quaternion tempnTargetRotation;
        
		// Find the location and rotation of the prev checkpoint
        int index = 0;
        if (m_carstate.currCheckpoint != 0)
        {
            index = m_carstate.currCheckpoint - 1;
            checkPoint = m_carstate.checkpoints[m_carstate.currCheckpoint - 1];
        }
        else
        {
            index = m_carstate.checkpoints.Count - 1;
            checkPoint = m_carstate.checkpoints[m_carstate.checkpoints.Count - 1];
        }

		// Get the temp location and rotation
		tempnTargetRotation = checkPoint.rotation;
		tempTargetLocation.y = checkPoint.position.y;
        if (isCarBottom)
        {
			tempTargetLocation.x = m_carstate.bottomResetLocation[index].position.x;
			tempTargetLocation.z = m_carstate.bottomResetLocation[index].position.z;
        }
        else
        {
			tempTargetLocation.x = m_carstate.topResetLocation[index].position.x;
			tempTargetLocation.z = m_carstate.topResetLocation[index].position.z;
        }

		// Set the target location and roation to the temp
		targetLocation = tempTargetLocation;
		targetRotation = tempnTargetRotation;
        initalRotation = m_transform.rotation;

		// If player went of the course in the practice map
		if (Main.S.practicing) {
			targetLocation = new Vector3(66f, 1,  1407f);
			targetRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}

        // Move car to correct spot
        goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        m_arcadeVehicle.zeroSpeed();
    }

	/// <summary>
	/// Move x checkpoints ahead corresponding to distance of portal.
	/// </summary>
    public void moveToNextCheckpoint()
    {
        //portalTransport = true;
        carrySpeed = 4f;
		// Advance checkoint, check to see if you are at the finish line and repeat if not...
        m_carstate.checkpoints[m_carstate.currCheckpoint].GetComponent<Checkpoint>().hitCheckpoint(transform);
        if (m_carstate.currCheckpoint == 0 && m_carstate.currLap == Main.S.Map.GetComponent<Map>().numLaps)
        {
            portalToCheckpoint(m_carstate.currCheckpoint);
            return;
        }
        m_carstate.checkpoints[m_carstate.currCheckpoint].GetComponent<Checkpoint>().hitCheckpoint(transform);
        if (m_carstate.currCheckpoint == 0 && m_carstate.currLap == Main.S.Map.GetComponent<Map>().numLaps)
        {
            portalToCheckpoint(m_carstate.currCheckpoint);
            return;
        }
        m_carstate.checkpoints[m_carstate.currCheckpoint].GetComponent<Checkpoint>().hitCheckpoint(transform);
        if (m_carstate.currCheckpoint == 0 && m_carstate.currLap == Main.S.Map.GetComponent<Map>().numLaps)
        {
            portalToCheckpoint(m_carstate.currCheckpoint);
            return;
        }
        m_carstate.checkpoints[m_carstate.currCheckpoint].GetComponent<Checkpoint>().hitCheckpoint(transform);
        if (m_carstate.currCheckpoint == 0 && m_carstate.currLap == Main.S.Map.GetComponent<Map>().numLaps)
        {
            portalToCheckpoint(m_carstate.currCheckpoint);
            return;
        }
        //moveToCheckpoint(m_carstate.currCheckpoint);
        portalToCheckpoint(m_carstate.currCheckpoint);
    }

    public void portalToCheckpoint(int checkpointToGo)
    {
        //carryHeight = 5;
        Transform checkPoint;
        Quaternion newRotation;

        // Find the location and rotation of the prev checkpoint
        checkPoint = m_carstate.checkpoints[checkpointToGo];
        newRotation = m_carstate.checkpoints[checkpointToGo].rotation;

        setTargetLocation(checkPoint);
        targetRotation = newRotation;
        initalRotation = m_transform.rotation;

        // Move car to correct spot
        //goingUp = true;

        //this is only lowering him to ~1-5 right now. could polish this.
        //m_arcadeVehicle.zeroSpeed();



        StartCoroutine("doPortal");
            
            //(new Vector3(100, 100, 100));
    }

    IEnumerator doPortal()
    {
        targetLocation.y += 4;
        transform.position = targetLocation;
        targetLocation.y -= 3;

        Vector3 newAngle = targetRotation.eulerAngles;
        newAngle.y += 90f;
        transform.rotation = Quaternion.Euler(newAngle);



        Vector3 direction = gameObject.transform.forward;
        float speed = m_arcadeVehicle.getSpeed() / 3;
        print(direction);
        float startTime = Time.time;

        //hide player (doing it this way in case i want to fade him.
        MeshRenderer[] meshes = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            Color32 newColor = mesh.material.color;
            newColor.a = 0;
            mesh.material.color = newColor;
        }
        SkinnedMeshRenderer[] meshes2 = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes2)
        {
            Color32 newColor = mesh.material.color;
            newColor.a = 0;
            mesh.material.color = newColor;
        }

        m_arcadeVehicle.leftSteam.gameObject.SetActive(false);
        m_arcadeVehicle.rightSteem.gameObject.SetActive(false);

        //create portal
        GameObject portalObj = (GameObject)Instantiate(portalPrefab, targetLocation, targetRotation);

        mainCamera.fieldOfView = 100;

        /////////////////////////////////wait for user to be ready
        while (Time.time - startTime < 2 || mainCamera.fieldOfView != 60)
        {
            mainCamera.fieldOfView -= .5f;
            if (mainCamera.fieldOfView < 60)
                mainCamera.fieldOfView = 60;
            transform.rotation = Quaternion.Euler(newAngle);
            m_arcadeVehicle.zeroSpeed();
            yield return 0;
        }



        //show player
        foreach(MeshRenderer mesh in meshes)
        {
            Color32 newColor = mesh.material.color;
            newColor.a = 255;
            mesh.material.color = newColor;
        }
        foreach (SkinnedMeshRenderer mesh in meshes2)
        {
            Color32 newColor = mesh.material.color;
            newColor.a = 255;
            mesh.material.color = newColor;
        }
        m_arcadeVehicle.leftSteam.gameObject.SetActive(true);
        m_arcadeVehicle.rightSteem.gameObject.SetActive(true);


        //add velocity
        m_arcadeVehicle.GetComponent<Rigidbody>().velocity = new Vector3(speed * direction.x, speed * direction.y, speed * direction.z);
        yield return new WaitForSeconds(10);
        Destroy(portalObj);
    }

    /// <summary>
    /// Set target to the start line for the beginning of the game.
    /// </summary>
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

	/// <summary>
	/// Set target to the specified checkpoint.
	/// </summary>
	/// <param name="checkpointToGo">Checkpoint to go t0.</param>
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

	/// <summary>
	/// Sets the target location.
	/// </summary>
	/// <param name="target">Target.</param>
    void setTargetLocation(Transform target)
    {
        Vector3 newPos = new Vector3();
        newPos.x = target.position.x;
        newPos.y = target.position.y;
        newPos.z = target.position.z;
        targetLocation = newPos;
    }

	/// <summary>
	/// Immediatly move the car to above the last checkpoint and immediatly rotate to correct direction
	/// </summary>
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
    
	/// <summary>
	/// Moves the car up to the last checkpoint,then down while rotating to the correct direction
	/// </summary>
	/// <remarks>
	/// Called in update of arcadeVehicle
	/// </remarks>
    public void moveToLocation()
    {
		// Move up to carry height
        if (goingUp)
        {
			// Fade out and don't lift up if coming from the practice screen
            if (Main.S.carsReady != 2 && quickStart)
                CarmonyGUI.S.fadeOut();

			m_arcadeVehicle.zeroSpeed();

            Vector3 newPos2 = m_transform.position;
            newPos2.y += .2f;

			// Prevent it from rotating due to "Physics"
            m_transform.rotation = initalRotation;

			// When it gets to the desired height
            if (newPos2.y >= carryHeight)
            {
                goingUp = false;
                goingToPoint = true;
                newPos2.y = carryHeight;
                if (carrySpeed == 0)
                {
                    carrySpeed = (float)Vector3.Distance(m_transform.position, targetLocation) / 75;
                }
            }

			// Set new rotation and position
            Vector3 newRotation = m_transform.rotation.eulerAngles;
            m_transform.rotation = Quaternion.Euler(0, newRotation.y, 0); 
            m_transform.position = newPos2;
        }
        else if (goingToPoint)
        {
			// If coming from practice map go straight to point
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


            //stop momentum and falling
            m_arcadeVehicle.zeroSpeed();

			// Find new Position
			Vector3 newPos = m_transform.position;
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

			// Find new aangle
            Vector3 Angles = m_transform.rotation.eulerAngles;
            bool rotationDone = false;
            //Chose whether to rotate or not
            float rotateQuantity = 2;
            float initialAngle = initalRotation.eulerAngles.y % 360;
            float targetAngle = (targetRotation.eulerAngles.y + 90) % 360;
			if (targetAngle < 180 && initialAngle < targetAngle+180 && initialAngle > targetAngle || 
			    (targetAngle > 180 && (initialAngle > targetAngle || initialAngle < targetAngle - 180))){
                
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

			// If the car is at the correct rotation and position we finish moving to point and start going down
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
			// If coming from the practice fade back in
            if (Main.S.carsReady != 2 && quickStart)
                CarmonyGUI.S.fadeIn();

            //Do not allow rotation until close to hitting the ground.
            Vector3 Angles2 = targetRotation.eulerAngles;
            Angles2.y += 90;
            m_transform.rotation = Quaternion.Euler(0, Angles2.y, 0);
			// If we are on the "ground" stop moving
            if (m_transform.position.y < .8)
            {
                goingDown = false;
                carrySpeed = 0;
                m_arcadeVehicle.zeroSpeed();
                portalTransport = false;
                if (!isCarBottom)
                {
                    CarmonyGUI.S.topSwapText.GetComponent<Text>().text = "SWAP";
                    CarmonyGUI.S.topSwapText.SetActive(false);
                }
                else
                {
                    CarmonyGUI.S.bottomSwapText.GetComponent<Text>().text = "SWAP";
                    CarmonyGUI.S.bottomSwapText.SetActive(false);
                }
				// If you portaled to or past the finish line
                if (m_carstate.currCheckpoint == 1 && m_carstate.currLap == Main.S.Map.GetComponent<Map>().numLaps + 1)
                {
                    Main.S.endGame(!isCarBottom);
                }
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
        if (gameObject.GetComponent<ArcadeVehicle>().grounded == false)
            return;
		Vector3 newLocation = gameObject.GetComponentInChildren<Camera>().GetComponent<Transform>().position;
        newLocation.y = gameObject.transform.position.y - .5f;//oilPrefab.transform.position.y;

		GameObject go = Instantiate(oilPrefab, newLocation, oilPrefab.transform.rotation) as GameObject;
		go.GetComponent<OilSpill>().isFromTop = !isCarBottom;
	}
	
	/**************** Boost Methods ****************/

    public void startBoost()
    {
        StartCoroutine("boostPowerup", m_arcadeVehicle);
    }

	/// <summary>
	/// Set flags for the update method to handle shrinking and growing
	/// </summary>
	/// <returns>The powerup.</returns>
	/// <param name="vehicle">Vehicle to boost.</param>
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

	/// <summary>
	/// Handles the car size change.
	/// </summary>
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

	/// <summary>
	/// Spawns the rocket.
	/// </summary>
	/// <param name="rocketStop">Rocket stop.</param>
	/// <param name="startPos">Start position.</param>
	/// <param name="target">Target.</param>
	public void spawnRocket(int rocketStop, Vector3 startPos, GameObject target)
	{
		var rocketgo = Instantiate(rocketPrefab);
		rocketgo.GetComponent<Rocket>().InitializeRocket(rocketStop, startPos, target);
	}

	/// <summary>
	/// Starts the explosion effect and raise the car.
	/// </summary>
	public void startBombRaiseCar()
	{
        bombTimer = Time.time;
        if (isCarBottom)
        {
            Main.S.Map.GetComponent<Map>().bottomMapYAirModifier = 1.2f;
            print("hello");
        }
        else
        {
            Main.S.Map.GetComponent<Map>().topMapYAirModifier = 1.2f;
            print("hello");
        }

        Vector3 newVel = gameObject.GetComponent<Rigidbody>().velocity;
		newVel.x = 0;
		newVel.z = 0;
		newVel.y = 400;
        if (isCarBottom && Main.S.carBottomDone)
            newVel.y = 50;
        else if (!isCarBottom && Main.S.carTopDone)
            newVel.y = 50;

		gameObject.GetComponent<Rigidbody>().velocity = newVel;
	}
}
