﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

	public static Vector2 gravity = new Vector2(0f, -9.82f);
//	public static float dampingFactor = 0.25f;

	public List<BabbysFirstCollider> colliders = new List<BabbysFirstCollider>();
	public List<BabbysFirstRigidbody> rigidbodies = new List<BabbysFirstRigidbody>();

#region Singleton
	static CollisionManager instance;
	public static CollisionManager Inst {
		get {
			if (instance == null){ //Lazy-load object or create it in case somebody forgot to add it to the scene
				if (applicationIsQuitting)
					return null;
				GameObject go = new GameObject("(Singleton) CollisionManager"); //Optionally enter a more descriptive object name
				go.AddComponent<CollisionManager>(); //AddComponent runs awake function before continuing
				DontDestroyOnLoad(go);
			}
			return instance;
		}
	}
	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			throw new System.InvalidOperationException("[Singleton] More than 1 CollisionManager instance exists.");
	}
	public static bool applicationIsQuitting = false;
	void OnDestroy(){
		applicationIsQuitting = true;
	}
#endregion

	public void RegisterCollider(BabbysFirstCollider _col){
		if (!colliders.Contains(_col))
			colliders.Add(_col);
	}
	public void DeRegisterCollider(BabbysFirstCollider _col){
		colliders.Remove(_col);
	}
	public void RegisterRigidbody(BabbysFirstRigidbody _col){
		if (!rigidbodies.Contains(_col))
			rigidbodies.Add(_col);
	}
	public void DeRegisterRigidbody(BabbysFirstRigidbody _col){
		rigidbodies.Remove(_col);
	}
	
	void FixedUpdate(){ //Runs 50 times per second
		for (int i = 0, len = rigidbodies.Count; i < len; i++){
			rigidbodies[i].UpdatePosition();
		}

		for (int i = 0, len = colliders.Count; i < len; i++){
			for (int j = i+1; j < len; j++){
				CalculateCollision(colliders[i], colliders[j]);
			}
		}
	}

	public void CalculateCollision(BabbysFirstCollider _col1, BabbysFirstCollider _col2){
		if (_col1.GetType() == typeof(BabbyBoxCollider)){
			var col1 = _col1 as BabbyBoxCollider;

			if (_col2.GetType() == typeof(BabbyBoxCollider)){
				var col2 = _col2 as BabbyBoxCollider;
				BoxBox(col1, col2);
			} else if (_col2.GetType() == typeof(BabbySphereCollider)){
				var col2 = _col2 as BabbySphereCollider;
				BoxSphere(col1, col2);
			}

		} else if (_col1.GetType() == typeof(BabbySphereCollider)){
			var col1 = _col1 as BabbySphereCollider;

			if (_col2.GetType() == typeof(BabbyBoxCollider)){
				var col2 = _col2 as BabbyBoxCollider;
				BoxSphere(col2, col1);
			} else if (_col2.GetType() == typeof(BabbySphereCollider)){
				var col2 = _col2 as BabbySphereCollider;
				SphereSphere(col1, col2);
			}
		}
	}

	public static void BoxBox(BabbyBoxCollider _col, BabbyBoxCollider _col2){

	}

	public static void SphereSphere(BabbySphereCollider _col, BabbySphereCollider _col2){
		var pos = _col.transform.position.ToVec2();
		var otherPos = _col2.transform.position.ToVec2();

		if (pos.Distance(otherPos) < _col.radius + _col2.radius){ //if overlapping
			var dir = (otherPos-pos).normalized;
			var closestPointOnCircle1 = _col.ClosestPoint(otherPos);
			var closestPointOnCircle2 = _col2.ClosestPoint(pos);
			var collisionPoint = (closestPointOnCircle1 + closestPointOnCircle2)/2;
			var offset1 = collisionPoint - closestPointOnCircle1;
			var offset2 = collisionPoint - closestPointOnCircle2;

			var rb1 = _col.GetComponent<BabbysFirstRigidbody>();
			var rb2 = _col2.GetComponent<BabbysFirstRigidbody>();

			//https://stackoverflow.com/questions/573084/how-to-calculate-bounce-angle
			var perpendicularVelocity = Vector2.Dot(rb1.velocity, -dir)*-dir;
			var paralellVelocity = rb1.velocity - perpendicularVelocity;
			var finalFriction = 1 - (_col.friction + _col2.friction) / 2;
			var finalBounciness = (_col.bounciness + _col2.bounciness) / 2;
			if (rb1 != null && !rb1.isKinematic){
				if (rb2 != null && !rb2.isKinematic){
					var massFrac1 = Mathf.Clamp01(rb2.mass / rb1.mass);
					rb1.transform.Translate(offset1 * massFrac1, Space.World);
					rb1.velocity = paralellVelocity * finalFriction - perpendicularVelocity * massFrac1 * finalBounciness;
					var massFrac2 = Mathf.Clamp01(rb1.mass / rb2.mass);
					rb2.transform.Translate(offset2 * massFrac2, Space.World);
					rb2.velocity = -paralellVelocity * finalFriction + perpendicularVelocity * massFrac2 * finalBounciness; //TODO: Do proper mass-dependent calc
				} else {
					rb1.transform.Translate(offset1 * 2, Space.World);
					rb1.velocity = paralellVelocity * finalFriction - perpendicularVelocity * finalBounciness;
				}
			} else {
				if (rb2 != null && !rb2.isKinematic){
					rb2.transform.Translate(offset2 * 2, Space.World);
					rb2.velocity = paralellVelocity * finalFriction - perpendicularVelocity * finalBounciness;
				}
			}
		}
	}

	public Vector2 GetBoxContactPoint(Vector2 _boxPos, Vector2 _otherPos, BabbyBoxCollider _boxCol){
		Vector2 boxContactPoint;
		boxContactPoint.x = Mathf.Max(_boxPos.x-_boxCol.Bounds.x, Mathf.Min(_otherPos.x, _boxPos.x + _boxCol.Bounds.x)); //CHECK IF NEED TO CENTER
		boxContactPoint.y = Mathf.Max(_boxPos.y-_boxCol.Bounds.y, Mathf.Min(_otherPos.y, _boxPos.y + _boxCol.Bounds.y));
		return boxContactPoint;
	}

	public void BoxSphere(BabbyBoxCollider _boxCol, BabbySphereCollider _sphereCol){
		var spherePos = _sphereCol.transform.position.ToVec2();
		var boxPos = _boxCol.transform.position.ToVec2();

		//Check clamped outer box coordinate | ALSO CONTACT POINT || https://yal.cc/rectangle-circle-intersection-test/
		var boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol); //CHECK IF NEED TO CENTER
		var delta = spherePos - boxContactPoint; //Delta to circle

		if (delta.x * delta.x + delta.y * delta.y < _sphereCol.radius * _sphereCol.radius){ //If distance to contact point is smaller than circle radius then they are in contact
			var boxRB = _boxCol.GetComponent<BabbysFirstRigidbody>();
			var sphereRB = _sphereCol.GetComponent<BabbysFirstRigidbody>();

			var normal = delta.normalized;
			var finalFriction = 1 - (_boxCol.friction + _sphereCol.friction) / 2;
			var finalBounciness = (_boxCol.bounciness + _sphereCol.bounciness) / 2;

			Vector2 closestPointOnCircle;
			if (_boxCol.Overlapping(spherePos)){ //TODO: set differently when sphere center overlaps box
				var dir = (spherePos - boxPos).normalized;
				spherePos = dir * (_boxCol.Bounds.x + _boxCol.Bounds.y + _sphereCol.radius); //Offset sphere outside box
				boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);
				normal = (spherePos - boxContactPoint).normalized;
				closestPointOnCircle = spherePos -normal.normalized * _sphereCol.radius;
				spherePos += boxContactPoint - closestPointOnCircle;
			}

			closestPointOnCircle = spherePos - normal.normalized * _sphereCol.radius;
			var offset = boxContactPoint - closestPointOnCircle; //ALSO NORMAL? | NOT ROBUST, WILL GENERATE WRONG NUMBERS IF SPHERE CENTER OVERLAPS BOX
			Debug.DrawLine(spherePos, spherePos + offset);

			if (boxRB != null && !boxRB.isKinematic){
				if (sphereRB != null && !sphereRB.isKinematic){
					var massFrac = boxRB.mass / sphereRB.mass;
					boxRB.transform.position = spherePos + -offset * massFrac;
					//RECALC DELTA FOR CORRECT NORMAL AFTER OFFSET
					boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);

					delta = spherePos - boxContactPoint;
					normal = delta.normalized;

					var perpendicularVelocity = Vector2.Dot(-boxRB.velocity, normal)*normal;
					var paralellVelocity = -boxRB.velocity - perpendicularVelocity;

					boxRB.velocity = paralellVelocity * finalFriction - perpendicularVelocity * massFrac * finalBounciness;
				} else {
					boxRB.transform.Translate(-offset, Space.World);
					boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);
					delta = spherePos - boxContactPoint;
					normal = delta.normalized;

					var perpendicularVelocity = Vector2.Dot(-boxRB.velocity, normal)*normal;
					var paralellVelocity = -boxRB.velocity - perpendicularVelocity;

					boxRB.velocity = paralellVelocity * finalFriction - perpendicularVelocity * finalBounciness;
				}
			} else {
				if (sphereRB != null && !sphereRB.isKinematic){
					sphereRB.transform.Translate(offset, Space.World);
					boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);
					delta = spherePos - boxContactPoint;
					normal = delta.normalized;
				
					var perpendicularVelocity = Vector2.Dot(sphereRB.velocity, normal) * normal;
					var paralellVelocity = sphereRB.velocity - perpendicularVelocity;

					sphereRB.velocity = paralellVelocity * finalFriction - perpendicularVelocity * finalBounciness;
				}
			}
		}
	}

	bool PointOverlapBox(Vector2 _point, BabbyBoxCollider _boxCol){
		var insideX = _boxCol.transform.position.x - _boxCol.transform.lossyScale.x/2 < _point.x && _point.x < _boxCol.transform.position.x + _boxCol.transform.lossyScale.x/2;
		var insideY = _boxCol.transform.position.y-_boxCol.transform.lossyScale.y/2 < _point.y && _point.y < _boxCol.transform.position.y+_boxCol.transform.lossyScale.y/2;
		return insideX && insideY;
	}
}

public static class Vector2Extensions {
	/// <summary>
	/// For convenience. Not in Unity by default.
	/// </summary>
	public static float Distance(this Vector2 _from, Vector2 _to){
		var delta = _from - _to;
		return Mathf.Sqrt(delta.x*delta.x + delta.y*delta.y);
	}
	/// <summary>
	/// Simple vector2 to 3 conversion, using z = 0.
	/// </summary>
	public static Vector3 ToVec3(this Vector2 _vec2){
		return new Vector3(_vec2.x, _vec2.y, 0f);
	}
}

public static class Vector3Extensions {
	/// <summary>
	/// Simple vector3 to 2 conversion, discarding the z component.
	/// </summary>
	public static Vector2 ToVec2(this Vector3 _vec3){
		return new Vector2(_vec3.x, _vec3.y);
	}
}