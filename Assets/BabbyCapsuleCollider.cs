using UnityEngine;

public class BabbyCapsuleCollider : BabbysFirstCollider {

	public float height = 1f;
	public float radius = 0.5f;

//	public override bool Overlapping(BabbysFirstCollider _otherCol){
//		return false;
//	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(transform.position, radius); //Ugh... don't wanna
	}
}