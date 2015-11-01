using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using InControl;

public class UserInteraction : MonoBehaviour {

    
    public bool isCarTop;
    public bool isBoosting;
    public float boostTimer;

	// Use this for initialization
	void Start () {
        isCarTop = gameObject.GetComponentInParent<CarUserControl>().isBottomCar;
        boostTimer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isCarTop)
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
        if (Time.time < 1)
            return;
        if (userControl.first >= InputManager.Devices.Count)
            return;

        var playerAInput = InputManager.Devices[userControl.first];
        var playerBInput = InputManager.Devices[userControl.second];

        //only resetting to beginnning right now
        //move to last checkpoint
        if (isBoosting && Time.time - boostTimer > 5)
            isBoosting = false;
        bool resetting = false;


        if ((playerAInput.RightBumper || playerBInput.RightBumper) && !isCarTop)
        {
            resetting = true;
        }
        else if ((playerAInput.RightBumper || playerBInput.RightBumper) && isCarTop)
        {
            resetting = true;
        }
        if (resetting)
        {
            Vector3 newPos = new Vector3();
            Transform checkPoint;
            Quaternion newRotation;
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

            Vector3 Angles = newRotation.eulerAngles;
            Angles.y += 90;
            newRotation = Quaternion.Euler(Angles.x, Angles.y, Angles.z);
            //newRotation.y += .9f;

            if (isCarTop) {

                gameObject.GetComponentInParent<Transform>().position = newPos;//new Vector3(34, 0, 449);
            }
            else
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
