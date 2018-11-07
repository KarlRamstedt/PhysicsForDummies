using System.Collections.Generic;
using UnityEngine;

public class WaterDemo : MonoBehaviour {

	public GameObject waterObject;
	public float cooldown = 0.1f;

	List<Transform> water = new List<Transform>();
	float lastSpawnTime = 0f;
	Camera cam;

	void Start(){
		cam = Camera.main;
	}

	void Update(){ //Performance could probably improve greatly by pooling the water objects, but that is way too much work for a quick demo like this
		var point = cam.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;

		if (Input.GetMouseButton(0)){
			var time = Time.time;
			if (time > lastSpawnTime + cooldown){
				lastSpawnTime = time;
				var waterDroplet = Instantiate(waterObject, point, Quaternion.identity);
				waterDroplet.SetActive(true);
				water.Add(waterDroplet.transform);
			}
		}
		for (int i = water.Count-1; i > 0; i--){
			var waterDroplet = water[i];
			if (waterDroplet.position.y < -6f){
				water.Remove(waterDroplet);
				Destroy(waterDroplet.gameObject);
			}
		}
	}
}
