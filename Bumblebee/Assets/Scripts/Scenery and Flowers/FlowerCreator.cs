using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerCreator : MonoBehaviour {

	public GameObject[] flowerPrefabs; 
	// 0-1 : blau	2-3: pink	4-5: orange	  6-7: lila	  8: Mohn

	//!!!!! THS NUMBER IS IMPORTANT!!!!!!!!!
	private int numberOfFlowerZones = 3;
	private int numberOfNoneFlowerZones = 3;

	public GameObject floweryStuff; //needed to attach flower scripts to

	private FlowerZone[] flowerZones;
	public Vector2 objectPoolPosition = new Vector2 (-10,0);  
	private float cameraSize;
	private float gameDuration;

	public int flowerNoZone1 = 25;
	public int flowerNoZone2 = 25;
	public int flowerNoZone3 = 25;

	public float zone1Offset = 0.1f;
	public float zone2Offset = 0.1f;
	public float zone3Offset = 0.0f;

	void Start () {
		
		cameraSize = Camera.main.orthographicSize;
		gameDuration = EventManager.instance.GetGameSessionDuration ();
		flowerZones = new FlowerZone[numberOfFlowerZones];
		SetUpFlowerZones ();
	}
		
	void Update () {
		for (int i = 0; i < flowerZones.Length; i++) {
			flowerZones [i].timeSinceLastSpawned += Time.deltaTime;

			if (flowerZones[i].timeSinceLastSpawned >= flowerZones[i].spawnRate) { 
				flowerZones [i].RepositionFlowers ();
			}
		}
	}

	private void SetUpFlowerZones(){
		//Mohnblumen, in der höchsten Ebene
		FlowerZone zone1 = floweryStuff.AddComponent<FlowerZone>(); 
		zone1.Init(10, CalculateZoneRange(1), new Vector2(8,9), CalculateSpawnRate(flowerNoZone1), zone1Offset);
		flowerZones [0] = zone1;

		//orange und lila in der mittleren Ebene
		FlowerZone zone2 = floweryStuff.AddComponent<FlowerZone> ();
		zone2.Init(10, CalculateZoneRange(2), new Vector2(4,8), CalculateSpawnRate(flowerNoZone2), zone2Offset);
		flowerZones [1] = zone2;

		//blau und pink in der unteren Ebene
		FlowerZone zone3 = floweryStuff.AddComponent<FlowerZone> ();
		zone3.Init(10, CalculateZoneRange(3), new Vector2(0,4), CalculateSpawnRate(flowerNoZone3), zone3Offset);
		flowerZones [2] = zone3;

		//Debug.Log (flowerZones.Length + " flowerZones have been set Up");
		for (int i = 0; i < flowerZones.Length; i++) {
			flowerZones [i].SetUpFlowers (flowerPrefabs, objectPoolPosition);
		}
	}

	Vector2 CalculateZoneRange(int zone){

		float zoneSize = (cameraSize * 2) / (numberOfNoneFlowerZones + numberOfFlowerZones); 
		float startOfFlowerZones = cameraSize - zoneSize * numberOfNoneFlowerZones;
		float minY = startOfFlowerZones - (zoneSize * zone);
		float maxY = minY + zoneSize;

		Vector2 spawnRange = new Vector2 (minY, maxY);
		return spawnRange;
	}

	float CalculateSpawnRate(int numberOfFlowers){
		return (gameDuration / numberOfFlowers);
	}

	public int ReturnFlowerZone (float yPos){
		int zone = 0;
	
		for (int i = 0; i < flowerZones.Length; i++) {
			Vector2 range = flowerZones [i].GetZoneRange();
			if (yPos > range.x && yPos < range.y)
				zone = i;
		}
		return zone;
	}
}