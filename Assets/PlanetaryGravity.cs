using UnityEngine;

public class PlanetaryGravity : MonoBehaviour {

	float gravitationalConstant = 6.674f * Mathf.Pow(10, -11);

	void FixedUpdate(){
		var rabies = CollisionManager.Inst.rigidbodies;
		for (int i = 0; i < rabies.Count; i++){
			for (int j = i+1; j < rabies.Count; j++) {
				var pos = rabies[i].transform.position.ToVec2();
				var otherPos = rabies[j].transform.position.ToVec2();
				var distance = pos.Distance(otherPos);
				float force = gravitationalConstant * rabies[i].mass*rabies[j].mass/(distance*distance);
				var dir = (otherPos-pos).normalized;
				if (rabies[i].useGravity)
					rabies[i].AddForce(dir*force, ForceMode.Force);
				if (rabies[j].useGravity)
					rabies[j].AddForce(-dir*force, ForceMode.Force);
			}
		}
	}
}
