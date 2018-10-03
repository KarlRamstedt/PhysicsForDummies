using UnityEngine;

public class ToggleGravityWithMouse : MonoBehaviour {
	
	void Start(){
		CollisionManager.gravity = Vector2.zero; //Make sure useGravity doesn't add normal gravity
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
					rabies[i].useGravity = !rabies[i].useGravity;
					break;
				}
			}
		}
	}
}
