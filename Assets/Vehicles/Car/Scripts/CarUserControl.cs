using System;
using UnityEngine;
using UnityEngine.UI;

using InControl;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public bool isBottomCar;

        private UserInteraction m_userInteract;
        private WheelCollider[] m_wheels;// the wheel colliders of the car
        public ParticleSystem leftSteam;
        public ParticleSystem rightSteem;
        public GameObject steeringWheel;
        public int first = 3; // first - player who turns right and accelerates
        public int second = 3; // second - player who turns left and brakes

        bool swapping;
        public GameObject monster;
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

        public void vibrateA()
        {
            var playerAInput = InputManager.Devices[first];
            playerAInput.Vibrate(1f, 1f);
        }
        public void vibrateB()
        {
            var playerBInput = InputManager.Devices[second];
            playerBInput.Vibrate(1f, 1f);
        }
        public void stopVibrateA()
        {
            var playerAInput = InputManager.Devices[first];
            playerAInput.Vibrate(0f, 0f);
            
        }

        public void stopVibrateB()
        {
            var playerBInput = InputManager.Devices[second];
            playerBInput.Vibrate(0f, 0f);
        }

        // Show SwapText and start Co-Routine
        public void playerSwap()
        {
            if (swapping)
                return;
            else
                swapping = true;
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

            StartCoroutine("turnMonster");
            //call co-routine
            StartCoroutine("pulseWait");

        }

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
            //origAngle.x = 0;
            origAngle.y += 180f;
            //origAngle.z = 0;
            //monster.transform.rotation = Quaternion.Euler(origAngle);
            monster.transform.localRotation = Quaternion.Euler(origAngle);
            print(Quaternion.Euler(origAngle));

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

            /*
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
            */
            //swap controls
            int temp = first;
            first = second;
            second = temp;

            ParticleSystem tempPart = leftSteam;
            leftSteam = rightSteem;
            rightSteem = tempPart;

         
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            m_wheels = gameObject.GetComponentsInChildren<WheelCollider>();
            m_userInteract = gameObject.GetComponentInParent<UserInteraction>();

        }

        void doubleStream(float accel,float footbrake)
        {
            if (leftSteam && rightSteem)
            {
                if (accel + footbrake > 0)
                {
                    leftSteam.startColor = new Color32(0, 255, 44, 255);
                    rightSteem.startColor = new Color32(0, 255, 44, 255);
                }
                else if (accel + footbrake < 0)
                {
                    leftSteam.startColor = new Color32(255, 0, 0, 255);
                    rightSteem.startColor = new Color32(255, 0, 0, 255);
                }
                else
                {
                    leftSteam.startColor = new Color32(255, 255, 255, 255);
                    rightSteem.startColor = new Color32(255, 255, 255, 255);
                }
            }
        }

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
            bool badA = false;
            bool badB = false;
            // Player A controls accelerator and turning right
            if (Main.S.normalControls)
            {
                accel = playerAInput.RightTrigger;
                if (playerBInput.RightTrigger)
                    badB = true;
                // Player B controls footbrake, handbrake, and turning left
                footbrake = -1f * playerBInput.LeftTrigger;
                if (playerAInput.LeftTrigger)
                    badA = true;
            }
            else
            {
                accel = playerAInput.RightTrigger;
                footbrake = -1f * playerAInput.LeftTrigger;
            }

            //doubleStream(accel, footbrake);
            singleStream(accel, footbrake);

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
                if (Math.Max(0f, playerBInput.LeftStickX) > 0)
                    badB = true;
                playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);
                if (Math.Min(0f, playerAInput.LeftStickX) < 0)
                    badA = true;
            }
            else{
                playerA_turnRight = Math.Max(0f, playerBInput.LeftStickX);
                playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);
            }
            float steering = playerA_turnRight + playerB_turnLeft;
			if (gameObject.transform.localScale.x < 1) {
                print("correcting wheels");
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
            if (first != second)
            {
                if (badA)
                {
                    vibrateA();
                }
                else
                {
                    stopVibrateA();
                }
                if (badB)
                {
                    vibrateB();
                }
                else
                {
                    stopVibrateB();
                }
            }
            float carY = transform.rotation.eulerAngles.y;
            float degree =  180 -steering * 60;

            Vector3 newRot = steeringWheel.transform.rotation.eulerAngles;
            newRot.z = degree;
            steeringWheel.transform.rotation = Quaternion.Euler(newRot);
            //steering = steering / 2f;//this is to not spin on a dime
#if !MOBILE_INPUT
            //float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            accel *= 2f;
            footbrake *= 2f;

            m_Car.Move(steering, accel, footbrake, handbrake);
#else
            m_Car.Move(steering, accel, footbrake, 0f);
#endif
        }
    }
}
