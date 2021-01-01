
using UnityEngine;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour, ITileInterface {

	bool isChecked = false;
	int tileCode = 0;

	SpriteRenderer myRend;

	public Color PressedColour; //7DB14AFF - 5CA119FF
	public Color TileColour; //c7dca8
	//public Color BorderTileColour; //e6eece


	void Start(){
		myRend = GetComponent<SpriteRenderer> ();
		myRend.color = TileColour;

	}

	void OnMouseDown(){

		if (!isChecked) {
			myRend.color = PressedColour;
			//transform.localScale = pressedScale;
			tileCode = 1;
		} else {
			myRend.color = TileColour;

			tileCode = 0;
		}

		isChecked = !isChecked;
	}

	public int GetTileCode(){
		return tileCode;
	}

	public void SetTileCode(int _tilecode){
		if (_tilecode == 1) {
			GetComponent<SpriteRenderer> ().color = PressedColour;

			isChecked = true;
			tileCode = 1;
		} else {
			GetComponent<SpriteRenderer> ().color = TileColour;

			isChecked = false;
			tileCode = 0;
		}
	}
}
