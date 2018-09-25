using UnityEngine;

public class BabbySpringJoint : MonoBehaviour {

	public BabbysFirstRigidbody connectedBody;
	public float springConstant = 9;
	public float restingLength = 2;

	void FixedUpdate(){
		var delta = connectedBody.transform.position.ToVec2() - transform.position.ToVec2();
		if (Mathf.Approximately(delta.magnitude, restingLength))
			return;
		
		var rb = GetComponent<BabbysFirstRigidbody>();
		float lengthDelta = delta.magnitude - restingLength;
		delta = delta.normalized;
		delta = delta * lengthDelta;
		
		if (delta.magnitude < restingLength){
			Debug.DrawRay(transform.position, delta.normalized);
			rb.AddForce(-delta * springConstant * (rb.mass / connectedBody.mass), ForceMode.Force);
			connectedBody.AddForce(delta * springConstant * (connectedBody.mass / rb.mass), ForceMode.Force);
		} else {
			Debug.DrawRay(transform.position, -delta.normalized);
			rb.AddForce(delta * springConstant * (rb.mass / connectedBody.mass), ForceMode.Force);
			connectedBody.AddForce(-delta * springConstant * (connectedBody.mass / rb.mass), ForceMode.Force);
		}
	}
}
