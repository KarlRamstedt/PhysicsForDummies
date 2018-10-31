using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColliderProperities : MonoBehaviour {

	public Material selectedMaterial;
	Material normalMaterial;

	UnityEngine.UI.Slider[] sliders;
	Collider2DBase selectedCollider;
	RigidBod2D selectedRB;

	void Awake(){
		sliders = GetComponentsInChildren<UnityEngine.UI.Slider>();
	}

	void Update(){
		var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;

		if (Input.GetMouseButtonDown(0)){
			var col = CollisionManager.Inst.colliders;
			for (int i = 0; i < col.Count; i++){
				if (col[i].Overlapping(point)){
					if (selectedCollider != null)
						selectedCollider.gameObject.GetComponent<Renderer>().material = normalMaterial;
					
					selectedCollider = col[i];
					selectedRB = col[i].gameObject.GetComponent<RigidBod2D>();
					var rend = col[i].gameObject.GetComponent<Renderer>();
					if (rend != null){
						normalMaterial = rend.material;
						rend.material = selectedMaterial;
					}
					if (sliders.Length > 0) sliders[0].value = selectedCollider.bounciness;
					if (sliders.Length > 1) sliders[1].value = selectedCollider.friction;
					if (sliders.Length > 2){
						if (selectedRB != null){
							sliders[2].interactable = true;
							sliders[2].value = selectedRB.mass;
						} else
							sliders[2].interactable = false;
					}
					break;
				}
			}
		}
	}

	public void SetBounciness(float _bounce){
		if (selectedCollider != null)
			selectedCollider.bounciness = _bounce;
	}

	public void SetFriction(float _friction){
		if (selectedCollider != null)
			selectedCollider.friction = _friction;
	}

	public void SetMass(float _mass){
		if (selectedRB != null)
			selectedRB.mass = _mass;
	}
}
