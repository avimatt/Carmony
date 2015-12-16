using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	public Transform		poi;
	public Transform		camTarget;
	public float			distance = 5, height = 1;
	public float			u = 0.1f;

	void FixedUpdate () {
		Vector3 pos = poi.position;
		pos -= poi.forward * distance;
		pos += poi.up * height;

		Vector3 pos2 = (1-u)*transform.position + u*pos;
		transform.position = pos2;

		transform.LookAt(camTarget);
	}
}
