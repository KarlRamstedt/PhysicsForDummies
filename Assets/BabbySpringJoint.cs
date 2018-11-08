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
			var finalForce = Vector2.ClampMagnitude(force + dampingRatio, force.magnitude); //Make sure damping can't put force into negatives
			rb.AddForce(finalForce);
			connectedBody.AddForce(-finalForce); //Note: Mass is taken into account through the use of Force
		} else if (rb.isKinematic){
			var dampingRatio = connectedBody.velocity * damping;
			connectedBody.AddForce(Vector2.ClampMagnitude(-force - dampingRatio, force.magnitude));
		} else if (connectedBody.isKinematic){
			var dampingRatio = rb.velocity * damping; //TODO: Improve damping. Damping is applied to all movement, not only that specific to the spring | https://gamedev.stackexchange.com/questions/105728/how-to-program-a-fully-controllable-spring-damped-motion
			rb.AddForce(Vector2.ClampMagnitude(force - dampingRatio, force.magnitude));
		}
	}
}
