using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	GameManager GameManager;


	void Awake(){
		GameManager = FindObjectOfType<GameManager>();
	}

	public void StartGame(){
		print ("Start Game");
		FindObjectOfType<GameManager>().StartGameFlag = true;
//		FindObjectOfType<GameManager>().GoToNextPlatform = true;
		FindObjectOfType<GameManager>().GoToNextPlatformFunction();

	}

	public void ClearScore(){
		PlayerPrefs.DeleteAll();
		FindObjectOfType<ScoreHistoryManager> ().ClearPlatformList ();
	}

	public void FillTiles(){
		print ("Auto Fill");
		FindObjectOfType<PlatformManager> ().FillTiles ();
	}

	public void ShowHint(){
		print ("show hint");
		FindObjectOfType<PlatformManager> ().ShowHint ();
	}

	public void HideHintScreen(){
		print ("hide hint screen");
		FindObjectOfType<PlatformManager> ().HideHintScreen ();
	}

	public void GoToNextPlatform(){
		print ("GO TO NEXT PLATFORM");
		FindObjectOfType<GameManager>().GoToNextPlatformFunction();
	}

	public void RetryPlatform(){
		print ("RETRY THIS PLATFORM");
		FindObjectOfType<PlatformManager>().RetryPlatform();
	}

	public void RetryPlatform(GameObject _score){
		print ("RETRY SELECTED PLATFORM");

		int myScore = int.Parse(_score.GetComponent<Text> ().text);
		print (myScore);
		Dictionary<int, ScoreDetails> _scoreDict = FindObjectOfType<ScoreHistoryManager> ().GetScoreDictionary();
		FindObjectOfType<GameManager>().RetrySelectedPlatform(_scoreDict[myScore]);
	}

	public void GoToHomePlatform(){
		print ("Go to Home Platform");
		FindObjectOfType<GameManager>().GoToHomePlatform();
	}

	public void ShowSelectedLevelDetails(){
		print ("Show Selected Level");
		FindObjectOfType<GameManager>().ShowSelectedLevel();
	}

	public void GoBackToLevelsScreen(){
		print ("Go Back to levels screen");
		FindObjectOfType<GameManager>().HideSelectedLevel();
	}

	public void GoBackToPlatform(){
		print ("Go Back To Platform");
		FindObjectOfType<GameManager>().GoBackToPlatform();
	}

	public void GoToPauseMenu(){
		print ("Go to pause menu");
		FindObjectOfType<GameManager>().GoToPauseMenu();
	}
}
