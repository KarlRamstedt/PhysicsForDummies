using UnityEngine;

public class CircleCol2D : Collider2DBase {

	public float radius = 0.5f; //The only relevant physics part, rest is just for drawing the circle

	public override bool Overlapping(Vector2 _point){
		return Vector2.Distance(transform.position, _point) < radius;
	}

	public override Vector2 ClosestPoint(Vector2 _point){
		var pos = transform.position.ToVec2();
		var dir = (_point-pos).normalized;
		var length = Mathf.Min(dir.magnitude, radius);
		return pos + dir * length;
	}

	protected override void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireSphere(transform.position, radius); //Real sphere colliders also draw a billboard circle, but :shrug:
	}
}