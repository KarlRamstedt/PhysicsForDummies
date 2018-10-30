using System.Collections.Generic;
using UnityEngine;

//"Base" to avoid conflict with Unity's own Collider class.
public abstract class Collider2DBase : MonoBehaviour {

	[Range(0f, 1f)] public float friction = 0.25f;
	[Range(0f, 1f)] public float bounciness = 0.25f;
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
