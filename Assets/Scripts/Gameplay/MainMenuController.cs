using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
	public static string DataFilePath = "SongData/RickAndMorty";
	public static int Difficulty = 1;
	[SerializeField] CanvasGroup m_mainGroup = null, m_selectGroup = null, m_difficultyGroup = null;
	[SerializeField] float m_fadeTime = 1.0f;
	public void LoadSong(string name)
	{
		DataFilePath = "SongData/" + name;
		StartCoroutine(FadeBetweenCanvasLayers(m_selectGroup, m_difficultyGroup, m_fadeTime));
	}
	public void ToSongSelectMenu()
	{
		StartCoroutine(FadeBetweenCanvasLayers(m_mainGroup, m_selectGroup, m_fadeTime));
	}
	public void BackToMainMenu()
	{
		StartCoroutine(FadeBetweenCanvasLayers(m_selectGroup, m_mainGroup, m_fadeTime));
	}
	public void SelectDifficulty(int difficulty)
	{
		Difficulty = difficulty;
		SceneLoader.i.LoadScene(1);
	}
	public void DifficultyMenuToMainMenu()
	{
		StartCoroutine(FadeBetweenCanvasLayers(m_difficultyGroup, m_mainGroup, m_fadeTime));
	}
	/// <summary>
	/// Swaps the canvas group by fading transparency over time
	/// </summary>
	/// <param name="a">The group to fade out</param>
	/// <param name="b">The group to fade in </param>
	/// <returns></returns>
	IEnumerator FadeBetweenCanvasLayers(CanvasGroup a, CanvasGroup b, float timea)
	{
		a.interactable = false;
		a.blocksRaycasts = false;
		float timer = 0f;
		while (timer < m_fadeTime)
		{
			a.alpha = 1f - (timer / m_fadeTime);
			timer += Time.deltaTime;
			yield return null;
		}
		a.alpha = 0;
		timer = 0f;
		while (timer < m_fadeTime)
		{
			b.alpha = (timer / m_fadeTime);
			timer += Time.deltaTime;
			yield return null;
		}
		b.alpha = 1;
		b.interactable = true;
		b.blocksRaycasts = true;
	}
	public void QuitGame()
	{
		Application.Quit();
	}
	public void ToTutorial()
	{
		SceneLoader.i.LoadScene(2);
	}
	public void FromTutorial()
	{
		SceneLoader.i.LoadScene(0);
	}
}
