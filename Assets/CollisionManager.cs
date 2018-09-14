using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

	public static Vector2 gravity = new Vector2(0f, -9.82f);
//	public static float dampingFactor = 0.25f;

	static List<BabbysFirstCollider> colliders = new List<BabbysFirstCollider>();
	static List<BabbysFirstRigidbody> rigidbodies = new List<BabbysFirstRigidbody>();

	public static void RegisterCollider(BabbysFirstCollider _col){
		if (!colliders.Contains(_col))
			colliders.Add(_col);
	}
	public static void DeRegisterCollider(BabbysFirstCollider _col){
		colliders.Remove(_col);
	}
	public static void RegisterRigidbody(BabbysFirstRigidbody _col){
		if (!rigidbodies.Contains(_col))
			rigidbodies.Add(_col);
	}
	public static void DeRegisterRigidbody(BabbysFirstRigidbody _col){
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

	public static void CalculateCollision(BabbysFirstCollider _col1, BabbysFirstCollider _col2){
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
			var closestPointOnCircle1 = pos + dir * _col.radius;
			var closestPointOnCircle2 = otherPos - dir * _col2.radius;
			var collisionPoint = (closestPointOnCircle1 + closestPointOnCircle2)/2;
			var offset1 = collisionPoint - closestPointOnCircle1;
			var offset2 = collisionPoint - closestPointOnCircle2;

			var rb1 = _col.GetComponent<BabbysFirstRigidbody>();
			var rb2 = _col2.GetComponent<BabbysFirstRigidbody>();

			//https://stackoverflow.com/questions/573084/how-to-calculate-bounce-angle
			var perpendicularVelocity = Vector2.Dot(rb1.velocity, -dir)*-dir;
			var paralellVelocity = rb1.velocity - perpendicularVelocity;
			var finalFriction = 1 - (_col.friction + _col2.friction) / 2;
			if (rb1 != null && !rb1.isKinematic){
				if (rb2 != null && !rb2.isKinematic){
					var massFrac = rb1.mass / rb2.mass;
					rb1.transform.Translate(offset1 / massFrac, Space.World);
					rb1.AddForce(paralellVelocity * finalFriction - perpendicularVelocity * 2 * massFrac, ForceMode.VelocityChange);
				} else {
					rb1.transform.Translate(offset1 * 2, Space.World);
					rb1.AddForce(paralellVelocity * finalFriction - perpendicularVelocity * 2, ForceMode.VelocityChange);
				}
			} else {
//				if (rb2 != null && !rb2.isKinematic) //TODO: Change velocity properly
//					rb2.transform.Translate(offset2 / (rb2.mass / rb1.mass), Space.World);
//				else
//					rb2.transform.Translate(offset2*2, Space.World);
			}
		}
	}

	public static void BoxSphere(BabbyBoxCollider _boxCol, BabbySphereCollider _sphereCol){
		var pos = _sphereCol.transform.position.ToVec2();
		var otherPos = _boxCol.transform.position.ToVec2();

		//Check clamped outer box coordinate | ALSO CONTACT POINT || https://yal.cc/rectangle-circle-intersection-test/
		var boxContactPointX = Mathf.Max(otherPos.x, Mathf.Min(pos.x, otherPos.x + _boxCol.size.x)); //CHECK IF NEED TO CENTER
		var boxContactPointY = Mathf.Max(otherPos.y, Mathf.Min(pos.y, otherPos.y + _boxCol.size.y));
		var delta = new Vector2(pos.x - boxContactPointX, pos.y - boxContactPointY);

		if (delta.x * delta.x + delta.y * delta.y < _sphereCol.radius * _sphereCol.radius){ //If distance to contact point is smaller than circle radius then they are in contact
//			var normal = delta.normalized;
			var rb1 = _boxCol.GetComponent<BabbysFirstRigidbody>();
			var rb2 = _sphereCol.GetComponent<BabbysFirstRigidbody>();

			var normal = delta.normalized;
			var finalFriction = 1 - (_boxCol.friction + _sphereCol.friction) / 2;

			if (rb1 != null && !rb1.isKinematic){
				var perpendicularVelocity = Vector2.Dot(rb1.velocity, normal)*normal;
				var paralellVelocity = rb1.velocity - perpendicularVelocity;
				if (rb2 != null && !rb2.isKinematic){
					var massFrac = rb1.mass / rb2.mass;
//					rb1.transform.Translate(offset1 / massFrac, Space.World);
					rb1.AddForce(paralellVelocity * finalFriction - perpendicularVelocity * 2 * massFrac, ForceMode.VelocityChange);
				} else {
//					rb1.transform.Translate(offset1 * 2, Space.World);
					rb1.AddForce(paralellVelocity * finalFriction - perpendicularVelocity * 2, ForceMode.VelocityChange);
				}
			} else {
				var perpendicularVelocity = Vector2.Dot(rb1.velocity, normal)*normal;
				var paralellVelocity = rb1.velocity - perpendicularVelocity;
				//				if (rb2 != null && !rb2.isKinematic) //TODO: Change velocity properly
				//					rb2.transform.Translate(offset2 / (rb2.mass / rb1.mass), Space.World);
				//				else
				//					rb2.transform.Translate(offset2*2, Space.World);
			}
		}
	}

	bool PointOverlapBox(Vector2 _point, BabbyBoxCollider _boxCol){
		var insideX = _boxCol.transform.position.x - _boxCol.size.x < _point.x && _point.x < _boxCol.transform.position.x + _boxCol.size.x;
		var insideY = _boxCol.transform.position.y-_boxCol.size.y < _point.y && _point.y < _boxCol.transform.position.y+_boxCol.size.y;
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