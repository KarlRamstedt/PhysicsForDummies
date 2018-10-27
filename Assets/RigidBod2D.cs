using UnityEngine;

//The most rigid of bods.
public class RigidBod2D : MonoBehaviour {

	public Vector2 force = Vector2.zero;
	public Vector2 angularVelocity = Vector2.zero;
	public float mass = 1f;
	public bool useGravity = true;
	public bool isKinematic = false;

	public Vector2 velocity { //Verlet integration
		get { return (transform.position.ToVec2()-previousPosition) / Time.fixedDeltaTime; } //V = (pos-prevPos)/T
		set { previousPosition = transform.position.ToVec2() - value * Time.fixedDeltaTime; } //prevPos = pos - V*T
	}
	Vector2 previousPosition;

	public void Move(Vector2 _movementDelta){
		transform.Translate(_movementDelta.ToVec3()); //previousPosition += _movementDelta;
	}

	void Awake(){
		velocity = force; // / mass * Time.fixedDeltaTime
	}

	void OnEnable(){
		CollisionManager.Inst.RegisterRigidbody(this);
	}
	void OnDisable(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.DeRegisterRigidbody(this);
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
				force += _force; //Correct?
				break;
			case ForceMode.Impulse:
				force += _force / Time.fixedDeltaTime; //Correct?
				break;
			default:
				Debug.LogWarning("Unsupported ForceMode");
				break;
		}
	}

	public void AddForceAtPoint(Vector2 _force, ForceMode _mode, Vector2 _point){
		switch (_mode){
			case ForceMode.Acceleration:
				velocity += _force * Time.fixedDeltaTime;
				break;
			case ForceMode.VelocityChange:
				velocity += _force;
				break;
			case ForceMode.Force:
				force += _force;
				break;
			case ForceMode.Impulse:
				force += _force / Time.fixedDeltaTime;
				break;
			default:
				Debug.LogWarning("Unsupported ForceMode");
				break;
		}
	}

	public void UpdatePosition(){
		if (isKinematic){
			velocity = Vector2.zero;
			return;
		}
		var timeDelta = Time.fixedDeltaTime;
		if (useGravity)
			force += mass * CollisionManager.gravity; //F = m * a
//		var velocityDelta = forces / mass * timeDelta;
//		var movementDelta = (velocity + velocityDelta) * timeDelta;
		var newPos = transform.position.ToVec2() * 2 - previousPosition + force * timeDelta*timeDelta / mass;
		force = Vector2.zero;

		previousPosition = transform.position;
		transform.position = newPos.ToVec3();
//		transform.Translate(movementDelta.ToVec3());
	}
}