using UnityEngine;

public class ToggleGravityWithMouse : MonoBehaviour {

	Vector2 oldGravity;

	void Start(){
		oldGravity = CollisionManager.gravity;
		CollisionManager.gravity = Vector2.zero; //Make sure useGravity doesn't add normal gravity
	}
	void OnDestroy(){
		CollisionManager.gravity = oldGravity;
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
					rabies[i].useGravity = !rabies[i].useGravity;
					break;
				}
			}
		}
	}
}
