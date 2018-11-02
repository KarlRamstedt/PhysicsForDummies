using UnityEngine;

public class CircleCol2D : Collider2DBase {

	[SerializeField] float radius = 0.5f;

	/// <summary>
	/// Radius multiplied by largest transform scale component.
	/// </summary>
	public float Radius {
		get {
			var scale = transform.lossyScale;
			if (scale.x > scale.y){
				if (scale.x > scale.z)
					return radius * scale.x;
				else
					return radius * scale.z;
			} else if (scale.y > scale.z){
				return radius * scale.y;
			} else
				return radius * scale.z;
		}
		set { radius = value; }
	}

	void Awake(){
		print("X");
	}

	public override bool Overlapping(Vector2 _point){
		return Vector2.Distance(transform.position, _point) < Radius;
	}

	/// <summary>
	/// Returns closest point ON the circle.
	/// </summary>
	public override Vector2 ClosestPoint(Vector2 _point){
		var pos = transform.position.ToVec2();

		var dir = (_point-pos).normalized;
		return pos + dir * Radius;
	}

	protected override void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireSphere(transform.position, Radius); //Real sphere colliders also draw a billboard circle, but :shrug:
	}
}