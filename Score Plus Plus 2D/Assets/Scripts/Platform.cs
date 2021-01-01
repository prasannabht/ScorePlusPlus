using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ScoreDetails{

	public int score;
	public int scoreStar;
	public float scoreTime;
	public string scoreString;
	public int puzzleNum;
}

public class Platform : MonoBehaviour {
	void OnMouseUp() {
		Debug.Log("Clicked");
	}
}