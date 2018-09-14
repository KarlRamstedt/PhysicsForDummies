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

	public void AddForce(Vector2 _force, ForceMode _mode){
		switch (_mode){
			case ForceMode.Acceleration:
				velocity += _force * Time.fixedDeltaTime; //Correct?
				break;
			case ForceMode.Force:
				velocity += (_force / mass) * Time.fixedDeltaTime; //Correct?
				break;
			case ForceMode.Impulse:
				velocity += _force / mass; //Correct?
				break;
			case ForceMode.VelocityChange:
				velocity += _force;
				break;
			default:
				Debug.LogWarning("Unsupported ForceMode");
				break;
		}
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