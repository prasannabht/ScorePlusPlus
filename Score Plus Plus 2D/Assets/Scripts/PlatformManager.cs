using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlatformManager : MonoBehaviour {

	[Header("Tiles")]
	public GameObject TileDrag;
	public GameObject BorderTileDrag;
	public float gridSpace = 0.15f;

	[Header("Platform Objects")]
	public GameObject CircleObject;
	public GameObject SquareObject;
	public GameObject HintScreenObject;
	public GameObject HintObject;
	public GameObject PauseButtonObject;
	public GameObject HomeButtonObject;
	public GameObject TimerSliderObject;
	public GameObject TimerLineObject;
	public GameObject TimerCircleObject;
	public GameObject TimeTakenText;

	[Header("Menu Items")]
	public GameObject NextButtonObject;
	public GameObject RetryButtonObject;
	public GameObject LevelsButtonObject;

	public GameObject StarObject;

	int score, puzzleNum;
	int myStar;

	//Platform objects variables
	Vector3 hintButtonPos;
	Vector3 currentScoreCirclePos, prevScoreCircleFarPos, prevScoreCircleNearPos, nextScoreCircleNearPos, nextScoreCircleFarPos;
	Vector3 timerCircle3Pos, timerCircle2Pos, timerCircle1Pos, timerSliderInitPos;
	Vector3 hintScreenSize, hintScreenCollapseSize, timerLineSize, timerLineCollapseSize;
	float timerStartCounter3, timerStartCounter2, timerStartCounter1;

	GameObject levelSquare, hintButton, hintScreen, pauseButton, homeButton;
	GameObject timerLine, timerSlider, timerCircle3Star, timerCircle2Star, timerCircle1Star;
	GameObject currentScoreCircle, prevScoreCircleFar, prevScoreCircleNear, nextScoreCircleNear, nextScoreCircleFar;
	GameObject nextButton, Star3, Star2, Star1, retryButton, levelsButton, TimeTakenObj;
	bool ShowNextButtonFlag = false;
	bool ShowStar3Flag = false;
	bool ShowStar2Flag = false;
	bool ShowStar1Flag = false;
	bool ShowLevelsFlag = false;
	bool ShowRetryFlag = false;
	GameObject TileForNextButton, TileForStar1, TileForStar2, TileForStar3, TileForRetryButton, TileForLevelsButton;
	bool Star1RotCompleteFlag = false;
	bool Star2RotCompleteFlag = false;
	bool Star3RotCompleteFlag = false;
	bool RetryPlatformFlag = false;
	float scoreCircleOffsetX = 1.5f;
	string currTileString;
	Color LevelColour, InProgressColour, NextColour, ScoreColour;

	List<GameObject> tilesToFlip = new List<GameObject> ();
	Dictionary<string, Color> colourDict;
	Dictionary<string, string> hintDict;

	DictionaryManager MyDictionary;
	ColourManager ColourManager;
	CameraBehaviour CameraManager;
	TimeManager TimeManager;
	GameManager GameManager;

	bool showScoreCircles = false;
	bool ShowHintScreenFlag = false;
	bool HideHintScreenFlag = false;
	//bool ColourTimerCirclesFlag = false;
	bool ShowStarsFlag = false;
	bool TimeTakenShown = false;
	[HideInInspector]
	public bool ShowMenuFlag = false;

	string hintStr;
	float ang = 0;
	float maxAng = 45;
	float alphaSpeed = 10;
	float circleSpeed = 5;
	float circleAlphaNear = 0.6f;
	float circleAlphaFar = 0.3f;
	bool flipTilesFlag = false;
	bool WaitForHintFlag = false;
	bool waitingFlag = false;
	bool HideHintFlag = false; 
	bool HintEnabled = true;
	bool PauseTimerFlag = false;
	bool RetrySelectedPlatformFlag = true;

	float pauseStartTime;
	[HideInInspector]
	public float pauseDuration = 0f;

	[HideInInspector]
	public bool ScoreCompleteFlag = false;

	[HideInInspector]
	public bool StartTimerFlag = false;

	[HideInInspector]
	public ScoreDetails currPlatformData;

	void Awake(){
		MyDictionary = FindObjectOfType<DictionaryManager>();
		ColourManager = FindObjectOfType<ColourManager>();
		CameraManager = FindObjectOfType<CameraBehaviour>();
		TimeManager = FindObjectOfType<TimeManager>();
		GameManager = FindObjectOfType<GameManager>();

		hintDict = MyDictionary.PopulateHintDictionary ();
		colourDict = ColourManager.PopulateColourDictionary ();

		LevelColour = colourDict ["LevelColour"];
		InProgressColour = colourDict ["InProgressColour"];
		NextColour = colourDict ["NextColour"];
		ScoreColour = colourDict ["ScoreColour"];

		currPlatformData = new ScoreDetails ();
	}

	void Start () {
		RetrySelectedPlatformFlag = false;
	}

	void Update(){
		if (showScoreCircles && CameraManager.cameraAdjusted) {

			if (ang < maxAng) {
				ang += Time.deltaTime * 300;
			} else {
				ang = maxAng;
			}
			levelSquare.transform.localRotation = Quaternion.AngleAxis (ang, Vector3.back);

			UpdatePlatformObjectProperties (currentScoreCircle, currentScoreCirclePos, 1f, true, true);
			UpdatePlatformObjectProperties (prevScoreCircleFar, prevScoreCircleFarPos, circleAlphaFar, true, true);
			UpdatePlatformObjectProperties (prevScoreCircleNear, prevScoreCircleNearPos, circleAlphaNear, true, true);
			UpdatePlatformObjectProperties (nextScoreCircleNear, nextScoreCircleNearPos, circleAlphaNear, true, true);
			UpdatePlatformObjectProperties (nextScoreCircleFar, nextScoreCircleFarPos, circleAlphaFar, true, true);

			UpdatePlatformObjectProperties (hintButton, hintButtonPos, 1f, true, false);

			UpdatePlatformObjectProperties (timerCircle3Star, timerCircle3Pos, 1f, false, false);
			UpdatePlatformObjectProperties (timerCircle2Star, timerCircle2Pos, 1f, false, false);
			UpdatePlatformObjectProperties (timerCircle1Star, timerCircle1Pos, 1f, false, false);
		
			if (prevScoreCircleFar.transform.localPosition == prevScoreCircleFarPos) {
				showScoreCircles = false;
			}

		}

		if (WaitForHintFlag && !waitingFlag) {
			if(score == 0 || score == 1)
				StartCoroutine(WaitForHint (2.0f));
			else
				StartCoroutine(WaitForHint (1.0f));
		}

		if (ShowHintScreenFlag) {
			hintScreen.transform.localScale = Vector3.Lerp (hintScreen.transform.localScale, hintScreenSize, circleSpeed * Time.deltaTime);
		}

		if (HideHintScreenFlag) {
			hintScreen.transform.localScale = Vector3.Lerp (hintScreen.transform.localScale, hintScreenCollapseSize, circleSpeed * Time.deltaTime);
		}



		if (StartTimerFlag && !PauseTimerFlag) {
			
			if(!timerSlider.activeSelf){
				timerSlider.SetActive(true);

				homeButton.SetActive (false);
				pauseButton.SetActive (true);
			}
				
			float currTime = TimeManager.GetCurrentTime () - pauseDuration;

			if (currTime < TimeManager.time3star) {
				
				timerStartCounter3 += Time.deltaTime / TimeManager.time3star;
				timerSlider.transform.localPosition = Vector3.Lerp (timerSliderInitPos, timerCircle3Star.transform.localPosition, timerStartCounter3);
				timerSlider.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour3Star"];

			} else if (currTime < TimeManager.time2star) {
				timerStartCounter2 += Time.deltaTime / (TimeManager.time2star - TimeManager.time3star);
				timerSlider.transform.localPosition = Vector3.Lerp (timerCircle3Star.transform.localPosition, timerCircle2Star.transform.localPosition, timerStartCounter2);
				timerSlider.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour2Star"];
				timerCircle3Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColourOff"];
				timerCircle3Star.GetComponentInChildren<Text> ().text = "";

			} else if (currTime < TimeManager.time1star) {
				timerStartCounter1 += Time.deltaTime / (TimeManager.time1star - TimeManager.time2star);
				timerSlider.transform.localPosition = Vector3.Lerp (timerCircle2Star.transform.localPosition, timerCircle1Star.transform.localPosition, timerStartCounter1);
				timerSlider.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour1Star"];
				timerCircle2Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColourOff"];
				timerCircle2Star.GetComponentInChildren<Text> ().text = "";
				timerCircle3Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColourOff"];
				timerCircle3Star.GetComponentInChildren<Text> ().text = "";
			} else {
				timerCircle2Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColourOff"];
				timerCircle2Star.GetComponentInChildren<Text> ().text = "";
				timerCircle3Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColourOff"];
				timerCircle3Star.GetComponentInChildren<Text> ().text = "";
			}

		}

		if (ScoreCompleteFlag) {
			UpdatePlatformObjectProperties (prevScoreCircleFar, currentScoreCirclePos, 0f, true, true);
			UpdatePlatformObjectProperties (prevScoreCircleNear, currentScoreCirclePos, 0f, true, true);
			UpdatePlatformObjectProperties (nextScoreCircleNear, currentScoreCirclePos, 0f, true, true);
			UpdatePlatformObjectProperties (nextScoreCircleFar, currentScoreCirclePos, 0f, true, true);

			Color newColor = Color.Lerp(currentScoreCircle.GetComponent<SpriteRenderer> ().color, LevelColour, alphaSpeed*Time.deltaTime);
			currentScoreCircle.GetComponent<SpriteRenderer> ().color = newColor;
			currentScoreCircle.GetComponentInChildren<Text> ().color = newColor;

			if(prevScoreCircleFar.transform.localPosition == currentScoreCircle.transform.localPosition){
				
				print ("SCORE COMPLETED.....");

				//Flip tiles to show menu items
				flipTilesFlag = true;
				ScoreCompleteFlag = false;

				HintEnabled = false;

				CameraManager.ZoomInPlatform ();

				homeButton.SetActive (true);
				pauseButton.SetActive (false);
			}
		}

		//Flip tiles to show Menu
		if (flipTilesFlag) {
			FlipTilesAndShowMenu();
		}

		//Show Menu Screen
		if (ShowStarsFlag) {


			if (!TimeTakenShown) {

				TimeTakenObj = Instantiate (TimeTakenText, transform);
				TimeTakenObj.transform.position = Star2.transform.position + new Vector3(0, 2, 0);
				TimeTakenObj.GetComponentInChildren<Text> ().text = "Completed in " + TimeManager.GetTimeTaken () + "s.";
				TimeTakenShown = true;
			}

			Quaternion _targetRotation = Quaternion.Euler (new Vector3 (0, 0, 180));

			if (myStar >= 1 && !Star1RotCompleteFlag) {
				Star1.GetComponent<SpriteRenderer>().color = Color.Lerp (Star1.GetComponent<SpriteRenderer> ().color, colourDict ["StarColour"], alphaSpeed * Time.deltaTime);

				Star1.transform.localRotation = Quaternion.Lerp (Star1.transform.localRotation, _targetRotation, 10 * Time.deltaTime);
				Star1.transform.localScale = Vector3.Lerp (Star1.transform.localScale, new Vector3 (0.6f, 0.6f, 0.6f), 10 * Time.deltaTime);
				if (Star1.transform.localRotation == _targetRotation) {
					Star1RotCompleteFlag = true;
				}
			}
			if (myStar >= 2 && !Star2RotCompleteFlag && Star1RotCompleteFlag) {
				Star2.GetComponent<SpriteRenderer>().color = Color.Lerp (Star2.GetComponent<SpriteRenderer> ().color, colourDict ["StarColour"], alphaSpeed * Time.deltaTime);

				Star2.transform.localRotation = Quaternion.Lerp (Star2.transform.localRotation, _targetRotation, 10 * Time.deltaTime);
				Star2.transform.localScale = Vector3.Lerp (Star2.transform.localScale, new Vector3 (0.6f, 0.6f, 0.6f), 10 * Time.deltaTime);
				if (Star2.transform.localRotation == _targetRotation) {
					Star2RotCompleteFlag = true;
				}
			}
			if (myStar >= 3 && !Star3RotCompleteFlag && Star2RotCompleteFlag) {
				Star3.GetComponent<SpriteRenderer>().color = Color.Lerp (Star3.GetComponent<SpriteRenderer> ().color, colourDict ["StarColour"], alphaSpeed * Time.deltaTime);

				_targetRotation = Quaternion.Euler (new Vector3 (0, 0, 180));
				Star3.transform.localRotation = Quaternion.Lerp (Star3.transform.localRotation, _targetRotation, 10 * Time.deltaTime);
				Star3.transform.localScale = Vector3.Lerp (Star3.transform.localScale, new Vector3 (0.6f, 0.6f, 0.6f), 10 * Time.deltaTime);
				if (Star3.transform.localRotation == _targetRotation) {
					Star3RotCompleteFlag = true;
				}
			}

			if ((myStar == 1 && Star1RotCompleteFlag) || (myStar == 2 && Star2RotCompleteFlag) || (myStar == 3 && Star3RotCompleteFlag))
				ShowStarsFlag = false;
		}

		//Retry enabled
		if (RetryPlatformFlag) {
			
			UpdatePlatformObjectProperties (prevScoreCircleFar, prevScoreCircleFarPos, circleAlphaFar, true, true);
			UpdatePlatformObjectProperties (prevScoreCircleNear, prevScoreCircleNearPos, circleAlphaNear, true, true);
			UpdatePlatformObjectProperties (nextScoreCircleNear, nextScoreCircleNearPos, circleAlphaNear, true, true);
			UpdatePlatformObjectProperties (nextScoreCircleFar, nextScoreCircleFarPos, circleAlphaFar, true, true);

			timerCircle3Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour3Star"];
			timerCircle2Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour2Star"];
			timerCircle1Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour1Star"];

			timerCircle3Star.GetComponentInChildren<Text> ().text = transform.GetComponent<TimeManager> ().time3star.ToString() + " s";
			timerCircle2Star.GetComponentInChildren<Text> ().text = transform.GetComponent<TimeManager> ().time2star.ToString() + " s";
			timerCircle1Star.GetComponentInChildren<Text> ().text = transform.GetComponent<TimeManager> ().time1star.ToString() + " s";

			if (prevScoreCircleFar.transform.localPosition == prevScoreCircleFarPos) {
				RetryPlatformFlag = false;
			}
		}

	}


	public void PopulatePlatform(string _tileString, int _puzzleNum){
		currTileString = _tileString;
		// if puzzle is Drag
		if (_puzzleNum == 0) {
			ShowScoreOnGrid (_tileString, false);
		}
		//Drag from whole platform
		if (_puzzleNum == 1) {
			ShowScoreOnGrid (_tileString, true);
		}
		//Drag from outside platform
		if (_puzzleNum == 2) {
			//string shuffledTileString = ShuffleTileString (GetScoreString (nextScore), currPlatform, true);
			GenerateTilesOutside (_tileString);
		}
		//--------------------------


	}
		

	public void GeneratePlatform(int width, int hight, int iterations, int _puzzleNum, int _score){

		GeneratePlatformGrids (width, hight, iterations, _puzzleNum, _score);

		GeneratePlatformBase ();

		currPlatformData.score = _score;
		currPlatformData.puzzleNum = _puzzleNum;
	}

	public void GeneratePlatformGrids(int width, int hight, int iterations, int _puzzleNum, int _score){

		puzzleNum = _puzzleNum;
		score = _score;

		GameObject currTile = new GameObject();
		GameObject currBorderTile = new GameObject();

		if (_puzzleNum == 0 || _puzzleNum == 1 || _puzzleNum == 2) {
			currTile = TileDrag;
			currBorderTile = BorderTileDrag;
		} else {
			print ("Puzzle num is not identified");
		}


		for (int k = 0; k < iterations; k++) {

			for (int i = 0; i < hight; i++) {
				for (int j = 0; j < width; j++) {
					if (j == 0 && k != 0) {
						continue;
					}
					GameObject myObj;

					if (i == 0 || i == hight - 1) {
						myObj = (GameObject)Instantiate (currBorderTile, transform);
					} else if(j == 0 || j == width - 1) {
						myObj = (GameObject)Instantiate (currBorderTile, transform);
					} else {
						myObj = (GameObject)Instantiate (currTile, transform);	
					}

					float myWidth;
					if (k != 0) {
						myWidth = width - 1;
					} else {
						myWidth = width;
					}

					float xSize = myObj.GetComponent<SpriteRenderer> ().sprite.bounds.size.x * myObj.transform.localScale.x;
					float ySize = myObj.GetComponent<SpriteRenderer> ().sprite.bounds.size.y * myObj.transform.localScale.y;
					float horizontalShift = j * xSize + j*gridSpace;
					float iterationShift = k * (xSize + gridSpace) * myWidth;
					horizontalShift = horizontalShift - (xSize + gridSpace) * (width/2) + iterationShift;
					float verticalShift = -i * ySize - i*gridSpace;
					verticalShift = verticalShift + (ySize + gridSpace) * (hight/2);
					myObj.transform.Translate (horizontalShift, verticalShift, 0);

				}

			}
		}

		Vector3 centerPoint = GameManager.GetCenterPoint(this.gameObject, true);
		float xDist = centerPoint.x - transform.position.x;
		foreach (Transform child in transform) {
			child.localPosition -= new Vector3(xDist, 0, 0);
		}

		//Add tiles to flip
		SpriteRenderer[] tiles = transform.GetComponentsInChildren<SpriteRenderer> ();
		if (score < 10) {
			tilesToFlip.Add (tiles [10].gameObject);
			tilesToFlip.Add (tiles [11].gameObject);
			tilesToFlip.Add (tiles [12].gameObject);
			tilesToFlip.Add (tiles [13].gameObject);
			tilesToFlip.Add (tiles [14].gameObject);
			tilesToFlip.Add (tiles [15].gameObject);
			tilesToFlip.Add (tiles [16].gameObject);
			tilesToFlip.Add (tiles [17].gameObject);
			tilesToFlip.Add (tiles [18].gameObject);
			tilesToFlip.Add (tiles [19].gameObject);
			tilesToFlip.Add (tiles [20].gameObject);
			tilesToFlip.Add (tiles [21].gameObject);
			tilesToFlip.Add (tiles [22].gameObject);
			tilesToFlip.Add (tiles [23].gameObject);
			tilesToFlip.Add (tiles [24].gameObject);

			TileForRetryButton = (tiles [16].gameObject);
			TileForNextButton = (tiles [17].gameObject);
			TileForLevelsButton = (tiles [18].gameObject);
			TileForStar1 = (tiles [11].gameObject);
			TileForStar2 = (tiles [12].gameObject);
			TileForStar3 = (tiles [13].gameObject);
		} else if (score < 100) {
			tilesToFlip.Add (tiles [12].gameObject);
			tilesToFlip.Add (tiles [13].gameObject);
			tilesToFlip.Add (tiles [14].gameObject);
			tilesToFlip.Add (tiles [43].gameObject);
			tilesToFlip.Add (tiles [44].gameObject);

			tilesToFlip.Add (tiles [17].gameObject);
			tilesToFlip.Add (tiles [18].gameObject);
			tilesToFlip.Add (tiles [19].gameObject);
			tilesToFlip.Add (tiles [47].gameObject);
			tilesToFlip.Add (tiles [48].gameObject);

			tilesToFlip.Add (tiles [22].gameObject);
			tilesToFlip.Add (tiles [23].gameObject);
			tilesToFlip.Add (tiles [24].gameObject);
			tilesToFlip.Add (tiles [51].gameObject);
			tilesToFlip.Add (tiles [52].gameObject);

			TileForRetryButton = (tiles [18].gameObject);
			TileForNextButton = (tiles [19].gameObject);
			TileForLevelsButton = (tiles [47].gameObject);
			TileForStar1 = (tiles [13].gameObject);
			TileForStar2 = (tiles [14].gameObject);
			TileForStar3 = (tiles [43].gameObject);
		} else if (score < 1000) {
			tilesToFlip.Add (tiles [14].gameObject);
			tilesToFlip.Add (tiles [19].gameObject);
			tilesToFlip.Add (tiles [24].gameObject);
			tilesToFlip.Add (tiles [43].gameObject);
			tilesToFlip.Add (tiles [44].gameObject);
			tilesToFlip.Add (tiles [45].gameObject);
			tilesToFlip.Add (tiles [46].gameObject);
			tilesToFlip.Add (tiles [47].gameObject);
			tilesToFlip.Add (tiles [48].gameObject);
			tilesToFlip.Add (tiles [49].gameObject);
			tilesToFlip.Add (tiles [50].gameObject);
			tilesToFlip.Add (tiles [51].gameObject);
			tilesToFlip.Add (tiles [52].gameObject);
			tilesToFlip.Add (tiles [53].gameObject);
			tilesToFlip.Add (tiles [54].gameObject);

			TileForRetryButton = (tiles [47].gameObject);
			TileForNextButton = (tiles [48].gameObject);
			TileForLevelsButton = (tiles [49].gameObject);
			TileForStar1 = (tiles [43].gameObject);
			TileForStar2 = (tiles [44].gameObject);
			TileForStar3 = (tiles [45].gameObject);
		}
			
	}



	public string GetTileString(GameObject _platform){
		string _tileString = "";
		foreach (Transform tile in _platform.transform) {
			if (tile.tag == "tile") {
				_tileString = _tileString + tile.GetComponent<ITileInterface> ().GetTileCode ();
			}
		}
		return _tileString;
	}

	public void ShowHint(){

		if (!HintEnabled) {
			return;
		}

		hintStr = GameManager.GetScoreString(score);
		int j = 0;
		foreach (Transform tile in transform) {
			if (tile.tag == "tile" && j < hintStr.Length) {
				if (hintStr [j] == '1') {
					//tile.GetComponent<SpriteRenderer> ().color = colourDict ["HighlightColour"];


					GameObject outlineTile = Instantiate(tile.gameObject, tile);
					outlineTile.transform.position = tile.position;
					outlineTile.tag = "outline";
					outlineTile.GetComponent<SpriteRenderer> ().color = colourDict ["HighlightColour"];
					outlineTile.GetComponent<TileDragBehaviour> ().enabled = false;
					outlineTile.GetComponent<BoxCollider> ().enabled = false;
					outlineTile.transform.Translate (0, 0, -0.1f);
					//outlineTile.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);

					tile.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
				}
				j++;
			}
		}

		ShowHintScreenFlag = true;

		WaitForHintFlag = true;
	}


	public void HideHint(){

		if (!HintEnabled) {
			return;
		}

		int j = 0;
		foreach (Transform tile in transform) {
			if (tile.tag == "tile" && j < hintStr.Length) {
				if (hintStr [j] == '1') {
					//tile.GetComponent<SpriteRenderer> ().color = tile.GetComponent<TileDragBehaviour> ().TileColour;
					tile.localScale = new Vector3 (1f, 1f, 1f);
					Transform outlineTile = tile.GetChild(0);
					if (outlineTile.CompareTag ("outline")) {
						Destroy (outlineTile.gameObject);
					}
				}
				j++;
			}
		}
		HideHintFlag = false;
	}

	public void SetTimeLine(){

		//Calculate time required
		string _scoreStr = GameManager.GetScoreString(score);
		string _tileStr = GetTileString(this.gameObject);

		transform.GetComponent<TimeManager> ().CalculateTime (_scoreStr, _tileStr, puzzleNum);

		timerCircle3Star.GetComponentInChildren<Text> ().text = transform.GetComponent<TimeManager> ().time3star.ToString() + " s";
		timerCircle2Star.GetComponentInChildren<Text> ().text = transform.GetComponent<TimeManager> ().time2star.ToString() + " s";
		timerCircle1Star.GetComponentInChildren<Text> ().text = transform.GetComponent<TimeManager> ().time1star.ToString() + " s";

	}

	public void GeneratePlatformBase(){

		Vector3 centerPoint = GameManager.GetCenterPoint(this.gameObject, true);
		Bounds objectBounds = GameManager.GetBounds (this.gameObject);
		float yPos; 

		levelSquare = Instantiate (SquareObject, this.transform);

		levelSquare.transform.localScale = levelSquare.transform.localScale * 1.6f;  
		levelSquare.transform.position = centerPoint + new Vector3(-objectBounds.extents.x - scoreCircleOffsetX, objectBounds.extents.y + 1f, 0);
		levelSquare.GetComponent<SpriteRenderer> ().color = LevelColour;

		currentScoreCircle = Instantiate (CircleObject, transform);
		currentScoreCirclePos = SetPlatformObjectProperties (currentScoreCircle, InProgressColour, score.ToString(), currentScoreCircle.transform.localScale * 1.4f, levelSquare.transform.position);

		prevScoreCircleFar = Instantiate (CircleObject, transform);
		float _score = score - 2;
		if (_score < 0) _score = 0;
		yPos = Mathf.Lerp (objectBounds.max.y, currentScoreCirclePos.y, 1f / 3f);
		prevScoreCircleFarPos = SetPlatformObjectProperties (prevScoreCircleFar, LevelColour, _score.ToString(), prevScoreCircleFar.transform.localScale * 1.1f, levelSquare.transform.position) + new Vector3 (0, yPos, 0);

		prevScoreCircleNear = Instantiate (CircleObject, transform);
		_score = score - 1;
		if (_score < 0) _score = 0;
		yPos = Mathf.Lerp (objectBounds.max.y, currentScoreCirclePos.y, 2f / 3f);
		prevScoreCircleNearPos = SetPlatformObjectProperties (prevScoreCircleNear, LevelColour, _score.ToString(), prevScoreCircleNear.transform.localScale * 1.1f, levelSquare.transform.position) + new Vector3 (0, yPos, 0);

		nextScoreCircleNear = Instantiate (CircleObject, transform);
		yPos = Mathf.Lerp (objectBounds.min.y, currentScoreCirclePos.y, 2f / 3f);
		nextScoreCircleNearPos = SetPlatformObjectProperties (nextScoreCircleNear, NextColour, (score + 1).ToString(), nextScoreCircleNear.transform.localScale * 1.2f, levelSquare.transform.position) + new Vector3 (0, yPos, 0);

		nextScoreCircleFar = Instantiate (CircleObject, transform);
		yPos = Mathf.Lerp (objectBounds.min.y, currentScoreCirclePos.y, 1f / 3f);
		nextScoreCircleFarPos = SetPlatformObjectProperties (nextScoreCircleFar, NextColour, (score + 2).ToString(), nextScoreCircleFar.transform.localScale * 1.1f, levelSquare.transform.position) + new Vector3 (0, yPos, 0);

		hintButton = Instantiate (HintObject, transform); 
		float xPos = objectBounds.max.x + 0.5f;
		hintButtonPos = SetPlatformObjectProperties (hintButton, levelSquare.transform.position,  0f) + new Vector3 (xPos, 0, 0);

		hintScreen = Instantiate (HintScreenObject, transform);
		hintScreen.transform.position = levelSquare.transform.position + new Vector3(1.4f, 1.5f, 0);
		hintScreenSize = hintScreen.transform.localScale;
		hintScreen.transform.localScale -= new Vector3 (hintScreen.transform.localScale.x, 0, 0);
		hintScreenCollapseSize = hintScreen.transform.localScale;



		timerCircle3Star = Instantiate (TimerCircleObject, transform);
		xPos = Mathf.Lerp (objectBounds.min.x, objectBounds.max.x, 3f / 4f);
		timerCircle3Pos = SetPlatformObjectProperties (timerCircle3Star, levelSquare.transform.position, 1f) + new Vector3 (xPos, 0, 0);
		timerCircle3Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour3Star"];

		timerCircle2Star = Instantiate (TimerCircleObject, transform);
		xPos = Mathf.Lerp (objectBounds.min.x, objectBounds.max.x, 2f / 4f);
		timerCircle2Pos = SetPlatformObjectProperties (timerCircle2Star, levelSquare.transform.position, 1f) + new Vector3 (xPos, 0, 0);
		timerCircle2Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour2Star"];

		timerCircle1Star = Instantiate (TimerCircleObject, transform);
		xPos = Mathf.Lerp (objectBounds.min.x, objectBounds.max.x, 1f / 4f);
		timerCircle1Pos = SetPlatformObjectProperties (timerCircle1Star, levelSquare.transform.position, 1f) + new Vector3 (xPos, 0, 0);
		timerCircle1Star.GetComponent<SpriteRenderer> ().color = colourDict ["TimerColour1Star"];

		timerLine = Instantiate (TimerLineObject, transform);
		timerLine.transform.position = levelSquare.transform.position + new Vector3(1.2f, 0, 0.1f);
		if (score >= 10 && score < 100) {
			timerLine.transform.localScale += new Vector3 (2.7f, 0, 0);
		} else if (score >= 100 && score < 1000) {
			timerLine.transform.localScale += new Vector3 (5.6f, 0, 0);
		}

		timerSlider = Instantiate (TimerSliderObject, transform);
		xPos = objectBounds.max.x;
		timerSliderInitPos = SetPlatformObjectProperties (timerSlider, levelSquare.transform.localPosition, 1f) + new Vector3 (xPos, 0, 0);
		timerSlider.transform.position = timerSliderInitPos;
		showScoreCircles = true;
		timerSlider.SetActive(false);
		timerStartCounter3 = timerStartCounter2 = timerStartCounter1 = 0;


		//Bring Levels buttons to front
		levelSquare.transform.position += new Vector3(0, 0, -1);

		pauseButton = Instantiate (PauseButtonObject, this.transform);
		pauseButton.transform.position = levelSquare.transform.position + new Vector3(0, 0, -1);
		pauseButton.SetActive (false);

		homeButton = Instantiate (HomeButtonObject, this.transform);
		homeButton.transform.position = levelSquare.transform.position + new Vector3(0, 0, -1);

		//Hint Screen Text
		if (score == 0) {
			hintScreen.GetComponentInChildren<Text> ().text = hintDict ["HINT_BASIC"];
		} else if (score == 1) {
			hintScreen.GetComponentInChildren<Text> ().text = hintDict ["HINT_ICON"];
		}
	}



	public void GenerateTilesOutside(string scoreString){
		int OneCount = scoreString.Split('1').Length - 1;

		//Vector3 centerPoint = GameManager.GetCenterPoint(this.gameObject, true);
		Bounds objectBounds = GameManager.GetBounds (this.gameObject);
		float minX = -objectBounds.size.x / 2;
		float maxX = objectBounds.size.x / 2;
		float minY = -objectBounds.size.y / 2;
		float maxY = objectBounds.size.y / 2;

		float myX = 0;
		float myY = 0;

		int rightTiles, bottomTiles, topTiles;
		if(OneCount / 2 <= 7)
			rightTiles = OneCount / 2;
		else
			rightTiles = 7;

		if (OneCount - rightTiles <= 5) {
			bottomTiles = OneCount - rightTiles;
			topTiles = 0;
		} else {
			bottomTiles = (OneCount - rightTiles) / 2;
			topTiles = OneCount - rightTiles - bottomTiles;
		}

		for (int i = 0; i < OneCount; i++) {
			GameObject greenTile = (GameObject)Instantiate (TileDrag, transform);
			greenTile.GetComponent<ITileInterface> ().SetTileCode (1);
			greenTile.GetComponent<TileDragBehaviour> ().dragFromOutside = true;

			Vector3 spawnPos;
			float randomPoint;
			float randomOffsetRight = Random.Range (0, 2);
			float randomOffsetX = Random.Range (-0.1f, 0.1f);
			float randomOffsetTop = Random.Range (1, 3);
			if (i < rightTiles) {
				randomPoint = Mathf.Lerp (minY, maxY - 2f, (float)i / (rightTiles - 1));
				myX = maxX + randomOffsetRight;
				myY = randomPoint + randomOffsetX;
				spawnPos = new Vector3 (myX, myY, 0);  
			} else if(i < rightTiles + bottomTiles) {
				randomPoint = Mathf.Lerp (minX + 2f, maxX - 2f, (float)(i - rightTiles) / (bottomTiles - 1));
				myX = randomPoint + randomOffsetX;
				myY = minY - randomOffsetRight;
				spawnPos = new Vector3 (myX, myY, 0);
			} else {
				randomPoint = Mathf.Lerp (minX + 2f, maxX - 2f, (float)(i - (rightTiles + bottomTiles)) / (topTiles - 1));
				myX = randomPoint + randomOffsetX;
				myY = maxY + randomOffsetTop;
				spawnPos = new Vector3 (myX, myY, 0);
			}
			greenTile.transform.localPosition = spawnPos;

			greenTile.GetComponent<BoxCollider> ().size = new Vector3 (3f, 3f, 3f);
		}

	}


	public void RetryPlatform(){
		foreach (Transform tile in this.transform) {
			if (tile.tag == "tile" || tile.tag == "bordertile") {
				tile.GetComponent<TileDragBehaviour> ().ResetTile();
				//Enable tile drag
				tile.GetComponent<TileDragBehaviour> ().EnableDrag ();
			}
		}

		CameraManager.ZoomOutPlatform ();

		currentScoreCircle.GetComponent<SpriteRenderer> ().color = InProgressColour;
		currentScoreCircle.GetComponentInChildren<Text> ().color = InProgressColour;
		 
		GetComponentInParent<TimeManager> ().ResetTimer ();

		timerStartCounter3 = timerStartCounter2 = timerStartCounter1 = 0;
		timerSlider.transform.localPosition = timerSliderInitPos;
		timerSlider.SetActive(false);

		PopulatePlatform (currTileString, puzzleNum);

		GameManager.RetryCurrentPlatform ();


		ShowNextButtonFlag = false;
		ShowStar1Flag = false;
		ShowStar2Flag = false;
		ShowStar3Flag = false;
		ShowRetryFlag = false;
		ShowStarsFlag = false;
		ShowLevelsFlag = false;

		Star1RotCompleteFlag = false;
		Star2RotCompleteFlag = false;
		Star3RotCompleteFlag = false;

		flipTilesFlag = false;

		TimeTakenShown = false;
		if (TimeTakenObj != null) {
			TimeTakenObj.GetComponentInChildren<Text> ().text = "";
		}

		HintEnabled = true;

		RetryPlatformFlag = true;
	}




	void ShowScoreOnGrid(string scoreString, bool includeBorder){

		currPlatformData.scoreString = scoreString;

		//string scoreString = GetScoreString(score);
		int[] scoreArray = new int[scoreString.Length];
		for (int i = 0; i < scoreString.Length; i++) {
			int.TryParse(scoreString [i].ToString(), out scoreArray[i]);
		}

		if (includeBorder) {
			int j = 0;
			foreach (Transform tile in this.transform) {
				if (tile.tag == "tile" || tile.tag == "bordertile" && j < scoreString.Length) {
					if (scoreArray [j] == 1)
						tile.GetComponent<ITileInterface> ().SetTileCode (scoreArray [j]);
					
					j++;
				}
			}
		} else {
			int j = 0;
			foreach (Transform tile in this.transform) {
				if (tile.tag == "tile" && j < scoreString.Length) {
					tile.GetComponent<ITileInterface> ().SetTileCode (scoreArray [j]);

					j++;
				}
			}
		}

	}


	Vector3 SetPlatformObjectProperties (GameObject _obj, Color _color, string _score, Vector3 _scale, Vector3 _pos){

		Color _color2 = _color;
		_color2.a = 0f;
		_obj.GetComponent<SpriteRenderer> ().color = _color2;
		_obj.GetComponentInChildren<Text> ().text = _score;
		_obj.GetComponentInChildren<Text> ().color = _color;
		_obj.transform.localScale = _scale; 
		_obj.transform.position = _pos;

		return new Vector3(_obj.transform.localPosition.x, 0, 0);
	}

	Vector3 SetPlatformObjectProperties (GameObject _obj, Vector3 _pos, float _alpha){
		Color _color = _obj.GetComponent<SpriteRenderer> ().color;;
		_color.a = _alpha;
		_obj.GetComponent<SpriteRenderer> ().color = _color;
		_obj.transform.position = _pos;

		return new Vector3(0, _obj.transform.localPosition.y, 0);
	}

	void UpdatePlatformObjectProperties(GameObject _obj, Vector3 _pos, float _alpha, bool _updateColor, bool _childText){

		_obj.transform.localPosition = Vector3.Lerp(_obj.transform.localPosition, _pos, circleSpeed*Time.deltaTime);

		if (_updateColor) {
			Color myColor = _obj.GetComponent<SpriteRenderer> ().color;
			Color _color = myColor;
			_color.a = _alpha;
			Color newColor = Color.Lerp (myColor, _color, alphaSpeed * Time.deltaTime);
			_obj.GetComponent<SpriteRenderer> ().color = newColor;

			if (_childText)
				_obj.GetComponentInChildren<Text> ().color = newColor;
			
		}
	}

	public List<GameObject> ListOfTiles(){
		List<GameObject> tileList = new List<GameObject> ();
		foreach (Transform tile in this.transform) {
			if (tile.CompareTag ("tile")) {
				tileList.Add (tile.gameObject);
			}
		}
		return tileList;
	}

	void FlipTilesAndShowMenu(){


		int i = 0;
		foreach (GameObject tile in tilesToFlip) {
			if (i == 0 || tilesToFlip [i - 1].GetComponent<TileDragBehaviour> ().isFlipping) {
				Quaternion _targetRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
				tile.transform.localRotation = Quaternion.Lerp (tile.transform.localRotation, _targetRotation, 8 * Time.deltaTime);
				if (!tile.GetComponent<TileDragBehaviour> ().isFlipping && tile.transform.localRotation.x < -0.5f) {
					tile.GetComponent<TileDragBehaviour> ().isFlipping = true;
					tile.GetComponent<ITileInterface> ().SetTileCode (2);
				}
				if (!tile.GetComponent<TileDragBehaviour> ().isFlipped && tile.transform.localRotation.x <= -1f) {
					tile.GetComponent<TileDragBehaviour> ().isFlipped = true;
				}
			}

			i++;
		}

//		print ("Retry? : " + RetrySelectedPlatformFlag);
//		if (RetrySelectedPlatformFlag) {
//			GameObject tempObj = TileForNextButton;
//			TileForNextButton = TileForLevelsButton;
//			TileForLevelsButton = tempObj;
//			Vector3 tempPosNext = NextButtonObject.transform.localPosition;
//			Vector3 tempPosLevels = LevelsButtonObject.transform.localPosition;
//			Vector3 tempScaleNext = NextButtonObject.transform.localScale;
//			Vector3 tempScaleLevels = LevelsButtonObject.transform.localScale;
//
//			NextButtonObject = LevelsButtonObject;
//			NextButtonObject.transform.localPosition = tempPosLevels;
//			//NextButtonObject.transform.localScale = tempScaleNext;
//
//			LevelsButtonObject = tempObj;
//			LevelsButtonObject.transform.localPosition = tempPosNext;
//			//LevelsButtonObject.transform.localScale = tempScaleLevels;
//		}

		if (!ShowNextButtonFlag && TileForNextButton.GetComponent<TileDragBehaviour> ().isFlipping) {

			GameObject TileObj = new GameObject ();
			if (RetrySelectedPlatformFlag) {
				TileObj = TileForLevelsButton;
			} else {
				TileObj = TileForNextButton;
			}

			nextButton = Instantiate (NextButtonObject, TileObj.transform);
			nextButton.transform.localRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
			if (!RetrySelectedPlatformFlag) {
				nextButton.transform.localScale = new Vector3 (1, 1, 1);
			} else {
				nextButton.transform.position += new Vector3 (1.2f, 0, 0);
			}
			ShowNextButtonFlag = true;
		}

		if (!ShowStar1Flag && TileForStar1.GetComponent<TileDragBehaviour> ().isFlipping) {
			Star1 = Instantiate (StarObject, TileForStar1.transform);
			Star1.transform.localRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
			ShowStar1Flag = true;
		}
		if (!ShowStar2Flag && TileForStar2.GetComponent<TileDragBehaviour> ().isFlipping) {
			Star2 = Instantiate (StarObject, TileForStar2.transform);
			Star2.transform.localRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
			ShowStar2Flag = true;
		}
		if (!ShowStar3Flag && TileForStar3.GetComponent<TileDragBehaviour> ().isFlipping) {
			Star3 = Instantiate (StarObject, TileForStar3.transform);
			Star3.transform.localRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
			ShowStar3Flag = true;
		}

		if (!ShowLevelsFlag && TileForLevelsButton.GetComponent<TileDragBehaviour> ().isFlipping) {
			GameObject TileObj = new GameObject ();
			if (RetrySelectedPlatformFlag) {
				TileObj = TileForNextButton;
			} else {
				TileObj = TileForLevelsButton;
			}

			levelsButton = Instantiate (LevelsButtonObject, TileObj.transform);
			levelsButton.transform.localRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
			if (RetrySelectedPlatformFlag) {
				levelsButton.transform.localScale = new Vector3 (1, 1, 1);
			} else {
				levelsButton.transform.position += new Vector3 (1, 0, 0);
			}
			ShowLevelsFlag = true;
		}
		if (!ShowRetryFlag && TileForRetryButton.GetComponent<TileDragBehaviour> ().isFlipping) {
			retryButton = Instantiate (RetryButtonObject, TileForRetryButton.transform);
			retryButton.transform.localRotation = Quaternion.Euler (new Vector3 (180, 0, 0));
			retryButton.transform.position += new Vector3 (-1, 0, 0);
			ShowRetryFlag = true;
		}
			
		if (tilesToFlip [i-1].GetComponent<TileDragBehaviour> ().isFlipped) {
			flipTilesFlag = false;
			ShowMenuFlag = true;
			ShowStarsFlag = true;
		}
	}

	IEnumerator WaitForHint(float t){
		waitingFlag = true;
		yield return new WaitForSeconds (t);
		waitingFlag = false;
		WaitForHintFlag = false;
		HideHint ();
	}

	public void PauseTimer(){
		pauseStartTime = Time.time;
		PauseTimerFlag = true;
	}

	public void ResumeTimer(){
		pauseDuration += Time.time - pauseStartTime;
		pauseStartTime = 0f;
		PauseTimerFlag = false;
	}

	public void ScoreComplete(){
		ScoreCompleteFlag = true;
		//ShowHintScreenFlag = true;

		if (GameManager.RetrySelectedPlatformFlag) {
			RetrySelectedPlatformFlag = true;
		}

		GetComponent<TimeManager> ().StopTimer ();
		//Get stars earned
		myStar = GetComponent<TimeManager>().GetStar();
		currPlatformData.scoreStar = myStar;

		//Disable Tile Drag
		foreach (Transform tile in transform) {
			if (tile.tag == "tile" || tile.tag == "bordertile") {
				tile.GetComponent<TileDragBehaviour> ().DisableDrag ();
			}
		}
	}

	public void HideHintScreen(){
		ShowHintScreenFlag = false;
		HideHintScreenFlag = true;
	}

//	public void ColourTimerCircles(){
//		ColourTimerCirclesFlag = true;
//	}

	public void FillTiles(){
		string _scoreStr = GameManager.GetScoreString(score);
		int[] scoreArray = new int[_scoreStr.Length];
		for (int i = 0; i < _scoreStr.Length; i++) {
			int.TryParse(_scoreStr [i].ToString(), out scoreArray[i]);
		}
		int j = 0;
		foreach (Transform tile in transform) {
			if (tile.tag == "tile" && j < _scoreStr.Length) {
				tile.GetComponent<ITileInterface> ().SetTileCode (scoreArray[j]);
				j++;
			}
		}
	}
}
