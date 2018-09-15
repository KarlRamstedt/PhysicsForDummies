using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRBVelocityWithMouse : MonoBehaviour {

	public BabbysFirstRigidbody rb;
	public Material mat;
	LineRenderer lr;
	Vector3 point;

	void Start(){
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 2;
	}

	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
		if (Input.GetKeyDown(KeyCode.R))
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;
		lr.SetPosition(0, rb.transform.position);
		lr.SetPosition(1, point);
		if (Input.GetMouseButtonDown(0)){
			rb.velocity = point - rb.transform.position;
		}
	}
}
