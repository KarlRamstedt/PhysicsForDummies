using UnityEngine;

public abstract class BabbysFirstCollider : MonoBehaviour {

//	public abstract bool Overlapping(BabbysFirstCollider _otherCol);

	protected virtual void OnDrawGizmos(){
		Gizmos.color = Color.green;
	}
}
