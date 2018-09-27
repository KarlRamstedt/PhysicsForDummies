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
		var totalDamp = damping / 2 * Mathf.Sqrt(springConstant * rb.mass);
		float lengthDelta = delta.magnitude - restingLength;
		delta = delta.normalized * lengthDelta;
		var force = delta * springConstant;

		if (!rb.isKinematic && !connectedBody.isKinematic){ //TODO: USE MASS IN CALCULATION && FIX HANDLING FOR HIGH DAMPING VALUES
			var dampingRatio = (connectedBody.velocity - rb.velocity) * damping; //Relative velocity * damping factor
			var forceMag = force.magnitude;
			rb.AddForce(Vector2.ClampMagnitude(force + dampingRatio, forceMag), ForceMode.Force); //Make sure damping can't put force into negatives
			connectedBody.AddForce(Vector2.ClampMagnitude(-force - dampingRatio, forceMag), ForceMode.Force);
		} else if (rb.isKinematic){
			var dampingRatio = connectedBody.velocity * damping;
			connectedBody.AddForce(Vector2.ClampMagnitude(-force - dampingRatio, force.magnitude), ForceMode.Force);
		} else if (connectedBody.isKinematic){
			var dampingRatio = rb.velocity * damping; //TODO: FIX
			rb.AddForce(Vector2.ClampMagnitude(force - dampingRatio, force.magnitude), ForceMode.Force);
		}
	}
}
