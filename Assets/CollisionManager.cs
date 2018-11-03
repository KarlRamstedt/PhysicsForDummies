using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiated on demand, don't place in scene.
/// </summary>
public class CollisionManager : MonoBehaviour {

	public static Vector2 gravity = new Vector2(0f, -9.8f);

	public List<Constraint> constraints = new List<Constraint>();
	public List<Collider2DBase> colliders = new List<Collider2DBase>();
	public List<RigidBod2D> rigidbodies = new List<RigidBod2D>();
	[Range(1, 99)] public int solverIterations = 9;

	Dictionary<GameObject, List<GameObject>> collisions = new Dictionary<GameObject, List<GameObject>>(); //Does order affect result? | CollisionExit will not run if object is disabled, is that a problem?

#region Singleton
	static CollisionManager instance;
	public static CollisionManager Inst {
		get {
			if (instance == null){
				if (ApplicationIsQuitting)
					return null;
				GameObject go = new GameObject("(Singleton) CollisionManager"); //Lazy-load object
				instance = go.AddComponent<CollisionManager>();
				DontDestroyOnLoad(go);
			}
			return instance;
		}
	}
	public static bool ApplicationIsQuitting { get; private set; }
	void OnDestroy(){
		ApplicationIsQuitting = true;
	}
#endregion

#region RegisterComponents
	public void RegisterComponent(RigidBod2D _col){
		if (!rigidbodies.Contains(_col))
			rigidbodies.Add(_col);
	}
	public void DeRegisterComponent(RigidBod2D _col){
		rigidbodies.Remove(_col);
	}
	public void RegisterComponent(Collider2DBase _col){
		if (!colliders.Contains(_col))
			colliders.Add(_col);
	}
	public void DeRegisterComponent(Collider2DBase _col){
		colliders.Remove(_col);
	}
	public void RegisterComponent(Constraint _con){
		if (!constraints.Contains(_con))
			constraints.Add(_con);
	}
	public void DeRegisterComponent(Constraint _con){
		constraints.Remove(_con);
	}
#endregion
	
	void FixedUpdate(){ //Runs 50 times per second (by default)
		for (int i = 0, len = rigidbodies.Count; i < len; i++){
			rigidbodies[i].UpdatePosition();
		}

		for (int i = 0; i < solverIterations; i++){ //Improves accuracy by repeating collision depenetration as needed
			for (int j = 0, len = colliders.Count; j < len; j++){
				for (int k = j+1; k < len; k++){
					CalculateCollision(colliders[j], colliders[k]);
				}
			}
			for (int j = 0, len = constraints.Count; j < len; j++){ //Collision first, then constrain, so constraints aren't executed with data from overlapping colliders
				constraints[j].Constrain();
			}
		}
	}

