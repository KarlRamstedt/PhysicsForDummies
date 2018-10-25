using System.Collections.Generic;
using UnityEngine;

public abstract class BabbysFirstCollider : MonoBehaviour {

	public float friction = 0.25f;
	public float bounciness = 0.25f;
	public bool isTrigger = false;

	protected virtual void OnEnable(){
		CollisionManager.Inst.RegisterCollider(this);
	}
	protected virtual void OnDisable(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.DeRegisterCollider(this);
	}

	public abstract bool Overlapping(Vector2 _point);
	public abstract Vector2 ClosestPoint(Vector2 _point);

	protected virtual void OnDrawGizmosSelected(){
		Gizmos.color = Color.green;
	}
}
