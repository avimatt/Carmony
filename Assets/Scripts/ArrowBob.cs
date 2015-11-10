using UnityEngine;
using System.Collections;

public class ArrowBob : MonoBehaviour {

    public GameObject arrow;
    public float heightMax;
    public float heightMin;
    public bool raising;
	// Use this for initialization
	void Start () {
        heightMax = 15;
        heightMin = 11;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (arrow.transform.position.y >= heightMax)
            raising = false;
        if (arrow.transform.position.y <= heightMin)
            raising = true;
	    if (raising)
        {
            Vector3 newPos = arrow.transform.position;
            newPos.y += .08f;
            arrow.transform.position = newPos;
        }
        else
        {
            Vector3 newPos = arrow.transform.position;
            newPos.y -= .1f;
            arrow.transform.position = newPos;
        }
    }
}
