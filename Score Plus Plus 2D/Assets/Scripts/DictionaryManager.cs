using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryManager : MonoBehaviour {

	public Dictionary<int, string> PopulateScoreDictionary(){
		Dictionary<int, string> tempDict = new Dictionary<int, string>();

		tempDict.Add
		(0,
			"111" +
			"101" +
			"101" +
			"101" +
			"111"
		);

		tempDict.Add
		(1,
			"110" +
			"010" +
			"010" +
			"010" +
			"010"
		);

		tempDict.Add
		(2,
			"111" +
			"001" +
			"111" +
			"100" +
			"111"
		);

		tempDict.Add
		(3,
			"111" +
			"001" +
			"111" +
			"001" +
			"111"
		);

		tempDict.Add
		(4,
			"101" +
			"101" +
			"111" +
			"001" +
			"001"
		);

		tempDict.Add
		(5,
			"111" +
			"100" +
			"111" +
			"001" +
			"111"
		);

		tempDict.Add
		(6,
			"111" +
			"100" +
			"111" +
			"101" +
			"111"
		);

		tempDict.Add
		(7,
			"111" +
			"001" +
			"001" +
			"001" +
			"001"
		);

		tempDict.Add
		(8,
			"111" +
			"101" +
			"111" +
			"101" +
			"111"
		);

		tempDict.Add
		(9,
			"111" +
			"101" +
			"111" +
			"001" +
			"111"
		);
		return tempDict;
	}


	public Dictionary<int, string> PopulatePuzzleTypeDictionary(){
		Dictionary<int, string> tempDict = new Dictionary<int, string>();

		//		tempDict.Add (0, "TAP");
		//		tempDict.Add (1, "COLOR");
		//		tempDict.Add (2, "MIRROR");
		//		tempDict.Add (3, "DRAG");
		//		tempDict.Add (4, "DRAG 2");
		//		tempDict.Add (5, "DRAG 3");
		//		tempDict.Add (6, "UPSIDE DOWN");

		tempDict.Add (0, "DRAG");
		tempDict.Add (1, "DRAG 2");
		tempDict.Add (2, "DRAG 3");

		return tempDict;
	}


	public Dictionary<string, string> PopulateHintDictionary(){
		Dictionary<string, string> tempDict = new Dictionary<string, string>();
		tempDict.Add
		("HINT_BASIC",
			"Drag the tiles to highlighted areas to complete your score"
		);
		tempDict.Add("HINT_ICON",
			"Click on '?' icon to highlight areas for current score"
		);

		return tempDict;
	}
}
