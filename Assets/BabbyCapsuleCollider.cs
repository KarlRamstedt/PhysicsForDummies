using UnityEngine;

public class BabbyCapsuleCollider : BabbysFirstCollider {

	public float height = 1f;
	public float radius = 0.5f;

	public override Vector2 ClosestPoint(){
		return Vector2.zero;
	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(transform.position, radius); //Ugh... don't wanna
	}
}