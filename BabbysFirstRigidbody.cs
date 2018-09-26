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
	Vector2 forces = Vector2.zero;
	bool kinematic = false;

	void Awake(){
		CollisionManager.RegisterRigidbody(this);
	}
	void OnDestroy(){
		CollisionManager.DeRegisterRigidbody(this);
	}

	public void AddForce(Vector2 _force, ForceMode _mode){ //F = mass * meters/seconds^2
		switch (_mode){
			case ForceMode.Acceleration:
				velocity += _force * Time.fixedDeltaTime; //Correct?
				break;
			case ForceMode.VelocityChange:
				velocity += _force;
				break;
			case ForceMode.Force:
				forces += _force; //Correct?
				break;
			case ForceMode.Impulse:
				forces += _force / Time.fixedDeltaTime; //Correct?
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
		velocity += forces / mass * timeDelta;
		forces = Vector2.zero;
		
		var movementDelta = velocity * timeDelta;
		transform.Translate(new Vector3(movementDelta.x, movementDelta.y, 0f));
	}
}