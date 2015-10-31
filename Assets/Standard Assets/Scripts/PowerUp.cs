using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityStandardAssets.Vehicles.Car;

public class PowerUp : MonoBehaviour
{
    public enum powerUpType
    {
        speed,
        letters,
        swap
    }
    List<string> xboxLetters = new List<string>();
    
    public powerUpType type;
    // Use this for initialization
    void Start()
    {
        xboxLetters.Add("X");
        xboxLetters.Add("Y");
        xboxLetters.Add("B");
        xboxLetters.Add("A");

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider coll)
    {
        Destroy(gameObject);
        if (type == powerUpType.speed)
            coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().startBoost();
        else if (type == powerUpType.letters)
        {
            bool isTopScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarTop;
            List<string> letterList = getNewLetterList();
            CarmonyGUI.S.setLetters(isTopScreen, letterList);
        }else if (type == powerUpType.swap)
        {
            bool isTopScreen = !coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarTop;
            if (isTopScreen)
                coll.GetComponentInParent<Transform>().GetComponentInParent<CarUserControl>().playerTopSwap();
            else
                coll.GetComponentInParent<Transform>().GetComponentInParent<CarUserControl>().playerBottomSwap();

        }

        //coll.gameObject.GetComponent<UserInteraction>().startBoost();
    }
    List<string> getNewLetterList()
    {
        List<string> letterList = new List<string>();
        int numLetters = 4;
        for (int i = 0; i < numLetters; i++)
        {
            int randInt = Random.Range(0, 4);
            letterList.Add(xboxLetters[randInt]);
        }
        return letterList;
    }

    void OnCollisionEnter(Collision coll)
    {
        /*
        print("collision");
        GameObject collidedWith = coll.gameObject;
        print("name: " + collidedWith.name);
        if (collidedWith.tag == "Player")
        {
            print("supposed to be deleting");
            //gameObject.GetComponent<UserInteraction>().startBoost();
            Destroy(gameObject);
            return;
        }
        */
    }
}
