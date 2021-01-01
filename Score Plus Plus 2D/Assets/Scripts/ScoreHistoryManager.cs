using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHistoryManager : MonoBehaviour {

	//static List<ScoreDetails> ScoreDetailsList;

	static Dictionary<int, ScoreDetails> ScoreDetailsDict;

	void Awake () {

		ScoreDetailsDict = new Dictionary<int, ScoreDetails>();

		print (Application.persistentDataPath);

		if (File.Exists (Application.persistentDataPath + "/scoreDetailsList.data")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/scoreDetailsList.data", FileMode.Open);
			ScoreDetailsDict = (Dictionary<int, ScoreDetails>)bf.Deserialize (file); 
			file.Close();
		}

	}

	void Update () {

	}

	public static void SavePlatformData(ScoreDetails _platformData){
		print ("Save Platform Data: " + _platformData.score );

		if (ScoreDetailsDict.ContainsKey (_platformData.score)) {
			if (_platformData.scoreStar > ScoreDetailsDict [_platformData.score].scoreStar) {
				//Update score details 
				print ("Update score details for: " + _platformData.score);

				ScoreDetailsDict.Remove (_platformData.score);
			} else {
				print ("New time is more than best time, not updating");

				return;
			}

		}

		ScoreDetails newPlatform = new ScoreDetails ();
		newPlatform.score = _platformData.score;
		newPlatform.scoreStar = _platformData.scoreStar;
		newPlatform.scoreTime = _platformData.scoreTime;
		newPlatform.scoreString = _platformData.scoreString;
		newPlatform.puzzleNum = _platformData.puzzleNum;

		ScoreDetailsDict.Add (_platformData.score, newPlatform);

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/scoreDetailsList.data");
		bf.Serialize (file, ScoreDetailsDict);
		file.Close ();
	}


	public static Dictionary<int, ScoreDetails> ReadPlatformData(){
		foreach( KeyValuePair<int, ScoreDetails> _platform in ScoreDetailsDict){
			print ("Platform: " + _platform.Key);
		}

		return ScoreDetailsDict;
	}

	public void ClearPlatformList(){
		if (File.Exists (Application.persistentDataPath + "/scoreDetailsList.data")) {
			File.Delete (Application.persistentDataPath + "/scoreDetailsList.data");
		}
	}

	public Dictionary<int, ScoreDetails> GetScoreDictionary(){
		return ScoreDetailsDict;
	}
}

