using UnityEngine;

public abstract class BabbysFirstCollider : MonoBehaviour {

	protected virtual void Awake(){
		CollisionManager.RegisterCollider(this);
	}
	protected virtual void OnDestroy(){
		CollisionManager.DeRegisterCollider(this);
	}

	protected virtual void OnDrawGizmos(){
		Gizmos.color = Color.green;
	}
}
