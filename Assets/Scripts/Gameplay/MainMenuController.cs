using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
	public static string DataFilePath = "SongData/RickAndMorty";
	public static int Difficulty = 1;
	[SerializeField] CanvasGroup m_mainGroup = null, m_selectGroup = null, m_difficultyGroup = null;
	[SerializeField] float m_fadeTime = 1.0f;

	//Info Display Stuff
	[SerializeField] TMPro.TMP_Text m_easyScore;
	[SerializeField] TMPro.TMP_Text m_hardScore;
	[SerializeField] Transform m_songInfoPanel;
	Dictionary<string, string> m_highscoreTable = new Dictionary<string, string>();
	Dictionary<string, BeatData> m_songInfo = new Dictionary<string, BeatData>();
	private void Start()
	{
		string[] songList = new string[] { "RickAndMorty", "Mesothelioma", "PacerTest" };
		//Highscore Setup
		for(int j = 0; j < songList.Length; j++)
		{
			string easyText = PlayerPrefs.GetString(songList[j] + "Initials", "---") 
				+ ":" + PlayerPrefs.GetInt(songList[j] + "Highscore", 0).ToString("D6");
			string hardText = PlayerPrefs.GetString(songList[j] + "HardInitials", "---")
				+ ":" + PlayerPrefs.GetInt(songList[j] + "HardHighscore", 0).ToString("D6");
			m_highscoreTable.Add(songList[j], easyText);
			m_highscoreTable.Add(songList[j] + "Hard", hardText);
		}
		//Song Info Setup
		for(int j = 0; j < songList.Length; j++)
		{
			m_songInfo.Add(songList[j], Resources.Load("SongData/" + songList[j]) as BeatData);
		}
	}
	bool m_lockScoreMenu = false;
	public void HoverSong(string songName)
	{
		m_easyScore.text = m_highscoreTable[songName];
		m_hardScore.text = m_highscoreTable[songName + "Hard"];
		m_easyScore.transform.parent.gameObject.SetActive(true);

		//Song Info
		BeatData data = m_songInfo[songName];
		m_songInfoPanel.GetChild(1).GetComponent<TMPro.TMP_Text>().text = data.Artist;
		m_songInfoPanel.GetChild(3).GetComponent<TMPro.TMP_Text>().text = data.SongName;
		m_songInfoPanel.GetChild(5).GetComponent<TMPro.TMP_Text>().text = data.BPM.ToString();
		m_songInfoPanel.gameObject.SetActive(true);
	}
	public void HoverEnd()
	{
		if (!m_lockScoreMenu)
		{
			m_easyScore.transform.parent.gameObject.SetActive(false);
			m_songInfoPanel.gameObject.SetActive(false);
		}
	}
	public void LoadSong(string name)
	{
		m_lockScoreMenu = true;
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
		m_lockScoreMenu = false;
		HoverEnd();
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
