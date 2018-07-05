using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerCreator : MonoBehaviour {

	public GameObject[] flowerPrefabs; 
	// 0-1 : blau	2-3: pink	4-5: orange	  6-7: lila	  8: Mohn

	public int numberOfFlowerZones = 1;
	public int numberOfNoneFlowerZones = 3;
	public GameObject floweryStuff; //needed to attach flower scripts to

	private FlowerZone[] flowerZones;
	private Vector2 objectPoolPosition = new Vector2 (0,0);  
	private float cameraSize;
	private float gameDuration;


	void Start () {
		cameraSize = Camera.main.orthographicSize;
		gameDuration = GetComponent<EventManager>().GetGameSessionDuration ();
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
		FlowerZone zone1 = new FlowerZone (10, CalculateZoneRange(1), new Vector2(8,9), CalculateSpawnRate(25));
		//FlowerZone zone1 = floweryStuff.AddComponent<FlowerZone>(); 
		//zone1.SetUpFlowerZone(10, CalculateZoneRange(1), new Vector2(8,9), CalculateSpawnRate(25));
		flowerZones [0] = zone1;
		FlowerZone zone2 = new FlowerZone (10, CalculateZoneRange(2), new Vector2(4,8), CalculateSpawnRate(50));
		flowerZones [1] = zone2;
		FlowerZone zone3 = new FlowerZone (10, CalculateZoneRange(3), new Vector2(0,4), CalculateSpawnRate(100));
		flowerZones [2] = zone3;

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
}
