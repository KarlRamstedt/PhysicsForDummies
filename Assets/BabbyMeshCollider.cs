using UnityEngine;

public class BabbyMeshCollider : BabbysFirstCollider {

//	public override bool Overlapping(BabbysFirstCollider _otherCol){
//		return false;
//	}

	public override bool Overlapping(Vector2 _point){
		return false;
	}

	public override Vector2 ClosestPoint(Vector2 _point){
		return Vector2.zero;
	}

	protected new void OnDrawGizmosSelected(){
		base.OnDrawGizmosSelected();
		var meh = GetComponent<MeshFilter>().mesh;
		if (meh != null)
			Gizmos.DrawWireMesh(meh, transform.position, transform.rotation, transform.lossyScale);
	}
}