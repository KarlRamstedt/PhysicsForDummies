using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 bounds = new Vector2(0.5f, 0.5f); //Offsets of outer bounds

	void Start(){
		bounds = transform.lossyScale.ToVec2() / 2;
	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireCube(transform.position, bounds*2f);
	}
}