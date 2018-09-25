using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 bounds;

	void Start(){
		bounds = transform.lossyScale.ToVec2()/2;
	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireCube(transform.position, transform.lossyScale.ToVec2());
	}
}