﻿using UnityEngine;

//The most rigid of bods.
[DisallowMultipleComponent]
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

	public void Move(Vector2 _movementDelta){
		transform.Translate(_movementDelta.ToVec3());
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
				force += _force;
				break;
			case ForceMode.Impulse:
				force += _force / Time.fixedDeltaTime; //Correct?
				break;
			default:
				Debug.LogWarning("Unsupported ForceMode");
				break;
		}
	}

	public void UpdatePosition(){
//		print(velocity);
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