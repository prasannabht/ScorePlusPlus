
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TileColourBehaviour : MonoBehaviour, ITileInterface {

	bool isChecked = false;
	int tileCode = 0;

	SpriteRenderer myRend;

	public Color PressedColour; //7DB14AFF - 5CA119FF
	int ColourCount = 0;
	public List<Color> TileColours; //e0e550
	List<Color> myTileColours;
	//6d7b74 - E3E783FF
	//5d2ba2 - 9BA9A2FF
	//ef7d45 - AB8ED2FF
	// E7A17FFF

	//public Color BorderTileColour; //e6eece

	void Awake(){
		TileColours.Add (PressedColour);
		myTileColours = ShuffleList (TileColours);
		myRend = GetComponent<SpriteRenderer> ();
		myRend.color = TileColours[0];

		if (myRend.color == PressedColour)
			tileCode = 1;
	}

	List<Color> ShuffleList(List<Color> myList){
		for (int i = 0; i < myList.Count; i++) {
			Color temp = myList [i];
			int randomIndex = Random.Range (i, myList.Count);
			myList [i] = myList [randomIndex];
			myList [randomIndex] = temp;
		}
		return myList;
	}

	void OnMouseDown(){
		ColourCount = (ColourCount + 1) % myTileColours.Count;
		myRend.color = myTileColours[ColourCount];

		if (myRend.color == PressedColour)
			tileCode = 1;
		else
			tileCode = 0;
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
			GetComponent<SpriteRenderer> ().color = myTileColours[0];

			isChecked = false;
			tileCode = 0;
		}
	}
}
