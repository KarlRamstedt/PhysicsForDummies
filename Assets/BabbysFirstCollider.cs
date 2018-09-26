using UnityEngine;

public abstract class BabbysFirstCollider : MonoBehaviour {

	public float friction = 0.25f;
	public float bounciness = 0.25f;

	protected virtual void Awake(){
		CollisionManager.RegisterCollider(this);
	}
	protected virtual void OnDestroy(){
		CollisionManager.DeRegisterCollider(this);
	}

	public abstract Vector2 ClosestPoint();

	protected virtual void OnDrawGizmos(){
		Gizmos.color = Color.green;
	}
}
