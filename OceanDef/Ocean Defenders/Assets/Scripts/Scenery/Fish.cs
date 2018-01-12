using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Direction myDir;
    public float mySpdMax;

	// Use this for initialization
	public void setProperty (Direction dir)
    {
        myDir = dir;
        mySpdMax = Random.Range(0.5f, mySpdMax);

        transform.localScale = new Vector3((myDir == Direction.Right) ? transform.localScale.x : -transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
	
	// Update is called once per frame
	void Update () {
		if(myDir == Direction.Right)
        {
            transform.position += Vector3.right * mySpdMax;
        }
        else
        {
            transform.position += Vector3.left * mySpdMax;
        }



        if (transform.localPosition.x > 600 || transform.localPosition.x < -600)
            Destroy(gameObject);
    }
}
