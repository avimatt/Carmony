using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.UI;

public class ArcadeVehicle : MonoBehaviour {
	[Header("Set in Inspector")]
	public Vector3			centerOfMass = new Vector3(0,-1,0);
	public Vector3			accelPointRel = new Vector3(0, -.25f, 0.5f);
	public float			groundedDrag = 0.5f;
	public float			groundedAngularDrag = 2;
	public float			airDrag = 0;
	public float			airAngularDrag = 2;
	public float			accel = 10;
	public float			turningTorque = 10;
	public float			horizontalFriction = 0.5f;
	public float			horizontalFrictionSkidding = 0.2f;
	public float			maxRotation = 50f;
	public GameObject[]		tireModels;

	[Header("From CarUserController")]
	public int 				first = 3; // first - player who turns right and accelerates
	public int 				second = 3; // second - player who turns left and brakes
	public bool 			isBottomCar;
	public GameObject 		monster;
	public ParticleSystem 	leftSteam;
	public ParticleSystem 	rightSteem;
	public GameObject 		steeringWheel;
	
	[Header("Calculated Dynamically")]
	public float		iH;
	public float		iV;
	public bool			grounded;
	public bool			skidding;
	public List<Shock>	shocks;

	// Private
	UserInteraction 	m_userInteract;
	Rigidbody			rigid;
	bool 				swapping;

	void Awake() {
		rigid = GetComponent<Rigidbody>();
		rigid.centerOfMass = new Vector3(0,-1,0);
		shocks = new List<Shock>(transform.GetComponentsInChildren<Shock>());
		m_userInteract = gameObject.GetComponentInParent<UserInteraction>();
	}
	
	void FixedUpdate () {
		// Make sure you are not moving at the start of the race
        if (!Main.S.practicing && !Main.S.raceStarted && rigid.transform.position.y < .75f)
        {
            rigid.drag = 100;
        }
        else
        {
            rigid.drag = .5f;
        }



		m_userInteract.moveToLocation();

		// Avoid errors when not all controllers are connected
		if (first >= InputManager.Devices.Count)
			return;

		// Make sure car isn't moving before race starts
		if (!Main.S.getRaceStarted() && !Main.S.practicing)
		{
			zeroXYSpeed();
			return;
		}

		float leftIn, rightIn, gasIn, brakeIn;
		bool badA, badB;
		GetInput (out leftIn, out rightIn, out gasIn, out brakeIn, out badA, out badB);

		iH = leftIn + rightIn;
		iV = gasIn + brakeIn;
        if (iV < 0) iV = iV / 4;

		// Ignore input if done with race
		if(SetNuetral()) return;

		// Indicate to players who is pressing what
		singleStream(gasIn, brakeIn);	

		// Give Vibrate feedback for wrong controls
		VibrateNegativeFeedback(badA, badB);

		grounded = Grounded;
		skidding = Skidding;

        var velocity = rigid.velocity;
        var localVel = transform.InverseTransformDirection(velocity);
        if (localVel.z < 0)
        {
            iH = -iH;
        }
        if (getSpeed() < 5)
        {
            iH = iH / (1/(getSpeed()/5f));
            if (getSpeed() == 0)
                iH = 0;
        }
		// Steering
		if (grounded) {
			rigid.AddTorque (0, iH * turningTorque, 0);
		}

		rigid.centerOfMass = centerOfMass;

		Vector3 fwdAlongGround = transform.forward;
		fwdAlongGround.y = 0;
		fwdAlongGround.Normalize();

		Vector3 accelPoint;
		if (grounded) {
			rigid.drag = groundedDrag;
			rigid.angularDrag = groundedAngularDrag;

			accelPoint = transform.TransformPoint(accelPointRel);

			Debug.DrawLine(transform.position, accelPoint);
			rigid.AddForceAtPosition(iV * accel * fwdAlongGround, accelPoint);

			foreach(GameObject tireModel in tireModels){
				if((iV * accel) > 0){
					tireModel.transform.Rotate(5.0f, 0, 0);
				} else if ((iV * accel) < 0){
					tireModel.transform.Rotate(-5.0f, 0, 0);
				}
			}

			// Add horizontal resistance to mimic directional friction of tires
			// This could use some work!
			Vector3 vel = rigid.velocity;
            Vector3 newVel = vel;
            if (vel.y > 0)
                newVel.y /= Main.S.Map.GetComponent<Map>().mapYGroundModifier;
            rigid.velocity = newVel;
            vel = rigid.velocity;

            Vector3 horizontalVel = transform.right * Vector3.Dot(transform.right, vel);
			horizontalVel.y = 0; // nullify y effects of this

			if (!skidding) {
				rigid.AddForce(-horizontalVel*horizontalFriction, ForceMode.VelocityChange);
			} else {
				rigid.AddForce(-horizontalVel*horizontalFrictionSkidding, ForceMode.VelocityChange);
			}
		} else {
			rigid.drag = airDrag;
			rigid.angularDrag = airAngularDrag;
            Vector3 newVel = rigid.velocity;

            if (newVel.y > 0)
            {
                if (isBottomCar)
                    newVel.y /= Main.S.Map.GetComponent<Map>().bottomMapYAirModifier;
                else
                    newVel.y /= Main.S.Map.GetComponent<Map>().topMapYAirModifier;

            }
            rigid.velocity = newVel;
            //accelPoint = transform.TransformPoint(centerOfMass);
        }
        RotateSteeringWheel();

        RotateFrontWheels ();
	}

