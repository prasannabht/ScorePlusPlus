using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject PlatformInstantiator;
	public GameObject StartScreenPlatform;
	public GameObject HomePlatform;
	public GameObject HomePlatformLandscape;
	public GameObject PauseMenu;
	public GameObject SelectedLevel;

	[Header("Generate Platform")]
	public float platformShiftOffset = 6f;
	public float platformShiftOffset2 = 8f;
	public float platformShiftOffset3 = 10f;
	float currPlatformShiftOffset;

	static Dictionary<int, string> scoreDict;
	Dictionary<int, string> puzzleDict;
	//Dictionary<string, Color> colorDict;

	public int currScore = 0;
	int retryScore, nextScore;
	static GameObject currPlatform;
	static GameObject currScreen;
	static GameObject homeScreen;
	int puzzleNum;

	[HideInInspector]
	public bool StartGameFlag = false;
	[HideInInspector]
	public bool GoToNextPlatform = false;
	[HideInInspector]
	public bool ScoreCompleteFlag = false;


	bool ShowHomeScreenFlag = false;
	//bool WaitForHintFlag = false;

	[HideInInspector]
	public bool RetrySelectedPlatformFlag = false;

	bool RetryCurrentPlatformFlag = false;

	string tileString, ScoreString;
	string shuffledTileString;


	CameraBehaviour CameraManager;
	//ColourManager ColourManager;
	DictionaryManager MyDictionary;
	ScoreHistoryManager ScoreHistoryManager;

	void Awake(){
		CameraManager = FindObjectOfType<CameraBehaviour>();
		MyDictionary = FindObjectOfType<DictionaryManager>();
		//ColourManager = FindObjectOfType<ColourManager>();
		ScoreHistoryManager = FindObjectOfType<ScoreHistoryManager>();
	}


	void Start () {

		print ("next score: " + nextScore);
		scoreDict = MyDictionary.PopulateScoreDictionary ();
		puzzleDict = MyDictionary.PopulatePuzzleTypeDictionary ();
		//colorDict = ColourManager.PopulateColourDictionary ();

		//nextScore = 1;

		if (!PlayerPrefs.HasKey ("score")) {
			PlayerPrefs.SetInt ("score", nextScore);
		}
		nextScore = PlayerPrefs.GetInt ("score");
		currScore = nextScore;
		print ("curr score: " + currScore);

		ShowHomeScreenFlag = true;
		//GameObject platformObjNew = new GameObject ();
		if (CameraManager.IsLandscapeMode) {
			print ("LandScape Mode. Resize Home screen");
			homeScreen = (GameObject)Instantiate(HomePlatformLandscape);
		} else {
			homeScreen = (GameObject)Instantiate(HomePlatform);
		}


		Dictionary<int, ScoreDetails> ScoreDetailsDict = ScoreHistoryManager.GetScoreDictionary();

		if (CameraManager.IsLandscapeMode) {
			homeScreen.GetComponentInChildren<GridSetter>().PopulateGrid (ScoreDetailsDict, true);
		} else {
			homeScreen.GetComponentInChildren<GridSetter>().PopulateGrid (ScoreDetailsDict, false);
		}



		currPlatform = homeScreen;

		StartGameFlag = false;
		GoToNextPlatform = false;
	}

	 

	  
	void Update () {

		//If escape is pressed
		if (Input.GetKeyDown (KeyCode.Escape)) {
			print ("Pressed Escape");
			if (StartGameFlag) {
				StartGameFlag = false;
				GoToHomePlatform ();
			} else {
				print ("Quit game");
				Application.Quit();
			}
		}

		if (ShowHomeScreenFlag) {
			if (CameraManager.IsLandscapeMode && !homeScreen.CompareTag ("landscape")) {
				Destroy (homeScreen);
				GoToHomePlatform ();
			}
			if (!CameraManager.IsLandscapeMode && homeScreen.CompareTag ("landscape")) {
				Destroy (homeScreen);
				GoToHomePlatform ();
			}
		}
			
		if (!StartGameFlag) {
			return;
		}

		if (RetrySelectedPlatformFlag || RetryCurrentPlatformFlag) {
			currScore = retryScore;
			//ScoreCompleteFlag = false;
		}
			
		if (GoToNextPlatform) {

//			ScoreCompleteFlag = false;
			ShowHomeScreenFlag = false;

			puzzleNum = GetPuzzleNum (currScore);

			//Generate new platform and move camera to that position
			GameObject newPlatform = CreateAndPopulateNewPlatform (currScore, puzzleNum);

			//Position the platform
			newPlatform.transform.position = GetNextPlatformPos (currPlatform, newPlatform);
			//newPlatform.transform.Rotate (0, 0, Random.Range (-90.0f, 0f));


			if (currScore == 0 || currScore == 1) {
				newPlatform.GetComponent<PlatformManager> ().ShowHint ();
			}
			newPlatform.GetComponent<PlatformManager> ().ShowMenuFlag = false;

			int zoomType = 1;
			if (currScore < 10) {
				zoomType = 1;
			} else if (currScore < 100) {
				zoomType = 2;
			} else if (currScore < 1000) {
				zoomType = 3;
			}

			float zoomOffSet = 0;
			if (puzzleNum == 2) {
				zoomOffSet = 3;
			}

	
			FindObjectOfType<CameraBehaviour> ().MoveCamera (newPlatform, currPlatform, zoomType, zoomOffSet, puzzleNum);
			currPlatform = newPlatform;

			//Destroy previous Platform
			//DestroyPlatform(currPlatform);

			//Asign new platform as currPlatform


			GoToNextPlatform = false;
		}

		tileString = "";
		ScoreString = GetScoreString (currScore);

		//Get current tile code to compare
		tileString = GetTileString(currPlatform);

//		print ("Score: " + currScore);
//		print ("Tile String: " + tileString);
//		print ("Score String: " + ScoreString);
		if (tileString == ScoreString) {
			tileString = "";

			if (!ScoreCompleteFlag) {
				print ("Score completed!");
		
				currPlatform.GetComponent<PlatformManager> ().ScoreComplete ();

				//save current platform details
				ScoreDetails _platformData = currPlatform.GetComponent<PlatformManager>().currPlatformData;
				print ("Add Details to File: " + "Score: " + _platformData.score + ",  ScoreStar: " + _platformData.scoreStar + ", ScoreTime: " + _platformData.scoreTime);
				ScoreHistoryManager.SavePlatformData (_platformData);

//				if (RetrySelectedPlatformFlag) {
//					currScore = nextScore - 1;
//					RetrySelectedPlatformFlag = false;
//				}
//
				if (!RetryCurrentPlatformFlag && !RetrySelectedPlatformFlag) {

					//Increment next score
					print ("Increment next score");
					nextScore++;
					currScore = nextScore;
					PlayerPrefs.SetInt ("score", nextScore);


				}

				ScoreCompleteFlag = true;
			}
		}
	}


	GameObject CreateAndPopulateNewPlatform(int _score, int _puzzleNum){
		print ("GoToNextPlatform");
		GameObject platformObjNew = (GameObject)Instantiate(PlatformInstantiator);

		if (_score >= 100) {
			currPlatformShiftOffset = platformShiftOffset3;
			platformObjNew.GetComponent<PlatformManager>().GeneratePlatform (5, 7, 3, _puzzleNum, _score);
		} else if (_score >= 10) {
			currPlatformShiftOffset = platformShiftOffset2;
			platformObjNew.GetComponent<PlatformManager>().GeneratePlatform (5, 7, 2, _puzzleNum, _score);
		} else {
			currPlatformShiftOffset = platformShiftOffset;
			platformObjNew.GetComponent<PlatformManager>().GeneratePlatform (5, 7, 1, _puzzleNum, _score);
		}

		shuffledTileString="";
		// if puzzle is Drag
		if (_puzzleNum == 0)
			shuffledTileString = ShuffleTileString (GetScoreString (_score), platformObjNew, false);
		//Drag from whole platform
		if (_puzzleNum == 1)
			shuffledTileString = ShuffleTileString (GetScoreString (_score), platformObjNew, true);
		//Drag from outside platform
		if (_puzzleNum == 2)
			shuffledTileString = GetScoreString (_score);

		//Set Tiles
		platformObjNew.GetComponent<PlatformManager>().PopulatePlatform(shuffledTileString, _puzzleNum);

		//Set Timer
		platformObjNew.GetComponent<PlatformManager>().SetTimeLine();


		return platformObjNew;
	}


	int GetPuzzleNum(int _score){
		int num;

		if (_score == 0) {
			num = 2;
		} else if (_score == 1) {
			num = 1;
		} else {
			num = Random.Range(0, puzzleDict.Count);
		}

		return num;
	}
		


	Vector3 GetNextPlatformPos(GameObject _currPlatform, GameObject nextPlatform){

		Vector3 platformPos;
		if (_currPlatform == null) {
			platformPos = Camera.main.transform.position;
		} else {
			platformPos = _currPlatform.transform.position;
		}

		int tempD = Random.Range(0, 2);
		Bounds bound = GetBounds (nextPlatform);
		Vector3 boundSize = bound.max - bound.min;
		if (tempD == 0) {
			platformPos = platformPos + new Vector3 (0, boundSize.y + currPlatformShiftOffset, 0);	
		} else {
			platformPos = platformPos + new Vector3 (boundSize.x + currPlatformShiftOffset, 0, 0);	
		}

		return platformPos;
	}



	string ShuffleTileString(string tileString, GameObject platform, bool includeBorder){
		string shuffledString = tileString;
		if (includeBorder) {
			string borderStr = "";
			foreach (Transform borderTile in platform.transform) {
				if (borderTile.tag == "bordertile") {
					borderStr += "0";
				}
			}
			tileString += borderStr; 
		}

		char[] tileStringArr = tileString.ToCharArray ();
		List<char> tileList = new List<char> (tileStringArr);
		List<char> shuffledList = new List<char> ();

		for (int i = tileList.Count; i > 0; i--) {
			int index = Random.Range (0, i);
			shuffledList.Add (tileList [index]);
			tileList [index] = tileList [i - 1];
		}
		shuffledString = new string (shuffledList.ToArray ());
		return shuffledString;
	}


	public void GoToHomePlatform(){

		ShowHomeScreenFlag = true;
		//GameObject platformGrid = new GameObject ();
		if (CameraManager.IsLandscapeMode) {
			print ("LandScape Mode. Resize Home screen");
			homeScreen = (GameObject)Instantiate(HomePlatformLandscape);
		} else {
			homeScreen = (GameObject)Instantiate(HomePlatform);
		}

		Bounds bound = GetBounds (currPlatform);
		Bounds gridBound = GetBounds (homeScreen);

		float YShift = bound.extents.y + gridBound.extents.y + 1f;
		print ("Show Levels"); 
		homeScreen.transform.position = currPlatform.transform.position;
		homeScreen.transform.rotation = currPlatform.transform.rotation;

		homeScreen.transform.Translate (0, -YShift, 0);

		Dictionary<int, ScoreDetails> ScoreDetailsDict = ScoreHistoryManager.GetScoreDictionary();

		if (CameraManager.IsLandscapeMode) {
			homeScreen.GetComponentInChildren<GridSetter>().PopulateGrid (ScoreDetailsDict, true);
		} else {
			homeScreen.GetComponentInChildren<GridSetter>().PopulateGrid (ScoreDetailsDict, false);
		}


//		//Destroy current screen
//		DestroyPlatform(currPlatform);
		if (currScreen != null) {
			DestroyPlatform (currPlatform);
			currPlatform = currScreen;

		}

		//Disable Start Game Flag
		StartGameFlag = false;

		//Reset currScore
		if (RetrySelectedPlatformFlag) {
			currScore = nextScore;
			RetrySelectedPlatformFlag = false;
		}
			
		CameraManager.MoveCamera (homeScreen, currPlatform, 1, 0, 0);
		currPlatform = homeScreen;
	}

	public void GoBackToPlatform(){
		if (StartGameFlag) {
			currPlatform.GetComponent<PlatformManager> ().ResumeTimer ();
			CameraManager.MoveCamera (currPlatform, currScreen);

		} else {
			CameraManager.MoveCamera (currPlatform, currScreen);
		}
		//DestroyPlatform (currScreen);
	}

	public void GoToNextPlatformFunction(){

		if (RetryCurrentPlatformFlag) {
			print ("Increment next score after Retry");
			//nextScore++;
			currScore = nextScore;
			PlayerPrefs.SetInt ("score", nextScore);

			RetryCurrentPlatformFlag = false;
		}

		if (RetrySelectedPlatformFlag) {

			print ("Increment next score after Retry");
			//nextScore++;
			currScore = nextScore;
			PlayerPrefs.SetInt ("score", nextScore);

			RetrySelectedPlatformFlag = false;
		}

		ScoreCompleteFlag = false;
		ShowHomeScreenFlag = false;

		puzzleNum = GetPuzzleNum (currScore);

		//Generate new platform and move camera to that position
		GameObject newPlatform = CreateAndPopulateNewPlatform (currScore, puzzleNum);

		//Position the platform
		newPlatform.transform.position = GetNextPlatformPos (currPlatform, newPlatform);
		//newPlatform.transform.Rotate (0, 0, Random.Range (-90.0f, 0f));

		if (currScore == 0 || currScore == 1) {
			newPlatform.GetComponent<PlatformManager> ().ShowHint ();
		}
		newPlatform.GetComponent<PlatformManager> ().ShowMenuFlag = false;

		int zoomType = 1;
		if (currScore < 10) {
			zoomType = 1;
		} else if (currScore < 100) {
			zoomType = 2;
		} else if (currScore < 1000) {
			zoomType = 3;
		}

		float zoomOffSet = 0;
		if (puzzleNum == 2) {
			zoomOffSet = 3;
		}


		FindObjectOfType<CameraBehaviour> ().MoveCamera (newPlatform, currPlatform, zoomType, zoomOffSet, puzzleNum);
		currPlatform = newPlatform;
	}


	public void GoToPauseMenu(){
		GameObject pauseMenu = (GameObject)Instantiate(PauseMenu);

		Bounds bound = GetBounds (currPlatform);
		float XShift = bound.size.x;

		pauseMenu.transform.position = currPlatform.transform.position;
		pauseMenu.transform.rotation = currPlatform.transform.rotation;

		if (currScore < 10) {
			pauseMenu.transform.Translate (XShift/2 + 5, 0, 0);
		} else if (currScore >= 10 && currScore < 100) {
			pauseMenu.transform.localScale = new Vector3 (2, 2, 2);
			pauseMenu.transform.Translate (XShift/2 + 10, 0, 0);
		} else if (currScore >= 100 && currScore < 1000) {
			if (!CameraManager.IsLandscapeMode) {
				pauseMenu.transform.localScale = new Vector3 (3, 3, 3);
			} else {
				pauseMenu.transform.localScale = new Vector3 (2f, 2f, 2f);
			}
			pauseMenu.transform.Translate (XShift/2 + 15, 0, 0);
		}



		currScreen = pauseMenu;

		currPlatform.GetComponent<PlatformManager> ().PauseTimer ();
		CameraManager.MoveCamera (currScreen);
	}

	public void RetryCurrentPlatform(){
		print ("Retry current platform:");

		if (RetrySelectedPlatformFlag)
			retryScore = currScore;
		else
			retryScore = currScore - 1;
		
		print (retryScore);
		RetryCurrentPlatformFlag = true;

		ScoreCompleteFlag = false;
	}

	public void RetrySelectedPlatform(ScoreDetails _platform){
		print ("Retry platform:");
		print ("Score: " + _platform.score);
		print ("PuzzleNum: " + _platform.puzzleNum);

		//Generate new platform and move camera to that position
		GameObject newPlatform = CreateAndPopulateNewPlatform (_platform.score, _platform.puzzleNum);

		//Position the platform
		newPlatform.transform.position = GetNextPlatformPos (currPlatform, newPlatform);
		//newPlatform.transform.Rotate (0, 0, Random.Range (-90.0f, 0f));

		newPlatform.GetComponent<PlatformManager> ().ShowMenuFlag = false;

		//Set RetrySelectedPlatform Flag
		ShowHomeScreenFlag = false;
		RetrySelectedPlatformFlag = true;
		StartGameFlag = true;
		retryScore = _platform.score;

		ScoreCompleteFlag = false;
//		//Destroy Selection Screen
//		DestroyPlatform(currPlatform);


		int zoomType = 1;
		if (_platform.score < 10) {
			zoomType = 1;
		} else if (_platform.score < 100) {
			zoomType = 2;
		} else if (_platform.score < 1000) {
			zoomType = 3;
		}

		float zoomOffSet = 0;
		if (_platform.puzzleNum == 2) {
			zoomOffSet = 3;
		}


		FindObjectOfType<CameraBehaviour> ().MoveCamera (newPlatform, currPlatform, zoomType, zoomOffSet, _platform.puzzleNum);
		currPlatform = newPlatform;

	}


	public void ShowSelectedLevel(){
		GameObject selectedLevel = currScreen.transform.Find("Canvas").Find ("RetryScoreScreen").gameObject;
		selectedLevel.SetActive (true);

	}

	public void HideSelectedLevel(){
		GameObject selectedLevel = currScreen.transform.Find("Canvas").Find ("RetryScoreScreen").gameObject;
		selectedLevel.SetActive (false);

	}


	Vector3 GetPlatformGridPos(GameObject _currPlatform){

		Vector3 tempPos;
		float YShift;
		if (_currPlatform == null) {
			tempPos = Camera.main.transform.position;
			YShift = 0;
		} else {
			tempPos = _currPlatform.transform.position;
			Bounds bound = GetBounds (_currPlatform);
			YShift = bound.min.y;
		}

		tempPos = tempPos - new Vector3 (0, YShift, 0);	


		return tempPos;
	}


	public Bounds GetBounds(GameObject _platform){

		Quaternion currRot = _platform.transform.rotation;
		Bounds bounds = new Bounds (_platform.transform.position, Vector3.zero);
		_platform.transform.rotation = Quaternion.Euler (0, 0, 0);

		SpriteRenderer[] tiles = _platform.transform.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer tile in tiles) {
			bounds.Encapsulate (tile.bounds);
		}

		_platform.transform.rotation = currRot;
		return bounds;
	}

	public Vector3 GetCenterPoint(GameObject _platform, bool boundsNeeded){

		if (boundsNeeded) {

			Transform[] tiles = _platform.transform.GetComponentsInChildren<Transform> ();
			if (tiles.Length == 1) {
				return tiles [0].position;
			}

			var bounds = new Bounds (tiles [0].position, Vector3.zero);
			for (int i = 0; i < tiles.Length; i++) {
				bounds.Encapsulate (tiles [i].position);
			}
			return bounds.center;
		} else {
			return _platform.transform.position;
		}
	}


	public string GetScoreString(int score){
		if (score < 10) {
			return scoreDict [score];
		} else if (score < 100) {
			int num1 = score / 10;
			int num2 = score % 10;

			string numString = scoreDict [num1] + scoreDict [num2];
			return numString;

		} else {
			int num1 = score / 100;
			int tempNum = score % 100;
			int num2 = tempNum / 10;
			int num3 = tempNum % 10;

			string numString = scoreDict [num1] + scoreDict [num2] + scoreDict [num3];
			return numString;
		}
	}

	public string GetTileString(GameObject _platform){
		string _tileString = "";
		foreach (Transform tile in _platform.transform) {
			if (tile.tag == "tile")
				_tileString = _tileString + tile.GetComponent<ITileInterface> ().GetTileCode ();
		}
		return _tileString;
	}

	public string GetCurrentScoreString(){
		return ScoreString;
	}

	public string GetCurrentTileString(){
		return tileString;
	}

	public int GetCurrentPuzzleNum(){
		return puzzleNum;
	}


	void DestroyPlatform(GameObject _platform){
		Destroy (_platform);
	}
}
