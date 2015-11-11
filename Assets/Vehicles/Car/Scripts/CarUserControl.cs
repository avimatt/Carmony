using System;
using UnityEngine;
using UnityEngine.UI;

using InControl;
using System.Collections;
using XInputDotNetPure;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public bool isBottomCar;

        public int first = 3; // first - player who turns right and accelerates
        public int second = 3; // second - player who turns left and brakes
       

        void Start()
        {
        }

		// Show SwapText and start Co-Routine
        public void playerSwap()
        {
            print("in player swap");
            // Start pulse vibration
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(1f, 1f);
            playerBInput.Vibrate(1f, 1f);

            //GamePad.SetVibration((PlayerIndex)first, 1f, 1f);
            //GamePad.SetVibration((PlayerIndex)second, 1f, 1f);
            if (!isBottomCar)
            {
                CarmonyGUI.S.topSwapText.SetActive(true);
            }
            else
            {
                CarmonyGUI.S.bottomSwapText.SetActive(true);

            }
            //call co-routine
            StartCoroutine("pulseWait");

        }

        IEnumerator pulseWait()
        {
            print("in pulse wait");

            yield return new WaitForSeconds(1);

            swapControls();
        }

		// Actually Swap the players controls
        void swapControls()
        {
            print("in swap controls");
            // Turn off vibrate
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(0f, 0f);
            playerBInput.Vibrate(0f, 0f);
            //GamePad.SetVibration((PlayerIndex)first, 0f, 0f);
            //GamePad.SetVibration((PlayerIndex)second, 0f, 0f);


			// Remove SwapText and swap control images
            if (!isBottomCar)
            {
                CarmonyGUI.S.topSwapText.SetActive(false);
                Sprite tempImage = CarmonyGUI.S.topImageLeft.GetComponent<Image>().sprite;
                CarmonyGUI.S.topImageLeft.GetComponent<Image>().sprite = CarmonyGUI.S.topImageRight.GetComponent<Image>().sprite;
                CarmonyGUI.S.topImageRight.GetComponent<Image>().sprite = tempImage;

            }
            else
            {
                CarmonyGUI.S.bottomSwapText.SetActive(false);
                Sprite tempImage = CarmonyGUI.S.bottomImageLeft.GetComponent<Image>().sprite;
                CarmonyGUI.S.bottomImageLeft.GetComponent<Image>().sprite = CarmonyGUI.S.bottomImageRight.GetComponent<Image>().sprite;
                CarmonyGUI.S.bottomImageRight.GetComponent<Image>().sprite = tempImage;
            }

            //swap controls
            int temp = first;
            first = second;
            second = temp;
         
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        //Modify these values in order to tweak steering
        private void FixedUpdate()
        {
            gameObject.GetComponentInParent<UserInteraction>().moveToLocation();

            if (first >= InputManager.Devices.Count)
                return;

            if (!Main.S.getRaceStarted() && !Main.S.practicing)
            {
                gameObject.GetComponentInParent<CarController>().zeroXYSpeed();
                return;
            }


            // Use InControl
            // Hard code the mapping of device to player for now
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];

            // Player A controls accelerator and turning right
            float accel = playerAInput.RightTrigger;
            // Player B controls footbrake, handbrake, and turning left
            float footbrake = -1f * playerBInput.LeftTrigger;
            if (accel + footbrake > 0)
            {
                accel = accel + footbrake;
                footbrake = 0;
            }
            else
            {
                footbrake = accel + footbrake;
                accel = 0;
            }
            //Turning off handbrake because we are already using that button
            // for the powerup character sequence input.
            float handbrake = 0;

            float playerA_turnRight = Math.Max(0f, playerAInput.LeftStickX);
            float playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);

            float steering = playerA_turnRight + playerB_turnLeft;
			if (gameObject.transform.localScale.x < 1) {
				steering *= Mathf.Pow(2.718f, gameObject.GetComponent<CarController> ().getSpeed()/-60); 
				gameObject.GetComponent<CarController> ().SlipLimit = .7f;
				gameObject.GetComponent<CarController> ().SteerHelperProperty = 1f;
                WheelCollider[] wheels = gameObject.GetComponentsInChildren<WheelCollider>();
                for(int i = 0; i < 4; i++)
                {
                    WheelFrictionCurve wheelCurve = wheels[i].sidewaysFriction;
                    wheelCurve.asymptoteSlip = .0001f;
                    wheelCurve.extremumSlip = .0001f;
                    wheels[i].sidewaysFriction = wheelCurve;

                    JointSpring wheelSpring = wheels[i].suspensionSpring;
                    wheelSpring.damper = 3500;
                    wheels[i].suspensionSpring = wheelSpring;
                }
			}else if (gameObject.transform.localScale.x > 1)
            {
                WheelCollider[] wheels = gameObject.GetComponentsInChildren<WheelCollider>();
                for (int i = 0; i < 4; i++)
                {
                    JointSpring wheelSpring = wheels[i].suspensionSpring;
                    wheelSpring.damper = 10;
                    wheels[i].suspensionSpring = wheelSpring;
                }
            } else {
				gameObject.GetComponent<CarController> ().SlipLimit = .3f;
				gameObject.GetComponent<CarController> ().SteerHelperProperty = .644f;
                WheelCollider[] wheels = gameObject.GetComponentsInChildren<WheelCollider>();
                for (int i = 0; i < 4; i++)
                {
                    WheelFrictionCurve wheelCurve = wheels[i].sidewaysFriction;
                    wheelCurve.asymptoteSlip = .5f;
                    wheelCurve.extremumSlip = .2f;
                    wheels[i].sidewaysFriction = wheelCurve;

                    JointSpring wheelSpring = wheels[i].suspensionSpring;
                    wheelSpring.damper = 3500;
                    wheels[i].suspensionSpring = wheelSpring;
                }
            }

            // pass the input to the car!
            if (!isBottomCar)
            {
                if (Main.S.carTopDone)
                {
                    m_Car.Move(0f, 0f, 0f, 0f);
                    return;
                }
            }
            else
            {
                if (Main.S.carBottomDone)
                {
                    m_Car.Move(0f, 0f, 0f, 0f);
                    return;
                }
            }
#if !MOBILE_INPUT
            //float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(steering, accel, footbrake, handbrake);
#else
            m_Car.Move(steering, accel, footbrake, 0f);
#endif
        }
    }
}
