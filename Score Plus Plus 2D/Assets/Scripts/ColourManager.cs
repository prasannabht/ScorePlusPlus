using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour {

	[Header("Timer Colours")]
	public Color TimerColour3Star;
	public Color TimerColour2Star;
	public Color TimerColour1Star;
	public Color TimerColourOff;

	[Header("Level Colours")]
	public Color LevelColour;
	public Color InProgressColour;
	public Color NextColour;
	public Color ScoreColour;

	[Header("Tile Colours")]
	public Color PressedColour;
	public Color TileColour;
	public Color BorderTileColour;
	public Color FlippedColour;
	public Color HighlightColour;

	[Header("Stars Colours")]
	public Color StarColour;

	[Header("ScoreList Colours")]
	public Color ScoreListColour3;
	public Color ScoreListColour2;
	public Color ScoreListColour1;

	public Dictionary<string, Color> PopulateColourDictionary(){
		Dictionary<string, Color> tempDict = new Dictionary<string, Color>();

		tempDict.Add ("TimerColour3Star", TimerColour3Star);
		tempDict.Add ("TimerColour2Star", TimerColour2Star);
		tempDict.Add ("TimerColour1Star", TimerColour1Star);
		tempDict.Add ("TimerColourOff", TimerColourOff);

		tempDict.Add ("LevelColour", LevelColour);
		tempDict.Add ("InProgressColour", InProgressColour);
		tempDict.Add ("NextColour", NextColour);
		tempDict.Add ("ScoreColour", ScoreColour);

		tempDict.Add ("PressedColour", PressedColour);
		tempDict.Add ("TileColour", TileColour);
		tempDict.Add ("BorderTileColour", BorderTileColour);
		tempDict.Add ("FlippedColour", FlippedColour);
		tempDict.Add ("HighlightColour", HighlightColour);

		tempDict.Add ("StarColour", StarColour);

		tempDict.Add ("ScoreListColour3", ScoreListColour3);
		tempDict.Add ("ScoreListColour2", ScoreListColour2);
		tempDict.Add ("ScoreListColour1", ScoreListColour1);


		return tempDict;
	}
}
