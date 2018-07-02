using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

	#region Note
	class Note
	{
		public RectTransform m_note;
		public TMPro.TMP_Text m_text;
		public KeyCode m_keycode;
		public Note(GameObject obj, char letter)
		{
			obj.SetActive(true);
			m_note = obj.GetComponent<RectTransform>();
			m_text = obj.GetComponentInChildren<TMPro.TMP_Text>();
			m_text.text = letter.ToString();
			m_keycode = UtilityFunc.GetKeycodeFromChar(letter);
		}
		public bool IsNoteHit()
		{
			return Input.GetKeyDown(m_keycode);
		}
	}
	#endregion

	[SerializeField] int m_noteScrollSpeed = 200;
	[SerializeField] int m_noteHitPosition = -530;
	[SerializeField] int m_noteMissPosition = -750;
	[SerializeField] int m_perfectDistance = 30;
	[SerializeField] int m_goodDistance = 80;

	[SerializeField] string testString = "Hello, World!";


	List<Note> m_activeNotes; //Contains all of the notes currently moving and checking input
	int m_currentLetter = 0;
	void Start ()
	{
		m_activeNotes = new List<Note>();
		StartCoroutine(SpawnNotes(0.3f));
		testString = testString.ToUpper();
	}
	
	void Update ()
	{
		Vector2 scrollAmount = Vector2.left * m_noteScrollSpeed * Time.deltaTime;
		m_activeNotes.ForEach(n => n.m_note.anchoredPosition += scrollAmount);
		if (m_activeNotes.Count > 0 && m_activeNotes[0].IsNoteHit())
		{
			HitFirstNote();
		}
		else if (m_activeNotes.Count > 0 && m_activeNotes[0].m_note.anchoredPosition.x < m_noteMissPosition)
		{
			MissNote();
		}
	}
	void HitFirstNote()
	{
		m_activeNotes[0].m_note.gameObject.SetActive(false);
		float distance = Mathf.Abs(m_noteHitPosition - m_activeNotes[0].m_note.anchoredPosition.x);
		if (distance <= m_perfectDistance) Debug.Log("Perfect!");
		else if (distance <= m_goodDistance) Debug.Log("Good");
		else Debug.Log("Miss");
		m_activeNotes.RemoveAt(0);
	}
	void MissNote()
	{
		m_activeNotes[0].m_note.gameObject.SetActive(false);
		m_activeNotes.RemoveAt(0);
	}
	IEnumerator SpawnNotes(float delay)
	{
		while (m_currentLetter < testString.Length)
		{
			yield return new WaitForSeconds(delay);
			if(testString[m_currentLetter] != ' ')
				SpawnNextNote();
			m_currentLetter++;
		}
	}
	void SpawnNextNote()
	{
		Note note = new Note(NotePool.i.GetObjectFromPool(), testString[m_currentLetter]);
		note.m_note.anchoredPosition = Vector2.right * 750;
		m_activeNotes.Add(note);
	}
}
