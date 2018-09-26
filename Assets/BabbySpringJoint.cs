using UnityEngine;

[RequireComponent(typeof(BabbysFirstRigidbody))]
public class BabbySpringJoint : MonoBehaviour {

	public BabbysFirstRigidbody connectedBody;
	public float springConstant = 9;
	public float restingLength = 2;
    public float damping = 1f;

	void FixedUpdate(){ //F = k * x
		var delta = connectedBody.transform.position.ToVec2() - transform.position.ToVec2();

        if (Mathf.Approximately(delta.magnitude, restingLength))
            return;

        var rb = GetComponent<BabbysFirstRigidbody>();
		float lengthDelta = delta.magnitude - restingLength;
		delta = delta.normalized * lengthDelta;

		if (!rb.isKinematic && !connectedBody.isKinematic){
			var dampingRatio = (connectedBody.velocity - rb.velocity) * damping; //Relative velocity * damping factor
			rb.AddForce(delta * springConstant * (rb.mass / connectedBody.mass) + dampingRatio, ForceMode.Force);
			connectedBody.AddForce(-delta * springConstant * (connectedBody.mass / rb.mass) - dampingRatio, ForceMode.Force);
		} else if (rb.isKinematic){
			var dampingRatio = connectedBody.velocity * damping;
			connectedBody.AddForce(-delta * springConstant - dampingRatio, ForceMode.Force); //TODO: DAMP
		} else if (connectedBody.isKinematic){
			var dampingRatio = rb.velocity * damping;
			rb.AddForce(delta * springConstant - dampingRatio, ForceMode.Force); //TODO: DAMP
		}
	}
}
