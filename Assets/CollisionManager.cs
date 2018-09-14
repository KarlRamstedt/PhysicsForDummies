using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

	public static Vector2 gravity = new Vector2(0f, -9.82f);

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

		bool overlapping = pos.Distance(otherPos) < _col.radius + _col2.radius;
		if (overlapping){
			_col.GetComponent<BabbysFirstRigidbody>().velocity *= -1;
			_col2.GetComponent<BabbysFirstRigidbody>().velocity *= -1;
			//			var dir = pos - otherPos; //TODO: Do the offsets n' all that
		}
	}

	public static void BoxSphere(BabbyBoxCollider _boxCol, BabbySphereCollider _sphereCol){
		var pos = _sphereCol.transform.position.ToVec2();
		var otherPos = _boxCol.transform.position.ToVec2();

		//Check clamped outer box coordinate | ALSO CONTACT POINT
		var boxContactPointX = Mathf.Max(otherPos.x, Mathf.Min(pos.x, otherPos.x + _boxCol.size.x));
		var boxContactPointY = Mathf.Max(otherPos.y, Mathf.Min(pos.y, otherPos.y + _boxCol.size.y));
		var deltaX = pos.x - boxContactPointX;
		var deltaY = pos.y - boxContactPointY;

		if (deltaX * deltaX + deltaY * deltaY < _sphereCol.radius * _sphereCol.radius){ //If distance to contact point is smaller than circle radius then they are in contact
			var direction = (new Vector2(deltaX, deltaY)).normalized; //DIRECTION TO OFFSET FROM CONTACT POINT
		}
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
}

public static class Vector3Extensions {
	/// <summary>
	/// Simple vector3 to 2 conversion, discarding the z component.
	/// </summary>
	public static Vector2 ToVec2(this Vector3 _vec3){
		return new Vector2(_vec3.x, _vec3.y);
	}
}