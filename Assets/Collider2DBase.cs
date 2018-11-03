using System.Collections.Generic;
using UnityEngine;

//"Base" to avoid conflict with Unity's own Collider class.
public abstract class Collider2DBase : MonoBehaviour {

	[Range(0f, 1f)] public float friction = 0.25f;
	[Range(0f, 1f)] public float bounciness = 0.25f;
	public bool isTrigger = false;

	[HideInInspector] public new Transform transform; //Caching for performance
	protected virtual void Awake(){
		transform = GetComponent<Transform>();
	}

	protected virtual void OnEnable(){
		CollisionManager.Inst.RegisterComponent(this);
	}
	protected virtual void OnDisable(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.DeRegisterComponent(this);
	}

	public abstract bool Overlapping(Vector2 _point);
	public abstract Vector2 ClosestPoint(Vector2 _point);

	protected virtual void OnDrawGizmosSelected(){
		transform = GetComponent<Transform>();
		Gizmos.color = Color.green;
	}
}