	public bool Grounded {
		get {
			foreach (Shock sh in shocks) {
				if (sh.grounded) return true;
			}
			return false;
		}
	}

	public bool Skidding {
		get {
			return Input.GetKey(KeyCode.LeftShift);
		}
	}

	public float getSpeed(){
		return rigid.velocity.magnitude*2.23693629f;
	}
	
	public void zeroSpeed()
	{
		rigid.velocity = new Vector3(0, 0, 0);
	}
	
	public void zeroXYSpeed()
	{
		rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
	}

	/// <summary>
	/// Starts the vibration for players 1 and 2 based on bool inputs
	/// </summary>
	public void startVibration(bool A, bool B)
	{
		var playerAInput = InputManager.Devices[first];
		var playerBInput = InputManager.Devices[second];
		if(A) playerAInput.Vibrate(1f, 1f);
		if(B) playerBInput.Vibrate(1f, 1f);
	}

	/// <summary>
	/// Ends the vibration for players 1 and 2 based on bool inputs
	/// </summary>
	public void endVibration(bool A, bool B)
	{
		var playerAInput = InputManager.Devices[first];
		var playerBInput = InputManager.Devices[second];
		if(A) playerAInput.Vibrate(0f, 0f);
		if(B) playerBInput.Vibrate(0f, 0f);
	}

	/// <summary>
	/// Show SwapText and start Co-Routine
	/// </summary>
	public void playerSwap()
	{
		if (swapping)
			return;
		else
			swapping = true;
		// Start pulse vibration
		startVibration(true, true);
		
		if (!isBottomCar)
		{
			CarmonyGUI.S.topSwapText.SetActive(true);
			CarmonyGUI.S.swappingTop = true;
		}
		else
		{
			CarmonyGUI.S.bottomSwapText.SetActive(true);
			CarmonyGUI.S.swappingBottom = true;
		}

		//call co-routine
		StartCoroutine("swapGrow");
		StartCoroutine("turnMonster");
		StartCoroutine("pulseWait");
	}

	/// <summary>
	/// Checks whether the car is done and if so sets the nuetral 
	/// </summary>
	/// <returns><c>true</c>, if nuetral was set, <c>false</c> otherwise.</returns>
	public bool SetNuetral(){
		// pass the input to the car!
		if (!isBottomCar)
		{
			if (Main.S.carTopDone)
			{
				iH = 0;
				iV = 0;
				return true;
			}
		}
		else
		{
			if (Main.S.carBottomDone)
			{
				iH = 0;
				iV = 0;
				return true;
			}
		}
		return false;
	}
	
	// Color the streams to indicate who is pressing what
	void singleStream(float accel,float footbrake)
	{
		if (leftSteam && rightSteem)
		{
			if (accel > 0)
			{
				rightSteem.startColor = new Color32(0, 255, 44, 255);
			}
			else
			{
				rightSteem.startColor = new Color32(255, 255, 255, 255);
			}
			if (footbrake < 0)
			{
				leftSteam.startColor = new Color32(255, 0, 0, 255);
			}
			else
			{
				leftSteam.startColor = new Color32(255, 255, 255, 255);
			}
		}
	}

	void VibrateNegativeFeedback(bool badA, bool badB){
		if (first != second)
		{
			if (badA)
			{
				startVibration(true, false);
			}
			else
			{
				endVibration(true, false);
			}
			if (badB)
			{
				startVibration(false, true);
			}
			else
			{
				endVibration(false, true);
			}
		}
	}
	
	// Actually Swap the players controls
	void swapControls()
	{
		// Turn off vibrate
		endVibration(true, true);
		
		//swap controls
		int temp = first;
		first = second;
		second = temp;
		
		ParticleSystem tempPart = leftSteam;
		leftSteam = rightSteem;
		rightSteem = tempPart;
	}

