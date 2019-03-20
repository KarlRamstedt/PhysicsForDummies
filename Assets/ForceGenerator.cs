using UnityEngine;

public abstract class ForceGenerator : MonoBehaviour {

	protected virtual void OnEnable(){
		CollisionManager.Inst.RegisterComponent(this);
	}
	protected virtual void OnDisable(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.DeRegisterComponent(this);
	}

	public abstract void GenerateForce();
}
