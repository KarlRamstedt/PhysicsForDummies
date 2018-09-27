using UnityEngine;

public class SceneLoader : MonoBehaviour {
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		} else if (Input.GetKeyDown(KeyCode.R))
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		else if (Input.GetKeyDown(KeyCode.Alpha1))
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		else if (Input.GetKeyDown(KeyCode.Alpha2))
			UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
}
