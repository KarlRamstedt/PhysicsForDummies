using UnityEngine;

public class BabbyBoxCollider : BabbysFirstCollider {

	public Vector2 size = new Vector2(0.5f, 0.5f); //Offsets of outer bounds

//	public override bool Overlapping(BabbysFirstCollider _otherCol){
//		return false;
//	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireCube(transform.position, size*2f);
	}
}