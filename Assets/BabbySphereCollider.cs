using UnityEngine;

public class BabbySphereCollider : BabbysFirstCollider {

	public float radius = 0.5f; //The only relevant physics part, rest is just for drawing the circle

	public override Vector2 ClosestPoint(){
		return Vector2.zero;
	}

//	public override bool Overlapping(BabbysFirstCollider _otherCol){
//		if (_otherCol.GetType() == typeof(BabbyBoxCollider)){
//			var otherBox = _otherCol as BabbyBoxCollider;
//			var pos = transform.position.ToVec2();
//			var otherPos = _otherCol.transform.position.ToVec2();
//
//			//Check clamped outer box coordinate 
//			var deltaX = pos.x - Mathf.Max(otherPos.x, Mathf.Min(pos.x, otherPos.x + otherBox.scale.x)); //THIS IS ALSO CONTACT POINT
//			var deltaY = pos.y - Mathf.Max(otherPos.y, Mathf.Min(pos.y, otherPos.y + otherBox.scale.y));
//
//			return deltaX * deltaX + deltaY * deltaY < radius * radius;
//
//			var direction = (new Vector2(deltaX, deltaY) - pos).normalized; //DIRECTION TO OFFSET FROM CONTACT POINT
//
//		} else if (_otherCol.GetType() == typeof(BabbySphereCollider)){
//			var otherSphere = _otherCol as BabbySphereCollider;
//			var pos = transform.position.ToVec2();
//			var otherPos = _otherCol.transform.position.ToVec2();
//
//			return pos.Distance(otherPos) < radius + otherSphere.radius;
//
//		} else if (_otherCol.GetType() == typeof(BabbyCapsuleCollider)){
//			return false;
//		} else if (_otherCol.GetType() == typeof(BabbyMeshCollider)){
//			return false;
//		} else
//			throw new System.InvalidOperationException("Invalid Collider type.");
//	}

	protected override void OnDrawGizmos(){
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(transform.position, radius); //Real sphere colliders also draw a billboard circle, but :shrug:
	}
}