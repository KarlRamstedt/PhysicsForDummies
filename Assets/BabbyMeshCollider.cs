using UnityEngine;

public class BabbyMeshCollider : BabbysFirstCollider {

//	public override bool Overlapping(BabbysFirstCollider _otherCol){
//		return false;
//	}

	public override Vector2 ClosestPoint(){
		return Vector2.zero;
	}

	protected new void OnDrawGizmos(){
		base.OnDrawGizmos();
		var meh = GetComponent<MeshFilter>().mesh;
		if (meh != null)
			Gizmos.DrawWireMesh(meh, transform.position, transform.rotation, transform.lossyScale);
	}
}