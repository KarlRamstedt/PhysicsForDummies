using UnityEngine;

/// <summary>
/// Named to avoid conflict with Unity's ConstantForce component.
/// </summary>
[RequireComponent(typeof(RigidBod2D))]
public class ConstForce : ForceGenerator {

	public Vector2 force = Vector2.zero;

	RigidBod2D rb;
	void Awake(){
		rb = GetComponent<RigidBod2D>();
	}

	public override void GenerateForce(){
		rb.AddForce(force);
	}
}
