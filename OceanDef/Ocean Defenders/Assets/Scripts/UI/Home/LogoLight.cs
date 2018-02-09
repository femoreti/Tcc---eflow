using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoLight : MonoBehaviour
{
    public float TweenTime;

    [Range(0, 1)]
    public float MinValue;

    private float startTime;
    private bool isFading = true;
    private Image myImg;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
        myImg = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isFading)
        {
            if (Time.time > (startTime + TweenTime))
            {
                startTime = Time.time;
                isFading = false;
                return;
            }
            
            myImg.color = new Color(myImg.color.r, myImg.color.g, myImg.color.b, Mathf.Abs(((startTime + TweenTime) - Time.time) / TweenTime));
        }
        else
        {
            if (Time.time > (startTime + TweenTime))
            {
                startTime = Time.time;
                isFading = true;
                return;
            }
            
            myImg.color = new Color(myImg.color.r, myImg.color.g, myImg.color.b, (Mathf.Abs((1 + MinValue) - ((startTime + TweenTime) - Time.time) / TweenTime)));
        }
	}
}
