using UnityEngine;

public class BabbysFirstRigidbody : MonoBehaviour {

	public Vector2 velocity = Vector2.zero;
	public float mass = 1f;
	public bool useGravity = true;
	public bool isKinematic {
		get { return kinematic; }
		set {
			if (value){
				velocity = Vector2.zero;
			}
			kinematic = value;
		}
	}
	bool kinematic = false;

	void Awake(){
		CollisionManager.RegisterRigidbody(this);
	}
	void OnDestroy(){
		CollisionManager.DeRegisterRigidbody(this);
	}

	public void UpdatePosition(){
		if (kinematic)
			return;
		var timeDelta = Time.fixedDeltaTime;
		if (useGravity)
			velocity += CollisionManager.gravity * timeDelta; //Convert gravity acceleration to velocity and add to total velocity
		
		var movementDelta = velocity * timeDelta;
		transform.Translate(new Vector3(movementDelta.x, movementDelta.y, 0f));
	}
}