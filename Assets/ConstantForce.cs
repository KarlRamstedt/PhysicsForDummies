using UnityEngine;

[RequireComponent(typeof(RigidBod2D))]
public class ConstantForce : MonoBehaviour {

	public Vector2 force = Vector2.zero;

	RigidBod2D rb;
	void Awake(){
		rb = GetComponent<RigidBod2D>();
	}

	void FixedUpdate(){
		rb.AddForce(force);
	}
}
