using UnityEngine;

[RequireComponent(typeof(BabbySpringJoint))]
public class DragColliderWithMouse : MonoBehaviour {

	BabbySpringJoint joint;
	Camera cam;

	void Start(){
		cam = Camera.main;
		GetComponent<RigidBod2D>().isKinematic = true;
		joint = GetComponent<BabbySpringJoint>();
		joint.enabled = false;
	}

	void Update(){
		var point = cam.ScreenToWorldPoint(Input.mousePosition);
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
			transform.position = point;
		}
	}
}
