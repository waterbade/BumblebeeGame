using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerZone : MonoBehaviour {

	private int minPrefabRange;  
	private int maxPrefabRange; 
	// 0-1 : blau	2-3: pink	4-5: orange	  6-7: lila	  8: Mohn
	private GameObject[] flowers;
	private int numberOfFlowers;
	private int poolSize = 10;

	public float timeSinceLastSpawned;
	public float spawnRate;
	private int currentFlower;

	private float spawnXPosition = 11f;
	private float yMin;
	private float yMax;

	public void Init(int m_poolSize, Vector2 zoneRange, Vector2 prefabRange, float m_spawnRate){
		poolSize = m_poolSize;
		yMin = zoneRange.x;
		yMax = zoneRange.y;

		minPrefabRange = (int) prefabRange.x;
		maxPrefabRange = (int) prefabRange.y;

		spawnRate = m_spawnRate;
		currentFlower = 0;
		timeSinceLastSpawned = 0f;
	}

	// Use this for initialization
	public void SetUpFlowers (GameObject[] flowerPrefabs, Vector2 objectPoolPosition) {
		flowers = new GameObject[poolSize];
		for (int i = 0; i < poolSize; i++) {
			int useFlower = Random.Range (minPrefabRange,maxPrefabRange);
			GameObject flowerPrefab = flowerPrefabs[useFlower];
			flowers[i] = (GameObject)Instantiate(flowerPrefab, objectPoolPosition, Quaternion.identity);
		}
	}


	// Update is called once per frame
	public void RepositionFlowers () {

			float flowerSize = flowers [currentFlower].GetComponent<FlowerScript> ().GetColliderExtentY ();
			float spawnYPosition = Random.Range(yMin + flowerSize, yMax - flowerSize);
			//float spawnYPosition = Random.Range(yMin, yMax);
			flowers[currentFlower].transform.position = new Vector2(spawnXPosition, spawnYPosition);

			timeSinceLastSpawned = 0f;

			currentFlower++;
			if (currentFlower >= poolSize)
				currentFlower = 0;
			}

	public Vector2 GetZoneRange(){
		return (new Vector2 (yMin, yMax));
	} 
}
