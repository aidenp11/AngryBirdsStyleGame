using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour
{
	[SerializeField] string nextLevel = "";
	public void OnRetryButton()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void OnNextLevelButton()
	{
		if (nextLevel == "") SceneManager.LoadScene("LevelSelector");
		else SceneManager.LoadScene(nextLevel);
	}

	public void OnLevelSelectButton()
	{
		SceneManager.LoadScene("LevelSelector");
	}
}
