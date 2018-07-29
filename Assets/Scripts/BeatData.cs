using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "BeatData")]
public class BeatData : ScriptableObject
{
	public int BPM;
	public AudioClip Song;
	public string Text;
	public string HardText;
	public int NoteScrollSpeed;
}
