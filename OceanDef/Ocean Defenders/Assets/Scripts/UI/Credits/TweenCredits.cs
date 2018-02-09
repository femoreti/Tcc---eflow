using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenCredits : MonoBehaviour
{
    RectTransform rt;
    public float tweenTime = 5f;
    public NightTween.EaseType easing;


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    // Use this for initialization
    IEnumerator Start ()
    {
        yield return new WaitForEndOfFrame();
        OnAnimate();

    }
	
    void OnAnimate()
    {
        if(rt.anchoredPosition.y < rt.rect.height)
        {
            rt.anchoredPosition = new Vector2(0, -778f);
        }

        NightTween.Create(gameObject, tweenTime, new NightTweenParams()
            .Property(NTPropType.rectTransformAnchoredPosition, new Vector2(0, rt.rect.height - 10f))
            .OnFinish(OnAnimate)
            );
    }
}
