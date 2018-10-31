using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiated on demand, don't place in scene.
/// </summary>
public class CollisionManager : MonoBehaviour {

	public static Vector2 gravity = new Vector2(0f, -9.82f);

	public List<Collider2DBase> colliders = new List<Collider2DBase>();
	public List<RigidBod2D> rigidbodies = new List<RigidBod2D>();

//	List<Collision> collisionData = new List<Collision>();
	Dictionary<GameObject, List<GameObject>> collisions = new Dictionary<GameObject, List<GameObject>>(); //Does order affect result? | CollisionExit will not run if object is disabled, is that a problem?

#region Singleton
	static CollisionManager instance;
	public static CollisionManager Inst {
		get {
			if (instance == null){ //Lazy-load object or create it in case somebody forgot to add it to the scene
				if (ApplicationIsQuitting)
					return null;
				GameObject go = new GameObject("(Singleton) CollisionManager");
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
	public static bool ApplicationIsQuitting { get; private set; }
	void OnDestroy(){
		ApplicationIsQuitting = true;
	}
#endregion

	public void RegisterCollider(Collider2DBase _col){
		if (!colliders.Contains(_col))
			colliders.Add(_col);
	}
	public void DeRegisterCollider(Collider2DBase _col){
		colliders.Remove(_col);
	}
	public void RegisterRigidbody(RigidBod2D _col){
		if (!rigidbodies.Contains(_col))
			rigidbodies.Add(_col);
	}
	public void DeRegisterRigidbody(RigidBod2D _col){
		rigidbodies.Remove(_col);
	}
	
	void FixedUpdate(){ //Runs 50 times per second
		for (int i = 0, len = rigidbodies.Count; i < len; i++){
			rigidbodies[i].UpdatePosition();
		}

		for (int solverIterations = 0; solverIterations < 9; solverIterations++){ //Improves accuracy but makes callbacks run multiple times
			for (int i = 0, len = colliders.Count; i < len; i++){
				for (int j = i+1; j < len; j++){
					CalculateCollision(colliders[i], colliders[j]);
				}
			}
		}
	}

	void CalculateCollision(Collider2DBase _col1, Collider2DBase _col2){
		if (_col1 is BoxCol2D){
			var col1 = _col1 as BoxCol2D;

			if (_col2 is BoxCol2D){
				var col2 = _col2 as BoxCol2D;
				BoxBox(col1, col2);
			} else if (_col2 is CircleCol2D){
				var col2 = _col2 as CircleCol2D;
				BoxCircle(col1, col2);
			}

		} else if (_col1 is CircleCol2D){
			var col1 = _col1 as CircleCol2D;

			if (_col2 is BoxCol2D){
				var col2 = _col2 as BoxCol2D;
				BoxCircle(col2, col1);
			} else if (_col2 is CircleCol2D){
				var col2 = _col2 as CircleCol2D;
				CircleCircle(col1, col2);
			}
		}
	}

	void BoxBox(BoxCol2D _col, BoxCol2D _col2){

	}

	void CircleCircle(CircleCol2D _col, CircleCol2D _col2){
		var pos = _col.transform.position.ToVec2();
		var otherPos = _col2.transform.position.ToVec2();

		if (pos.Distance(otherPos) < _col.Radius + _col2.Radius){ //if overlapping
//			CallbackCheck(_col, _col2);
			if (_col.isTrigger || _col2.isTrigger)
				return;
			
			var dir = (otherPos - pos).normalized;
			var closestPointOnCircle1 = _col.ClosestPoint(otherPos);
			var closestPointOnCircle2 = _col2.ClosestPoint(pos);
			var collisionPoint = (closestPointOnCircle1 + closestPointOnCircle2) / 2;
			var offset1 = collisionPoint - closestPointOnCircle1;
			var offset2 = collisionPoint - closestPointOnCircle2;

			Debug.DrawLine(collisionPoint, collisionPoint + dir, Color.green);

			var rb1 = _col.GetComponent<RigidBod2D>();
			var rb2 = _col2.GetComponent<RigidBod2D>();

			//https://stackoverflow.com/questions/573084/how-to-calculate-bounce-angle

			var avgFriction = Mathf.Clamp01(1 - (_col.friction + _col2.friction) / 2);
			var avgBounciness = Mathf.Clamp01((_col.bounciness + _col2.bounciness) / 2);

			if (rb1 != null && !rb1.isKinematic){
				if (rb2 != null && !rb2.isKinematic){
					var massFraction = rb1.mass / (rb1.mass+rb2.mass); //Simply constraining will make in-elastic collisions work great with verlet integration. Elasticity is ignored tho
					rb1.Move(offset1 * (1-massFraction));
					rb2.Move(offset2 * massFraction);

//					var separatingVelocity = Vector2.Dot(rb1.velocity - rb2.velocity, dir);
//					if (separatingVelocity > 0)
//						return;
//					var newSepVelocity = -separatingVelocity * avgBounciness; //Bounce = restitution?
//					var deltaVelocity = newSepVelocity - separatingVelocity;
//					var impulse = deltaVelocity / (1/rb1.mass + 1/rb2.mass);
//					var impulsePerIMass = dir * impulse;
//					print(deltaVelocity);
//					rb1.velocity = rb1.velocity + impulsePerIMass * -(1/rb1.mass);
//					rb2.velocity = rb2.velocity + impulsePerIMass * 1/rb2.mass;

//					var closingVelocity = rb1PerpendicularVelocity + rb2PerpendicularVelocity;
				} else {
					rb1.Move(offset1 * 2);
					var rb1PerpendicularVelocity = Vector2.Dot(rb1.velocity, -dir) * -dir;
					var rb1ParalellVelocity = rb1.velocity - rb1PerpendicularVelocity;
					rb1.velocity = rb1ParalellVelocity * avgFriction - rb1PerpendicularVelocity * avgBounciness;
				}
			} else {
				if (rb2 != null && !rb2.isKinematic){
					rb2.Move(offset2 * 2);
					var rb2PerpendicularVelocity = Vector2.Dot(rb2.velocity, dir) * dir;
					var rb2ParalellVelocity = rb2.velocity - rb2PerpendicularVelocity;
					rb2.velocity = rb2ParalellVelocity * avgFriction - rb2PerpendicularVelocity * avgBounciness;
				}
			}
		}// else
//			ExitCallbackCheck(_col, _col2);
	}

	Vector2 GetBoxContactPoint(Vector2 _boxPos, Vector2 _otherPos, BoxCol2D _boxCol){
		Vector2 boxContactPoint;
		boxContactPoint.x = Mathf.Max(_boxPos.x-_boxCol.Bounds.x, Mathf.Min(_otherPos.x, _boxPos.x + _boxCol.Bounds.x)); //CHECK IF NEED TO CENTER
		boxContactPoint.y = Mathf.Max(_boxPos.y-_boxCol.Bounds.y, Mathf.Min(_otherPos.y, _boxPos.y + _boxCol.Bounds.y));
		return boxContactPoint;
	}

	void BoxCircle(BoxCol2D _boxCol, CircleCol2D _circleCol){
		var spherePos = _circleCol.transform.position.ToVec2();
		var boxPos = _boxCol.transform.position.ToVec2();

		//Check clamped outer box coordinate | ALSO CONTACT POINT || https://yal.cc/rectangle-circle-intersection-test/
		var boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol); //CHECK IF NEED TO CENTER
		var delta = spherePos - boxContactPoint; //Delta to circle

		if (delta.x * delta.x + delta.y * delta.y < _circleCol.Radius * _circleCol.Radius){ //If distance to contact point is smaller than circle radius then they are in contact
//			CallbackCheck(_boxCol, _circleCol);
			if (_boxCol.isTrigger || _circleCol.isTrigger)
				return;

			var boxRB = _boxCol.GetComponent<RigidBod2D>();
			var sphereRB = _circleCol.GetComponent<RigidBod2D>();

			var normal = delta.normalized;
			var avgFriction = Mathf.Clamp01(1 - (_boxCol.friction + _circleCol.friction) / 2);
			var avgBounciness = Mathf.Max((_boxCol.bounciness + _circleCol.bounciness) / 2, 0);

			Vector2 closestPointOnCircle;
			if (_boxCol.Overlapping(spherePos)){ //TODO: set differently when sphere center overlaps box
				var dir = (spherePos - boxPos).normalized;
				spherePos = dir * (_boxCol.Bounds.x + _boxCol.Bounds.y + _circleCol.Radius); //Offset sphere outside box
				boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);
				normal = (spherePos - boxContactPoint).normalized;
				closestPointOnCircle = spherePos - normal * _circleCol.Radius;
				spherePos += boxContactPoint - closestPointOnCircle;
			}

			closestPointOnCircle = spherePos - normal * _circleCol.Radius;
			var offset = boxContactPoint - closestPointOnCircle; //ALSO NORMAL? | NOT ROBUST, WILL GENERATE WRONG NUMBERS IF SPHERE CENTER OVERLAPS BOX

			if (boxRB != null && !boxRB.isKinematic){
				if (sphereRB != null && !sphereRB.isKinematic){
					var massFrac = boxRB.mass / (boxRB.mass+sphereRB.mass);
					boxRB.transform.position = spherePos + -offset * massFrac;
					//RECALC DELTA FOR CORRECT NORMAL AFTER OFFSET
					boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);

					delta = spherePos - boxContactPoint;
					normal = delta.normalized;

					var perpendicularVelocity = Vector2.Dot(-boxRB.velocity, normal)*normal;
					var paralellVelocity = -boxRB.velocity - perpendicularVelocity;

					boxRB.velocity = paralellVelocity * avgFriction - perpendicularVelocity * massFrac * avgBounciness;
				} else {
					boxRB.Move(-offset);
					boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);
					delta = spherePos - boxContactPoint;
					normal = delta.normalized;

					var perpendicularVelocity = Vector2.Dot(-boxRB.velocity, normal)*normal;
					var paralellVelocity = -boxRB.velocity - perpendicularVelocity;

					boxRB.velocity = paralellVelocity * avgFriction - perpendicularVelocity * avgBounciness;
				}
			} else {
				if (sphereRB != null && !sphereRB.isKinematic){
					sphereRB.Move(offset);
					boxContactPoint = GetBoxContactPoint(boxPos, spherePos, _boxCol);
					delta = spherePos - boxContactPoint;
					normal = delta.normalized;
				
					var perpendicularVelocity = Vector2.Dot(sphereRB.velocity, normal) * normal;
					var paralellVelocity = sphereRB.velocity - perpendicularVelocity;

					sphereRB.velocity = paralellVelocity * avgFriction - perpendicularVelocity * avgBounciness;
				}
			}
		}// else
//			ExitCallbackCheck(_boxCol, _circleCol);
	}

	bool PointOverlapBox(Vector2 _point, BoxCol2D _boxCol){
		var insideX = _boxCol.transform.position.x - _boxCol.transform.lossyScale.x/2 < _point.x && _point.x < _boxCol.transform.position.x + _boxCol.transform.lossyScale.x/2;
		var insideY = _boxCol.transform.position.y-_boxCol.transform.lossyScale.y/2 < _point.y && _point.y < _boxCol.transform.position.y+_boxCol.transform.lossyScale.y/2;
		return insideX && insideY;
	}


	delegate void Callbacks(Collider2DBase _col);

	void CallbackCheck(Collider2DBase _col, Collider2DBase _col2){
		if (_col.gameObject.GetComponent<RigidBod2D>() != null ||
			_col2.gameObject.GetComponent<RigidBod2D>() != null){
			var go1 = _col.gameObject;
			var go2 = _col2.gameObject;

			bool pairFound = false;
			if (collisions.ContainsKey(go1)){
				if (collisions[go1].Contains(go2)){
					pairFound = true;
				} else
					collisions[go1].Add(go2);
			} else if (collisions.ContainsKey(go2)){
				if (collisions[go2].Contains(go1)){
					pairFound = true;
				} else
					collisions[go2].Add(go1);
			} else
				collisions.Add(go1, new List<GameObject>{go2});

			if (!pairFound){
				var enterCallbacks = go1.GetComponents<IEnterCallback>();
				for (int i = 0; i < enterCallbacks.Length; i++)
					enterCallbacks[i].OnColEnter(_col2);
				enterCallbacks = go2.GetComponents<IEnterCallback>();
				for (int i = 0; i < enterCallbacks.Length; i++)
					enterCallbacks[i].OnColEnter(_col);
			}

			var stayCallbacks = go1.GetComponents<IStayCallback>();
			for (int i = 0; i < stayCallbacks.Length; i++)
				stayCallbacks[i].OnColStay(_col2);
			
			stayCallbacks = go2.GetComponents<IStayCallback>();
			for (int i = 0; i < stayCallbacks.Length; i++)
				stayCallbacks[i].OnColStay(_col);
		}
	}

	void ExitCallbackCheck(Collider2DBase _col, Collider2DBase _col2){
		var go1 = _col.gameObject;
		var go2 = _col2.gameObject;

		bool pairFound = false;
		if (collisions.ContainsKey(go1)){
			if (collisions[go1].Contains(go2)){
				collisions[go1].Remove(go2);
				if (collisions[go1].Count == 0)
					collisions.Remove(go1);
				pairFound = true;
			}
		}
		if (collisions.ContainsKey(go2)){
			if (collisions[go2].Contains(go1)){
				collisions[go2].Remove(go1);
				if (collisions[go2].Count == 0)
					collisions.Remove(go2);
				pairFound = true;
			}
		}
		if (pairFound){
			var exitCallbacks = go1.GetComponents<IExitCallback>();
			for (int i = 0; i < exitCallbacks.Length; i++)
				exitCallbacks[i].OnColExit(_col2);
			exitCallbacks = go2.GetComponents<IExitCallback>();
			for (int i = 0; i < exitCallbacks.Length; i++)
				exitCallbacks[i].OnColExit(_col);
		}
	}
}

public interface IEnterCallback {
	void OnColEnter(Collider2DBase _col);
}
public interface IStayCallback {
	void OnColStay(Collider2DBase _col);
}
public interface IExitCallback {
	void OnColExit(Collider2DBase _col);
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