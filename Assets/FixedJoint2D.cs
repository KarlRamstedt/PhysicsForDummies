using UnityEngine;

[RequireComponent(typeof(RigidBod2D))]
public class FixedJoint2D : MonoBehaviour {

	public RigidBod2D connectedBody;
	[Tooltip("Set length equal to distance between this and connectedBody at start.")]
	public bool autoConfigureLength = true;
	public float length = 0;

	RigidBod2D rb;
	void Awake(){
		rb = GetComponent<RigidBod2D>();
		if (autoConfigureLength)
			length = (connectedBody.transform.position.ToVec2() - transform.position.ToVec2()).magnitude;
	}

	void FixedUpdate(){
		var delta = connectedBody.transform.position.ToVec2() - transform.position.ToVec2();

        if (Mathf.Approximately(delta.magnitude, length))
            return;
		
		float lengthDelta = delta.magnitude - length;
		delta = delta.normalized * lengthDelta;

		if (!rb.isKinematic && !connectedBody.isKinematic){
			var massFrac = rb.mass / (rb.mass+connectedBody.mass); //Doesn't use force for movement, so needs to take mass into account
			rb.Move(delta * (1-massFrac));
			connectedBody.Move(-delta * massFrac); //Movement outside of collision loop, possible to integrate this in CollisionManager?
		} else if (rb.isKinematic){
			connectedBody.Move(-delta);
		} else if (connectedBody.isKinematic){
			rb.Move(delta);
		}
	}
}
