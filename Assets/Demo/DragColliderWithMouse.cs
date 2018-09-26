using UnityEngine;

[RequireComponent(typeof(BabbySpringJoint))]
public class DragColliderWithMouse : MonoBehaviour {

	Vector3 point;

	void Update(){
		point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;
//		var rabies = CollisionManager.rigidbodies;
//		for (int i = 0; i < rabies.Count; i++){
//			rabies[i].GetComponent<BabbysFirstCollider>().Overlapping(point);
//		}
		if (Input.GetMouseButtonDown(0)){
			GetComponent<BabbySpringJoint>().connectedBody.velocity = Vector2.zero;
			GetComponent<BabbySpringJoint>().enabled = true;
		} else if (Input.GetMouseButtonUp(0)){
			GetComponent<BabbySpringJoint>().enabled = false;
		}
		if (Input.GetMouseButton(0)){
			transform.position = point;
		}
	}
}
