using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Right,
    Left
}

public class UIFishGenerator : MonoBehaviour
{
    public GameObject _fishPrefab;
    public float TimeToSpawn;
    private float nextTimeToSpawn;

	// Use this for initialization
	void Start () {
        nextTimeToSpawn = Time.time + TimeToSpawn;
    }
	
	// Update is called once per frame
	void Update () {
		if(Time.time > nextTimeToSpawn)
        {
            nextTimeToSpawn = Time.time + TimeToSpawn;

            Direction sortDir = (Random.Range(0f, 1f) > 0.5f) ? Direction.Left : Direction.Right;

            GameObject go = Instantiate(_fishPrefab);
            go.name = "Home Fish";
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localPosition = new Vector3((sortDir == Direction.Right) ? -580 : 580, Random.Range(-320, 320), 0);
            go.transform.localScale = Vector3.one * Random.Range(0.5f, 2f);

            go.GetComponent<Fish>().setProperty(sortDir);
        }
	}
}
