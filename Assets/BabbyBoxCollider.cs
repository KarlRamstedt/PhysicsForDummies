using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 Bounds {
		get {
			return transform.lossyScale.ToVec2()/2;
		}
	}

	public override bool Overlapping(Vector2 _point){
		var pos = transform.position.ToVec2();
		var bounds = Bounds;
		bool insideX = pos.x - bounds.x < _point.x && _point.x < pos.x + bounds.x;
		bool insideY = pos.y - bounds.y < _point.y && _point.y < pos.y + bounds.y;
		return insideX && insideY;
	}

	public override Vector2 ClosestPoint(Vector2 _point){
		return Vector2.zero;
	}

	protected override void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireCube(transform.position, transform.lossyScale.ToVec2());
	}
}