//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDragBehaviour : MonoBehaviour, ITileInterface {

	bool firstDrag = false;
	bool isChecked = false;

	public bool isFlipping = false;
	public bool isFlipped = false;

	[HideInInspector]
	public int tileCode = 0;

	SpriteRenderer myRend;

	Color PressedColour, BorderTileColour, FlippedColour;
	[HideInInspector]
	public Color TileColour;

	bool drag = false;
	bool dragAllowed = true;
	bool moveTiles = false;
	bool backToStart = false;

	[HideInInspector]
	public bool dragFromOutside = false;

	[HideInInspector]
	public Vector3 startPos;
	Quaternion startRotation;
	Transform platform;
	GameObject tileToDrop;
	Vector3 tileMovePos = new Vector3();

	Dictionary<string, Color> colorDict;
	ColourManager ColourManager;

	void Awake () {
		ColourManager = FindObjectOfType<ColourManager>();
		colorDict = ColourManager.PopulateColourDictionary ();

		PressedColour = colorDict ["PressedColour"];
		TileColour = colorDict ["TileColour"];
		BorderTileColour = colorDict ["BorderTileColour"];
		FlippedColour = colorDict ["FlippedColour"];

		myRend = GetComponent<SpriteRenderer> ();
		if(this.tag == "tile")
			myRend.color = TileColour;
		else if (this.tag == "bordertile")
			myRend.color = BorderTileColour;
	}

	void Start(){
		startPos = transform.position;
		platform = transform.parent;
		startRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (drag && !isFlipped) {
			Vector3 positionOfScreen = Camera.main.WorldToScreenPoint(transform.position);
			Vector3 tilePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, positionOfScreen.z);
			transform.position = Camera.main.ScreenToWorldPoint((Vector3)tilePos);

			List<GameObject> tiles = platform.GetComponent<PlatformManager> ().ListOfTiles ();
			float shortestDist = Mathf.Infinity;
			GameObject nearestTile = null;

			foreach (GameObject tile in tiles) {
				if (tile != this.gameObject && tile.GetComponent<ITileInterface>().GetTileCode() != 1) {
					float dist = Vector3.Distance (transform.position, tile.transform.position);

					if (dist < shortestDist) {
						shortestDist = dist;
						nearestTile = tile;

					}
				}
			}

			if (Vector3.Distance (transform.position, startPos) < shortestDist) {
				backToStart = true;
			} else {
				backToStart = false;
			}
			tileToDrop = nearestTile;
			tileMovePos = tileToDrop.transform.position;
		}

		if (moveTiles) {

			if (dragFromOutside) {
				Destroy (this.gameObject);
				return;
			} else {
				transform.position = Vector3.MoveTowards (transform.position, startPos, Time.deltaTime * 20f);
				if (transform.position == startPos) {
					moveTiles = false;
				}
			}

		}
	}

	void OnMouseDown(){

		if (dragAllowed && !isFlipped) {
			drag = true;
			backToStart = false;
			if (!firstDrag) {
				firstDrag = true;
				GetComponentInParent<TimeManager> ().StartTimer ();
				GetComponentInParent<PlatformManager> ().HideHintScreen ();
				//GetComponentInParent<PlatformManager> ().ColourTimerCircles ();

			}
			transform.position = new Vector3 (transform.position.x, transform.position.y, -0.1f);
		}
	}

	void OnMouseUp(){
		if (dragAllowed && !isFlipped) {
			drag = false;
			if (backToStart) {	 
				transform.position = startPos;
				moveTiles = false;
			} else {
				int tempCode = GetTileCode ();
				SetTileCode (tileToDrop.GetComponent<ITileInterface> ().GetTileCode ());
				tileToDrop.GetComponent<ITileInterface> ().SetTileCode (tempCode);
				tileToDrop.GetComponent<BoxCollider> ().size = new Vector3 (2, 2, 2);
				moveTiles = true;
			}

			transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		}
	}

	public int GetTileCode(){
		return tileCode;
	}

	public void SetTileCode(int _tilecode){
		if (_tilecode == 1) {
			GetComponent<SpriteRenderer> ().color = PressedColour;
			isChecked = true;
			tileCode = 1;
		} else if (_tilecode == 2) {
			GetComponent<SpriteRenderer> ().color = FlippedColour;
			isChecked = false;
			tileCode = 2;
		} else {
			if(this.tag == "tile")
				GetComponent<SpriteRenderer> ().color = TileColour;
			else if (this.tag == "bordertile")
				GetComponent<SpriteRenderer> ().color = BorderTileColour;
			

			isChecked = false;
			tileCode = 0;
		}
	}

	public void ResetTile(){
		SetTileCode (0);
		transform.rotation = startRotation;
		isFlipped = false;
		isFlipping = false;
		drag = false;
		firstDrag = false;
		foreach (Transform tileChild in transform){
			Destroy (tileChild.gameObject);
		}
	}

	public void DisableDrag(){
		dragAllowed = false;
	}

	public void EnableDrag(){
		dragAllowed = true;
	}

}
