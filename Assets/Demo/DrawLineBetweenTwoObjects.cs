using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLineBetweenTwoObjects : MonoBehaviour {

	public Transform[] other;

	LineRenderer lr;
	void Awake(){
		lr = GetComponent<LineRenderer>();
		if (other.Length > 1)
			lr.positionCount = other.Length*2;
	}

	void Update(){
		if (other.Length < 1)
			return;
		for (int i = 0; i < other.Length*2; i+=2){
			lr.SetPosition(i, transform.position);
			lr.SetPosition(i+1, other[i/2].position);
		}
	}
}
