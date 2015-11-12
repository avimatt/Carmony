using UnityEngine;
using System.Collections;

public class CarObjCollision : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision coll)
    {
        print("Hit: " + coll.gameObject.name);
        print(coll.gameObject.layer);
    }
}