	void CalculateCollision(Collider2DBase _col1, Collider2DBase _col2){
		if (_col1 is BoxCol2D){
			var col1 = _col1 as BoxCol2D;

			if (_col2 is BoxCol2D){
//				var col2 = _col2 as BoxCol2D;
//				BoxBox(col1, col2);
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

//	void BoxBox(BoxCol2D _col, BoxCol2D _col2){ //TODO: Implement | https://gamedev.stackexchange.com/questions/29786/a-simple-2d-rectangle-collision-algorithm-that-also-determines-which-sides-that
//		
//	}

	void CircleCircle(CircleCol2D _col1, CircleCol2D _col2){
		var pos1 = _col1.transform.position.ToVec2(); //Cache some variables for performance
		var pos2 = _col2.transform.position.ToVec2();
		var radius1 = _col1.Radius;
		var radius2 = _col2.Radius;

		if (pos1.Distance(pos2) < radius1 + radius2){ //if overlapping
//			CallbackCheck(_col, _col2);
			if (_col1.isTrigger || _col2.isTrigger)
				return;
			
			var dir = (pos2 - pos1).normalized;
			var penetration = (pos2 - dir * radius1) - (pos1 + dir * radius2); //Essentially: _col2.ClosestPoint(pos) - _col.ClosestPoint(otherPos);

			var rb1 = _col1.GetComponent<RigidBod2D>();
			var rb2 = _col2.GetComponent<RigidBod2D>();

//			var avgFriction = Mathf.Clamp01(1 - (_col.friction + _col2.friction) / 2);
			var bounciness = Mathf.Clamp01((_col1.bounciness + _col2.bounciness) / 2); //Completely inelastic when final kinetic energy is 50% of original

			if (rb1 != null && !rb1.isKinematic){
				if (rb2 != null && !rb2.isKinematic){
					if (bounciness == 1){ //TODO: Constraints for elastic and gradual change between elastic and in-elastic collisions | https://physics.stackexchange.com/questions/126860/how-to-calculate-a-collision-which-is-partly-elastic-and-partly-inelastic
						var newVel1 = (rb1.velocity*(rb1.mass-rb2.mass) + 2*rb2.mass*rb2.velocity) / (rb1.mass + rb2.mass);
						var newVel2 = (rb2.velocity*(rb2.mass-rb1.mass) + 2*rb1.mass*rb1.velocity) / (rb1.mass + rb2.mass);
						rb1.velocity = newVel1; // * bounciness?
						rb2.velocity = newVel2;
					} else {
						var massFraction = rb1.mass / (rb1.mass + rb2.mass); //Simply constraining will make in-elastic collisions work great with verlet integration
						rb1.Move(penetration * (1 - massFraction));
						rb2.Move(-penetration * massFraction);
					}
				} else {
					rb1.Move(penetration);
				}
			} else {
				if (rb2 != null && !rb2.isKinematic){
					rb2.Move(-penetration);
				}
			}
		}// else
//			ExitCallbackCheck(_col, _col2);
	}

	void BoxCircle(BoxCol2D _boxCol, CircleCol2D _circleCol){
		var circlePos = _circleCol.transform.position.ToVec2(); //Cache some variables for performance
		var radius = _circleCol.Radius;
		var boxPos = _boxCol.transform.position.ToVec2();
		var boxBounds = _boxCol.Bounds;
	
		Vector2 closestPointOnBox; //Essentially: _boxCol.PointClampedToBox(circlePos) | -5% of total method execution time running it locally
		closestPointOnBox.x = Mathf.Max(boxPos.x-boxBounds.x, Mathf.Min(circlePos.x, boxPos.x + boxBounds.x));
		closestPointOnBox.y = Mathf.Max(boxPos.y-boxBounds.y, Mathf.Min(circlePos.y, boxPos.y + boxBounds.y));

		var delta = circlePos - closestPointOnBox;
	
		if (delta.x * delta.x + delta.y * delta.y < radius * radius){ //If distance to clamped point is smaller than circle radius then they are in contact || Squared distance for efficiency
//			CallbackCheck(_boxCol, _circleCol);
			if (_boxCol.isTrigger || _circleCol.isTrigger)
				return;
	
			var boxRB = _boxCol.GetComponent<RigidBod2D>();
			var sphereRB = _circleCol.GetComponent<RigidBod2D>();
	
			Vector2 closestPointOnCircle;
			if (_boxCol.Overlapping(circlePos)){
				closestPointOnBox = _boxCol.ClosestInsideOut(circlePos);
				var dir = (circlePos-closestPointOnBox).normalized;
				closestPointOnCircle = circlePos + dir * radius;
			} else {
				closestPointOnCircle = circlePos + -delta.normalized * radius;
			}
			var penetration = closestPointOnBox - closestPointOnCircle;
	
			if (boxRB != null && !boxRB.isKinematic){
				if (sphereRB != null && !sphereRB.isKinematic){
					var massFraction = boxRB.mass / (boxRB.mass+sphereRB.mass);
					boxRB.Move(-penetration * (1 - massFraction));
					sphereRB.Move(penetration * massFraction);
				} else {
					boxRB.Move(-penetration);
				}
			} else {
				if (sphereRB != null && !sphereRB.isKinematic){
					sphereRB.Move(penetration);
				}
			}
		}// else
//			ExitCallbackCheck(_boxCol, _circleCol);
	}

#region CollisionCallbacks
	//TODO: Proper collision data structure and prevent callbacks from running multiple times with solverIterations > 1 | Use Unity's own? List<Collision> collisionData = new List<Collision>();
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
#endregion

public static class Vector2Extensions {
	/// <summary>
	/// For convenience. Not in Unity by default. Bonus: better performance
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
	/// <summary>
	/// For convenience. Not in Unity by default. Bonus: better performance
	/// </summary>
	public static float Distance(this Vector3 _from, Vector3 _to){
		var delta = _from - _to;
		return Mathf.Sqrt(delta.x*delta.x + delta.y*delta.y + delta.z*delta.z);
	}
}