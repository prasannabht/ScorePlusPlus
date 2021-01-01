using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class TimeManager : MonoBehaviour {

	[HideInInspector]
	public float time3star = 10.0f;
	[HideInInspector]
	public float time2star = 15.0f;
	[HideInInspector]
	public float time1star = 20.0f;

	static float startTime, endTime;
	[HideInInspector]
	public bool timerStart = false;

	[HideInInspector]
	public float pausedDuration = 0;

	int star = 0;

	float timeTaken = 0;


	void Start () {

	}
	

	void Update () {
		
	}

	public void StartTimer(){
		if (!timerStart) {
			timerStart = true;
			startTime = Time.time;
			star = 0;

			GetComponentInParent<PlatformManager> ().StartTimerFlag = true;
		}
	}

	public float GetCurrentTime(){
		return Time.time - startTime;
	}

	public int GetStar(){
		return star;
	}

	public float GetTimeTaken(){
		return timeTaken;
	}

	public void StopTimer(){
		endTime = Time.time;
		float pauseDuration = GetComponentInParent<PlatformManager> ().pauseDuration;
		timeTaken = Mathf.Round(endTime - startTime - pauseDuration);
		print ("Time Taken: " + timeTaken);

		if (timeTaken <= time3star) {
			star = 3;
		} else if (timeTaken <= time2star) {
			star = 2;
		} else {
			star = 1;
		}

		GetComponentInParent<PlatformManager> ().StartTimerFlag = false;
	}

	public void CalculateTime(string _scoreString, string _tileString, int _puzzleNum){

		int[] _scoreArr = new int[_scoreString.Length];
		for (int i = 0; i < _scoreString.Length; i++) {
			int.TryParse(_scoreString [i].ToString(), out _scoreArr[i]);
		}

		int[] _tileArr = new int[_tileString.Length];
		for (int i = 0; i < _tileString.Length; i++) {
			int.TryParse(_tileString [i].ToString(), out _tileArr[i]);
		}

		int numTilesToMove = 0;

		for (int i = 0; i < _scoreArr.Length; i++) {
			if (_puzzleNum == 2 && _scoreArr [i] == 1) {
				numTilesToMove++; 
			} else if(_scoreArr[i] == 1 && _tileArr[i] == 0) {
				numTilesToMove++;
			}
		}

		//print ("Number of tiles to move: " + numTilesToMove);

		time3star = numTilesToMove;
		time2star = numTilesToMove + (numTilesToMove / 2);
		time1star = 2 * numTilesToMove;
	}

	public void ResetTimer(){
		timerStart = false;
	}
}
