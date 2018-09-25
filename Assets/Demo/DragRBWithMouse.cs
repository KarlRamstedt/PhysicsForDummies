using UnityEngine;

public class DragRBWithMouse : MonoBehaviour {
	
	Vector3 point;

	void Update(){
		point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;
		var rabies = CollisionManager.rigidbodies;
//		for (int i = 0; i < rabies.Count; i++){
//			rabies[i].GetComponent<BabbysFirstCollider>().Overlapping(point);
//		}
		var rb = GetComponent<BabbysFirstRigidbody>();
		if (Input.GetMouseButtonDown(0)){
			rb.transform.position = point;
		}
	}
}
