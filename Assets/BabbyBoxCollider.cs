using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 size = new Vector2(0.5f, 0.5f); //Offsets of outer bounds

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireCube(transform.position, size*2f);
	}
}