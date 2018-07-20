using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour {
	public static SceneLoader i;
	CanvasGroup m_canvasGroup = null;
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(gameObject);
		i = this;
		m_canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
	}
	public void LoadScene(int sceneID)
	{
		StartCoroutine(FadeIntoScene(sceneID, 1.25f));
	}
	IEnumerator FadeIntoScene(int sceneID, float fadeDuration)
	{
		m_canvasGroup.blocksRaycasts = true;
		float timer = 0;
		while(timer < fadeDuration)
		{
			yield return null;
			timer += Time.deltaTime;
			m_canvasGroup.alpha = timer / fadeDuration;
		}
		m_canvasGroup.alpha = 1.0f;
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
		while(timer > 0)
		{
			yield return null;
			timer -= Time.deltaTime;
			m_canvasGroup.alpha = timer / fadeDuration;
		}
		m_canvasGroup.blocksRaycasts = false;
	}
}
