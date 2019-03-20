using UnityEngine;

/// <summary>
/// Shitty thrown-together scene-switching utility script.
/// </summary>
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
		else if (Input.GetKeyDown(KeyCode.Alpha3))
			UnityEngine.SceneManagement.SceneManager.LoadScene(2);
		else if (Input.GetKeyDown(KeyCode.Alpha4))
			UnityEngine.SceneManagement.SceneManager.LoadScene(3);
		else if (Input.GetKeyDown(KeyCode.Alpha5))
			UnityEngine.SceneManagement.SceneManager.LoadScene(4);
		else if (Input.GetKeyDown(KeyCode.Alpha6))
			UnityEngine.SceneManagement.SceneManager.LoadScene(5);
		else if (Input.GetKeyDown(KeyCode.Alpha7))
			UnityEngine.SceneManagement.SceneManager.LoadScene(6);
		else if (Input.GetKeyDown(KeyCode.Alpha8))
			UnityEngine.SceneManagement.SceneManager.LoadScene(7);
		else if (Input.GetKeyDown(KeyCode.Alpha9))
			UnityEngine.SceneManagement.SceneManager.LoadScene(8);
		else if (Input.GetKeyDown(KeyCode.Alpha0))
			UnityEngine.SceneManagement.SceneManager.LoadScene(9);
	}
}
