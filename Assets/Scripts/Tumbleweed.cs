using UnityEngine;
using System.Collections;



public class Tumbleweed : MonoBehaviour {

    public GameObject tumbleweedPrefab;
    public int zPosEnd;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float randz = Random.Range(0f, 1f);
        gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -randz));
        if (gameObject.transform.position.z < zPosEnd)
        {
            int randX = Random.Range(40, 58);
            GameObject newObj = Instantiate(tumbleweedPrefab, new Vector3(randX,.5f,2180),gameObject.transform.rotation) as GameObject;
            newObj.GetComponent<Tumbleweed>().tumbleweedPrefab = tumbleweedPrefab;
            newObj.GetComponent<Tumbleweed>().zPosEnd = zPosEnd;
            Destroy(gameObject);
        }
    }
}
