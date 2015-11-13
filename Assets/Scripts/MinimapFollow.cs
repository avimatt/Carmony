using UnityEngine;
using System.Collections;

public class MinimapFollow : MonoBehaviour {

    public bool isTop;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        float xratio;
        float yratio;
        if (!isTop)
        {
            xratio = (Main.S.carBottom.transform.position.x - Main.S.Map.GetComponent<Map>().x) / 500f;
            yratio = (Main.S.carBottom.transform.position.z - Main.S.Map.GetComponent<Map>().z) / 500f;
        }
        else
        {
            xratio = (Main.S.carTop.transform.position.x - Main.S.Map.GetComponent<Map>().x) / 500f;
            yratio = (Main.S.carTop.transform.position.z - Main.S.Map.GetComponent<Map>().z) / 500f;
        }
        updateCar(xratio, yratio);
    }
    void updateCar(float xratio,float yratio)
    {
        float xpos = (float)Screen.width * .2f;
        float ypos = (float)Screen.height * .26f;

        Vector3 newPos = gameObject.transform.position;
        newPos.x = xpos * xratio;
        newPos.y = ypos * yratio;
        gameObject.transform.localPosition = newPos;

    }

}
