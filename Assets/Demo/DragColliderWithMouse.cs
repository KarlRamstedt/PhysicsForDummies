using UnityEngine;

[RequireComponent(typeof(BabbySpringJoint))]
public class DragColliderWithMouse : MonoBehaviour {

	BabbySpringJoint joint;

	void Start(){
		GetComponent<RigidBod2D>().isKinematic = true;
		joint = GetComponent<BabbySpringJoint>();
		joint.enabled = false;
	}

	void Update(){
		var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;

		if (Input.GetMouseButtonDown(0)){
			var rabies = CollisionManager.Inst.rigidbodies;
			for (int i = 0; i < rabies.Count; i++){
				if (rabies[i].GetComponent<Collider2DBase>() == null)
					continue;
				if (rabies[i].GetComponent<Collider2DBase>().Overlapping(point)){
					joint.connectedBody = rabies[i].GetComponent<RigidBod2D>();
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
