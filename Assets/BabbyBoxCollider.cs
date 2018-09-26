using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 bounds;

	void Awake(){
		bounds = transform.lossyScale.ToVec2()/2;
	}

	public override Vector2 ClosestPoint(){
		return Vector2.zero;
	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireCube(transform.position, transform.lossyScale.ToVec2());
	}
}