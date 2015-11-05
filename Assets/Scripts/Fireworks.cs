using UnityEngine;
using System.Collections;

public class Fireworks : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Main.S.carTopDone || Main.S.carBottomDone)
        {
            //gameObject.GetComponent<AudioSource>().enabled = true;
        }
	}
}
