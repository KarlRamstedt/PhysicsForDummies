using UnityEngine;

public class BoxCol2D : Collider2DBase {

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

	/// <summary>
	/// Returns closest point ON box.
	/// </summary>
	public override Vector2 ClosestPoint(Vector2 _point){
		if (Overlapping(_point)){
			return PointClampedToBox(_point);
		} else { //TODO: Set properly
			return ClosestInsideOut(_point);
		}
	}

	public Vector2 PointClampedToBox(Vector2 _point){ //https://yal.cc/rectangle-circle-intersection-test/
		var pos = transform.position.ToVec2();
		var bounds = Bounds;
		Vector2 boxContactPoint;
		boxContactPoint.x = Mathf.Max(pos.x-bounds.x, Mathf.Min(_point.x, pos.x + bounds.x));
		boxContactPoint.y = Mathf.Max(pos.y-bounds.y, Mathf.Min(_point.y, pos.y + bounds.y));
		return boxContactPoint;
	}

	public Vector2 ClosestInsideOut(Vector2 _point){ //TODO: Finish implementing | https://math.stackexchange.com/questions/356792/how-to-find-nearest-point-on-line-of-rectangle-from-anywhere
		var pos = transform.position.ToVec2();
		var bounds = Bounds;

		var distanceToPositiveBounds = pos+bounds - _point;
		var distanceToNegativeBounds = pos-bounds - _point;
		if (distanceToPositiveBounds.x < distanceToNegativeBounds.x){
			return pos;
		} else {
			return pos;
		}
	}

	protected override void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireCube(transform.position, transform.lossyScale.ToVec2());
	}
}