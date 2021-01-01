using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GridSetter : MonoBehaviour {

	public GameObject ScoreListItem;
	public GameObject ScoreListItemLandscape;
//	public GameObject Star1Object;
//	public GameObject Star2Object;
//	public GameObject Star3Object;

	ColourManager ColourManager;
	Dictionary<string, Color> colourDict;

	void Awake(){
		ColourManager = FindObjectOfType<ColourManager>();
		colourDict = ColourManager.PopulateColourDictionary ();
	}

	void Start(){

	}

	public void PopulateGrid(Dictionary<int, ScoreDetails> ScoreDetailsDict, bool isLandscape){

		foreach (KeyValuePair<int, ScoreDetails> _scoreDetails in ScoreDetailsDict) {

			GameObject scoreItem;

			if (isLandscape) {
				scoreItem = (GameObject)Instantiate (ScoreListItemLandscape, transform);
			} else {
				scoreItem = (GameObject)Instantiate (ScoreListItem, transform);
			}
				

			scoreItem.GetComponentInChildren<Text> ().text = _scoreDetails.Key.ToString();

			if (_scoreDetails.Key < 10) {
				scoreItem.GetComponentInChildren<Text> ().fontSize = 200;
			} else if (_scoreDetails.Key < 100) {
				scoreItem.GetComponentInChildren<Text> ().fontSize = 150;
			} else {
				scoreItem.GetComponentInChildren<Text> ().fontSize = 100;
			}

			GameObject starObj1 = new GameObject();
			GameObject starObj2 = new GameObject();
			GameObject starObj3 = new GameObject();

			foreach (Transform child in scoreItem.transform) {
				if (child.CompareTag ("1star")) {
					starObj1 = child.gameObject;
				} else if (child.CompareTag ("2star")) {
					starObj2 = child.gameObject;
				} else if (child.CompareTag ("3star")) {
					starObj3 = child.gameObject;
				}
			}

			if (_scoreDetails.Value.scoreStar == 1) {
				scoreItem.GetComponent<Image> ().color = colourDict ["ScoreListColour1"];
				starObj1.SetActive (true);
			} else if (_scoreDetails.Value.scoreStar == 2) {
				scoreItem.GetComponent<Image> ().color = colourDict ["ScoreListColour2"];
				starObj2.SetActive (true);
			} else if (_scoreDetails.Value.scoreStar == 3) {
				scoreItem.GetComponent<Image>().color = colourDict ["ScoreListColour3"];
				starObj3.SetActive (true);
			} 
		}

	}
}
