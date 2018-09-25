using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLineBetweenTwoObjects : MonoBehaviour {

	public Transform other;

	LineRenderer lr;

	void Start(){
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 2;
	}

	void Update(){
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, other.position);
	}
}
