using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using InControl;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public bool isTopCar;

        public int first = 3; // first - player who turns right and accelerates
        public int second = 3; // second - player who turns left and brakes
       

        void Start()
        {
        }


        public void playerSwap()
        {
            print("in player swap");
            //Start pulse vibration
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(1f, 1f);
            playerBInput.Vibrate(1f, 1f);

            //call co-routine
            StartCoroutine("pulseWait");

        }

        IEnumerator pulseWait()
        {
            print("in pulse wait");

            yield return new WaitForSeconds(1);

            swapControls();
        }

        void swapControls()
        {
            print("in swap controls");
            //turn off vibrate
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];
            playerAInput.Vibrate(0f, 0f);
            playerBInput.Vibrate(0f, 0f);

            //swap controls
            int temp = first;
            first = second;
            second = temp;

            //swap images
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        //Modify these values in order to tweak steering
        private void FixedUpdate()
        {

            if (Time.realtimeSinceStartup < 1)
                return;
            if (first >= InputManager.Devices.Count)
                return;
            // Use InControl
            // Hard code the mapping of device to player for now
            var playerAInput = InputManager.Devices[first];
            var playerBInput = InputManager.Devices[second];

            // Player A controls accelerator and turning right
            float accel = playerAInput.RightTrigger;
            // Player B controls footbrake, handbrake, and turning left
            float footbrake = -1f * playerBInput.LeftTrigger;
            //Turning off handbrake because we are already using that button
            // for the powerup character sequence input.
            float handbrake = 0;
            float playerA_turnRight = Math.Max(0f, playerAInput.LeftStickX);
            float playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);

            float steering = playerA_turnRight + playerB_turnLeft;
            // pass the input to the car!
            if (!isTopCar)
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
