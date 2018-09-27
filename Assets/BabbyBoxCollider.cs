using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 bounds;

	void Awake(){
		bounds = transform.lossyScale.ToVec2()/2;
	}

	public override bool Overlapping(Vector2 _point){
		return false;
	}

	public override Vector2 ClosestPoint(Vector2 _point){
		return Vector2.zero;
	}

	protected override void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireCube(transform.position, transform.lossyScale.ToVec2());
	}
}