	void GetInput(out float leftIn, out float rightIn, out float gasIn, out float brakeIn, out bool badA, out bool badB){
		var playerAInput = InputManager.Devices[first];
		var playerBInput = InputManager.Devices[second];

		badA = false;
		badB = false;

		// Get Accel/Decel
		// Player A controls accelerator and turning right
		if (Main.S.normalControls)
		{
			gasIn = playerAInput.RightTrigger;
			if (playerBInput.RightTrigger)
				badB = true;
			// Player B controls footbrake, handbrake, and turning left
			brakeIn = -1f * playerBInput.LeftTrigger;
			if (playerAInput.LeftTrigger)
				badA = true;
		}
		else
		{
			gasIn = playerAInput.RightTrigger;
			brakeIn = -1f * playerAInput.LeftTrigger;
		}

		// Get Left/Right
		if (Main.S.normalControls)
		{
			rightIn = Mathf.Max(0f, playerAInput.LeftStickX);
			if (Mathf.Max(0f, playerBInput.LeftStickX) > 0)
				badB = true;
			leftIn = Mathf.Min(0f, playerBInput.LeftStickX);
			if (Mathf.Min(0f, playerAInput.LeftStickX) < 0)
				badA = true;
		}
		else{
			rightIn = Mathf.Max(0f, playerBInput.LeftStickX);
			leftIn = Mathf.Min(0f, playerBInput.LeftStickX);
		}
	}

	void RotateSteeringWheel(){
		float degree =  180 -iH * 60;
		Vector3 newRot = steeringWheel.transform.rotation.eulerAngles;
		newRot.z = degree;
		steeringWheel.transform.rotation = Quaternion.Euler(newRot);
	}
	
	void RotateFrontWheels(){
		Vector3 lf = tireModels [2].transform.localRotation.eulerAngles;
		Vector3 lr = tireModels [3].transform.localRotation.eulerAngles;
		
		if (iH > 0) {
			tireModels [2].transform.Rotate (0, iH * maxRotation - lf.y, 0);
			tireModels [3].transform.Rotate (0, iH * maxRotation - lr.y, 0);
		} else if (iH < 0) {
			tireModels [2].transform.Rotate (0, iH * maxRotation - lf.y, 0);
			tireModels [3].transform.Rotate (0, iH * maxRotation - lr.y, 0);
		}
		
		lf = tireModels [2].transform.localRotation.eulerAngles;
		lr = tireModels [3].transform.localRotation.eulerAngles;
		lf.z = 0;
		lr.z = 0;
		tireModels [2].transform.localRotation = Quaternion.Euler(lf);
		tireModels [3].transform.localRotation = Quaternion.Euler(lr);
	}


	/******************* COROUTINES ********************/
	IEnumerator turnMonster()
	{
		float curTime = Time.time;
		Vector3 origAngle = monster.transform.localEulerAngles;
		while(Time.time - curTime < 2)
		{
			Vector3 newRot = monster.transform.rotation.eulerAngles;
			newRot.y = newRot.y + 20;
			monster.transform.rotation = Quaternion.Euler(newRot);
			yield return 0;
		}
		origAngle.y += 180f;
		monster.transform.localRotation = Quaternion.Euler(origAngle);

		swapping = false;
	}
	
	IEnumerator swapGrow()
	{
		yield return new WaitForSeconds(2);
		float savedTime = Time.time;
		while (Time.time - savedTime < 2)
		{
			if (!isBottomCar)
			{
				CarmonyGUI.S.topSwapText.GetComponent<Text>().fontSize += 4;
				Color newColor = CarmonyGUI.S.topSwapText.GetComponent<Text>().color;
				newColor.a -= .03f;
				CarmonyGUI.S.topSwapText.GetComponent<Text>().color = newColor;
			}
			else
			{
				Color newColor = CarmonyGUI.S.bottomSwapText.GetComponent<Text>().color;
				newColor.a -= .03f;
				CarmonyGUI.S.bottomSwapText.GetComponent<Text>().color = newColor;
				CarmonyGUI.S.bottomSwapText.GetComponent<Text>().fontSize += 4;
			}
			yield return 0;
		}

		if (!isBottomCar)
		{
			CarmonyGUI.S.topSwapText.GetComponent<Text>().fontSize = 64;
			Color newColor = CarmonyGUI.S.topSwapText.GetComponent<Text>().color;
			newColor.a = 1;
			CarmonyGUI.S.topSwapText.GetComponent<Text>().color = newColor;
			CarmonyGUI.S.topSwapText.SetActive(false);
		}
		else
		{
			CarmonyGUI.S.bottomSwapText.GetComponent<Text>().fontSize = 64;
			Color newColor = CarmonyGUI.S.bottomSwapText.GetComponent<Text>().color;
			newColor.a = 1;
			CarmonyGUI.S.bottomSwapText.GetComponent<Text>().color = newColor;
			CarmonyGUI.S.bottomSwapText.SetActive(false);
		}
	}
	
	IEnumerator pulseWait()
	{
		yield return new WaitForSeconds(1);
		swapControls();
	}
}
