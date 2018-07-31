using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HighscoreManager : MonoBehaviour {
	static int m_score = 12345;
	public static bool IsNewHighscore(string songName, int score)
	{
		int previousScore = PlayerPrefs.GetInt(songName + "Highscore", 0);
		m_score = score; //this scene can only be reached if this returns true so setting it here is safe
		return score > previousScore;
	}
	[SerializeField] TMPro.TMP_Text m_initialText;
	[SerializeField] TMPro.TMP_Text m_scoreText;
	[SerializeField] GameObject m_continueText;
	[SerializeField] Transform m_previousScorePanel;
	[SerializeField] AudioSource m_SFXPlayer;
	[SerializeField] AudioClip m_finishSFX;
	StringBuilder m_currentName = new StringBuilder();

	string songName = "";
	private void Start()
	{
		songName = MainMenuController.DataFilePath.Split('/')[1];
		if(MainMenuController.Difficulty == 1)
		{
			songName += "Hard";
		}
		m_scoreText.text = m_score.ToString("D6");
		m_previousScorePanel.GetChild(1).GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString(songName + "Initials", "---");
		m_previousScorePanel.GetChild(2).GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetInt(songName + "Highscore", 0).ToString("D6");
	}
	void Update ()
	{
		if(Input.inputString.Length == 1 && m_currentName.Length != 3 
			&& Input.inputString[0] >= 'a' && Input.inputString[0] <= 'z')
		{
			m_currentName.Append(Input.inputString);
			string outputString = m_currentName.ToString();
			for(int i = 0; i < 3 - m_currentName.Length; i++)
			{
				outputString += "-";
			}
			m_initialText.text = outputString;
			m_continueText.SetActive(m_currentName.Length == 3);
			UIShake.i.Shake(.2f);
			m_SFXPlayer.Play();
		}
		if (Input.GetKeyDown(KeyCode.Backspace) && m_currentName.Length != 0)
		{
			m_currentName.Length--;
			string outputString = m_currentName.ToString();
			for (int i = 0; i < 3 - m_currentName.Length; i++)
			{
				outputString += "-";
			}
			m_initialText.text = outputString;
			m_continueText.SetActive(false);
			UIShake.i.Shake(.2f);
			m_SFXPlayer.Play();
		}
		if (Input.GetKeyDown(KeyCode.Return) && m_currentName.Length == 3)
		{
			m_SFXPlayer.PlayOneShot(m_finishSFX);
			//Save highscore and initials
			PlayerPrefs.SetInt(songName + "Highscore", m_score);
			PlayerPrefs.SetString(songName + "Initials", m_currentName.ToString());
			if(SceneLoader.i != null)
			{
				SceneLoader.i.LoadScene(0);
			}
		}
	}
}
