using UnityEngine;

public abstract class Constraint : MonoBehaviour {

	protected virtual void OnEnable(){
		CollisionManager.Inst.RegisterComponent(this);
	}
	protected virtual void OnDisable(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.DeRegisterComponent(this);
	}

	public abstract void Constrain();
}
