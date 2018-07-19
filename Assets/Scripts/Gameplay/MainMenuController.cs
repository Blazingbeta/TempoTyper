using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
	public static string GameString = "Hello, World!";
	[SerializeField] CanvasGroup m_mainGroup = null, m_selectGroup = null;
	[SerializeField] float m_fadeTime = 1.0f;
	public void LoadSong(string name)
	{
		TextAsset file = Resources.Load("Texts/" + name) as TextAsset;
		string text = file.text;
		GameString = text.Replace('\n', ' ');
		//Load game
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
	public void ToSongSelectMenu()
	{
		StartCoroutine(ToSongSelect());
	}
	public void BackToMainMenu()
	{
		StartCoroutine(ToMainMenu());
	}
	IEnumerator ToSongSelect()
	{
		m_mainGroup.interactable = false;
		m_mainGroup.blocksRaycasts = false;
		float time = 0f;
		while (time < m_fadeTime)
		{
			m_mainGroup.alpha = 1f - (time / m_fadeTime);
			time += Time.deltaTime;
			yield return null;
		}
		m_mainGroup.alpha = 0;
		time = 0f;
		while(time < m_fadeTime)
		{
			m_selectGroup.alpha = (time / m_fadeTime);
			time += Time.deltaTime;
			yield return null;
		}
		m_selectGroup.alpha = 1;
		m_selectGroup.interactable = true;
		m_selectGroup.blocksRaycasts = true;
	}
	IEnumerator ToMainMenu()
	{
		m_selectGroup.interactable = false;
		m_selectGroup.blocksRaycasts = false;
		float time = 0f;
		while(time < m_fadeTime)
		{
			m_selectGroup.alpha = 1f - (time / m_fadeTime);
			time += Time.deltaTime;
			yield return null;
		}
		m_selectGroup.alpha = 0;
		time = 0f;
		while (time < m_fadeTime)
		{
			m_mainGroup.alpha = (time / m_fadeTime);
			time += Time.deltaTime;
			yield return null;
		}
		m_mainGroup.alpha = 1;
		m_mainGroup.interactable = true;
		m_mainGroup.blocksRaycasts = true;
	}
	public void QuitGame()
	{
		Application.Quit();
	}
	public void ToTutorial()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}
	public void FromTutorial()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}
