using UnityEngine;

public class SetRBVelocityWithMouse : MonoBehaviour {

	public BabbysFirstRigidbody rb;
	LineRenderer lr;
	Vector3 point;

	void Start(){
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 2;
	}

	void Update(){
		point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;
		lr.SetPosition(0, rb.transform.position);
		lr.SetPosition(1, point);
		if (Input.GetMouseButtonDown(0)){
			rb.velocity = point - rb.transform.position;
		}
	}
}
