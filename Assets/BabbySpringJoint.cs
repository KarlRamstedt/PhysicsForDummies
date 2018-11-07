using UnityEngine;

[RequireComponent(typeof(RigidBod2D))]
public class BabbySpringJoint : MonoBehaviour {

	public RigidBod2D connectedBody;
	public float springConstant = 9;
	[Tooltip("Set length equal to distance between this and connectedBody at start.")]
	public bool autoConfigureLength = false;
	public float restingLength = 2;
    public float damping = 1f;

	RigidBod2D rb;
	void Awake(){
		rb = GetComponent<RigidBod2D>();
		if (autoConfigureLength)
			restingLength = (connectedBody.transform.position.ToVec2() - rb.transform.position.ToVec2()).magnitude;
	}

	void FixedUpdate(){ //Force = springConstant * lengthDelta
		var delta = connectedBody.transform.position.ToVec2() - rb.transform.position.ToVec2();

		var deltaMag = delta.magnitude;
		if (Mathf.Approximately(deltaMag, restingLength))
			return;
		
		float lengthDelta = deltaMag - restingLength;
		delta = delta.normalized * lengthDelta;
		var force = delta * springConstant;

		if (!rb.isKinematic && !connectedBody.isKinematic){
			var dampingRatio = (connectedBody.velocity - rb.velocity) * damping; //Relative velocity * damping factor
			var forceMag = force.magnitude;
			rb.AddForce(Vector2.ClampMagnitude(force + dampingRatio, forceMag)); //Make sure damping can't put force into negatives
			connectedBody.AddForce(Vector2.ClampMagnitude(-force - dampingRatio, forceMag)); //Note: Mass is taken into account through the use of Force
		} else if (rb.isKinematic){
			var dampingRatio = connectedBody.velocity * damping;
			connectedBody.AddForce(Vector2.ClampMagnitude(-force - dampingRatio, force.magnitude));
		} else if (connectedBody.isKinematic){
			var dampingRatio = rb.velocity * damping; //Damping seems kind of off
			rb.AddForce(Vector2.ClampMagnitude(force - dampingRatio, force.magnitude));
		}
	}
}
