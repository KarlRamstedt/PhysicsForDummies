using UnityEngine;

//The most rigid of bods.
[DisallowMultipleComponent]
public class RigidBod2D : MonoBehaviour {

	public Vector2 force = Vector2.zero;
//	public Vector2 angularVelocity = Vector2.zero; //TODO: Implement D:
	public float mass = 1f;
	public bool useGravity = true;
	public bool isKinematic = false;

	public Vector2 velocity { //Verlet integration
		get { return (transform.position.ToVec2()-previousPosition) / Time.fixedDeltaTime; } //V = (pos-prevPos)/T
		set { previousPosition = transform.position.ToVec2() - value * Time.fixedDeltaTime; } //prevPos = pos - V*T
	}
	public Vector2 previousPosition;

	void Awake(){
		velocity = force; //Verlet isn't self-starting | Easier to just treat starting force value as starting velocity | / mass * Time.fixedDeltaTime
	}

	void OnEnable(){
		CollisionManager.Inst.RegisterComponent(this);
	}
	void OnDisable(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.DeRegisterComponent(this);
	}

	public void Move(Vector2 _movementDelta){
		transform.Translate(_movementDelta.ToVec3());
	}

	public void AddForce(Vector2 _force){
		force += _force;
	}
	public void AddForce(float x, float y){
		force.x += x;
		force.y += y;
	}
	public void AddForce(Vector2 _force, ForceMode _mode){ //F = mass * meters/seconds^2
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
		
		var newPos = transform.position.ToVec2() * 2 - previousPosition + force * timeDelta*timeDelta / mass;
		force = Vector2.zero;

		previousPosition = transform.position;
		transform.position = newPos.ToVec3();
	}
}

//TODO: more robust verlet integration (for "UpdatePosition" method):
//1. Advance positions by 0.5dT
//2. Apply half collision/friction
//3. Compute(add?) forces
//4. Advance velocities by dT
//5. Calculate collision/friction
//6. Apply half collision/friction
//7. Advance positions by 0.5dT

//Source: http://www.scitepress.org/Papers/2017/62693/62693.pdf
//Could allegedly remove the need for multiple solverIterations.