using UnityEngine;

public class BabbyCapsuleCollider : BabbysFirstCollider {

	public float height = 1f;
	public float radius = 0.5f;

	public override bool Overlapping(Vector2 _point){
		return false;
	}

	public override Vector2 ClosestPoint(Vector2 _point){
		return Vector2.zero;
	}

	protected override void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireSphere(transform.position, radius); //Ugh... don't wanna
	}
}