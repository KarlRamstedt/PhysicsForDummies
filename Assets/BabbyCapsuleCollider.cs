using UnityEngine;

public class BabbyCapsuleCollider : BabbysFirstCollider {

	public float height = 1f;
	public float radius = 0.5f;

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(transform.position, radius); //Ugh... don't wanna
	}
}