using UnityEngine;

public class SceneLoader : MonoBehaviour {
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
		if (Input.GetKeyDown(KeyCode.R))
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}
}
