using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using InControl;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public bool isTopCar;

        public int first = 3;
        public int second = 3;
        public int third = 3;
        public int fourth = 3;

        void Start()
        {
            //first = 3;
            //second = 3;
            //third = 3;
            //fourth = 3;
        }


        public void playerSwap()
        {
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

            if (Time.realtimeSinceStartup < 1)
                return;
            if (first >= InputManager.Devices.Count)
                return;
            // Use InControl
            // Hard code the mapping of device to player for now
            var playerAInput = InputManager.Devices[first];
            //if (InputManager.Devices.Count == 1 && isTopCar)
            //    second = first;
            //if (InputManager.Devices.Count == 3 && !isTopCar)
            //    second = first;
            var playerBInput = InputManager.Devices[second];
            //var playerCInput = InputManager.Devices[third];
            //var playerDInput = InputManager.Devices[fourth];

            // Comments below are to debug the controller configuration,
            // prints out information to console for use to debug controller
            // setup issues
            //int numDevices = InputManager.Devices.Count;
            //print(numDevices);
            //for (int i = 0; i < numDevices; ++i)
            //{
            //    print(InputManager.Devices[i].Name);
            //}

            // Player A controls accelerator and turning right
            float accel = playerAInput.RightTrigger;
            // Player B controls footbrake, handbrake, and turning left
            float footbrake = -1f * playerBInput.LeftTrigger;
            //Turning off because 
            //float handbrake = playerBInput.Action1;
            float handbrake = 0;
            float playerA_turnRight = Math.Max(0f, playerAInput.LeftStickX);
            float playerB_turnLeft = Math.Min(0f, playerBInput.LeftStickX);

            float steering = playerA_turnRight + playerB_turnLeft;
            // pass the input to the car!
            //float h = CrossPlatformInputManager.GetAxis("Horizontal2");
            //float v = CrossPlatformInputManager.GetAxis("Vertical2");

            //if (isTopCar == true)
            //{
            // Use InControl

            //h = CrossPlatformInputManager.GetAxis("Horizontal");
            //v = CrossPlatformInputManager.GetAxis("Vertical");
            //}
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
