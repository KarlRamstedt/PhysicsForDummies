using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColliderProperities : MonoBehaviour {

	public GameObject objectToChange;

	Collider2DBase col;
	RigidBod2D rb;

	void Start(){
		col = objectToChange.GetComponent<Collider2DBase>();
		rb = objectToChange.GetComponent<RigidBod2D>();
		var slider = GetComponentsInChildren<UnityEngine.UI.Slider>();
		slider[0].value = col.bounciness;
		slider[1].value = col.friction;
//		slider[2].value = rb.mass;
	}
	
	public void SetBounciness(float _bounce){
		col.bounciness = _bounce;
	}

	public void SetFriction(float _friction){
		col.friction = _friction;
	}

	public void SetMass(float _mass){
		rb.mass = _mass;
	}
}
