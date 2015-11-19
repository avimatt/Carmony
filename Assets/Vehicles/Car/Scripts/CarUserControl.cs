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

        private UserInteraction m_userInteract;
        private WheelCollider[] m_wheels;// the wheel colliders of the car
        public int first = 3; // first - player who turns right and accelerates
        public int second = 3; // second - player who turns left and brakes
       


        void Start()
        {
        }

        public void startVibration()
        {
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(1f, 1f);
            playerBInput.Vibrate(1f, 1f);
        }
        public void endVibration()
        {
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(0f, 0f);
            playerBInput.Vibrate(0f, 0f);
        }

		// Show SwapText and start Co-Routine
        public void playerSwap()
        {
            // Start pulse vibration
            startVibration();

            if (!isBottomCar)
            {
                CarmonyGUI.S.topSwapText.SetActive(true);
            }
            else
            {
                CarmonyGUI.S.bottomSwapText.SetActive(true);

            }
            StartCoroutine("swapGrow");
            //call co-routine
            StartCoroutine("pulseWait");

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
            print("Swap text closed");
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
            print("in pulse wait");

            yield return new WaitForSeconds(1);

            swapControls();
        }

		// Actually Swap the players controls
        void swapControls()
        {
            print("in swap controls");
            // Turn off vibrate
            endVibration();

			// Remove SwapText and swap control images
            if (!isBottomCar)
            {
                //CarmonyGUI.S.topSwapText.SetActive(false);
                Sprite tempImage = CarmonyGUI.S.topImageLeft.GetComponent<Image>().sprite;
                CarmonyGUI.S.topImageLeft.GetComponent<Image>().sprite = CarmonyGUI.S.topImageRight.GetComponent<Image>().sprite;
                CarmonyGUI.S.topImageRight.GetComponent<Image>().sprite = tempImage;

            }
            else
            {
                //CarmonyGUI.S.bottomSwapText.SetActive(false);
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
            m_wheels = gameObject.GetComponentsInChildren<WheelCollider>();
            m_userInteract = gameObject.GetComponentInParent<UserInteraction>();

        }

        //Modify these values in order to tweak steering
        private void FixedUpdate()
        {
            m_userInteract.moveToLocation();

            if (first >= InputManager.Devices.Count)
                return;

            if (!Main.S.getRaceStarted() && !Main.S.practicing)
            {
                m_Car.zeroXYSpeed();
                return;
            }


            // Use InControl
            // Hard code the mapping of device to player for now
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];

            float accel;
            float footbrake;
            // Player A controls accelerator and turning right
            if (Main.S.normalControls)
            {
                accel = playerAInput.RightTrigger;
                // Player B controls footbrake, handbrake, and turning left
                footbrake = -1f * playerBInput.LeftTrigger;
            }
            else
            {
                accel = playerAInput.RightTrigger;
                footbrake = -1f * playerAInput.LeftTrigger;
            }
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
            float playerA_turnRight;
            float playerB_turnLeft;
            if (Main.S.normalControls)
            {
                playerA_turnRight = Math.Max(0f, playerAInput.LeftStickX);
                playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);
            }
            else{
                playerA_turnRight = Math.Max(0f, playerBInput.LeftStickX);
                playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);
            }
            float steering = playerA_turnRight + playerB_turnLeft;
			if (gameObject.transform.localScale.x < 1) {
				steering *= Mathf.Pow(2.718f, m_Car.getSpeed()/-60);
                m_Car.SlipLimit = .7f;
                m_Car.SteerHelperProperty = 1f;
                for(int i = 0; i < 4; i++)
                {
                    WheelFrictionCurve wheelCurve = m_wheels[i].sidewaysFriction;
                    wheelCurve.asymptoteSlip = .0001f;
                    wheelCurve.extremumSlip = .0001f;
                    m_wheels[i].sidewaysFriction = wheelCurve;

                    JointSpring wheelSpring = m_wheels[i].suspensionSpring;
                    wheelSpring.damper = 3500;
                    m_wheels[i].suspensionSpring = wheelSpring;
                }
			}else if (gameObject.transform.localScale.x > 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    JointSpring wheelSpring = m_wheels[i].suspensionSpring;
                    wheelSpring.damper = 10;
                    m_wheels[i].suspensionSpring = wheelSpring;
                }
            } else {
                m_Car.SlipLimit = .3f;
                m_Car.SteerHelperProperty = .644f;
                for (int i = 0; i < 4; i++)
                {
                    WheelFrictionCurve wheelCurve = m_wheels[i].sidewaysFriction;
                    wheelCurve.asymptoteSlip = .5f;
                    wheelCurve.extremumSlip = .2f;
                    m_wheels[i].sidewaysFriction = wheelCurve;

                    JointSpring wheelSpring = m_wheels[i].suspensionSpring;
                    wheelSpring.damper = 3500;
                    m_wheels[i].suspensionSpring = wheelSpring;
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
