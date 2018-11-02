using UnityEngine;

public class SetSolverIterations : MonoBehaviour {

	[Range(1, 99)] public int solverIterations = 1;

	int oldValue;

	void Awake(){
		oldValue = CollisionManager.Inst.solverIterations;
		CollisionManager.Inst.solverIterations = solverIterations;
	}

	void OnDestroy(){
		if (!CollisionManager.ApplicationIsQuitting)
			CollisionManager.Inst.solverIterations = oldValue;
	}
}
