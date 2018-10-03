using UnityEngine;

[RequireComponent(typeof(BabbySpringJoint))]
public class DragColliderWithMouse : MonoBehaviour {

	BabbySpringJoint joint;

	void Start(){
		joint = GetComponent<BabbySpringJoint>();
		joint.enabled = false;
	}

	void Update(){
		var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;

		if (Input.GetMouseButtonDown(0)){
			var rabies = CollisionManager.Inst.rigidbodies;
			for (int i = 0; i < rabies.Count; i++){
				if (rabies[i].GetComponent<BabbysFirstCollider>() == null)
					continue;
				if (rabies[i].GetComponent<BabbysFirstCollider>().Overlapping(point)){
					joint.connectedBody = rabies[i].GetComponent<BabbysFirstRigidbody>();
					joint.enabled = true;
					break;
				}
			}
		} else if (Input.GetMouseButtonUp(0)){
			joint.enabled = false;
			joint.connectedBody = null;
		}
		if (Input.GetMouseButton(0)){
//			GetComponent<BabbySpringJoint>().connectedBody.ClearForce();
			transform.position = point;
		}
	}
}
