using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class UserInteraction : MonoBehaviour {


    public bool isCarTop;
    public bool isBoosting;
    public float boostTimer;
	// Use this for initialization
	void Start () {
        isCarTop = gameObject.GetComponentInParent<CarUserControl>().isTopCar;
        boostTimer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        //only resetting to beginnning right now.
        //move to last checkpoint
        if (isBoosting && Time.time - boostTimer > 5)
            isBoosting = false;
        bool resetting = false;
        if ((Input.GetKeyDown(KeyCode.LeftShift) && !isCarTop) || (Input.GetKeyDown(KeyCode.RightShift) && isCarTop))
        {
            isBoosting = true;
            boostTimer = Time.time;
        }


        if (Input.GetKeyDown(KeyCode.Alpha1) && !isCarTop)
        {
            resetting = true;
        } else if (Input.GetKeyDown(KeyCode.Alpha2) && isCarTop)
        {
            resetting = true;
        }
        if (resetting)
        {
            if(isCarTop)
                gameObject.GetComponentInParent<Transform>().position = new Vector3(3438, 0, 1961);
            else
                gameObject.GetComponentInParent<Transform>().position = new Vector3(3438, 0, 1965);

            //this is only lowering him to ~1-5 right now. could polish this.
            gameObject.GetComponentInParent<CarController>().zeroSpeed();
        }
    }
}
