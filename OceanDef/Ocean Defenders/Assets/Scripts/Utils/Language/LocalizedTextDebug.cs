using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LocalizedTextDebug : LocalizedText
{
    private float moveSpeed = 100;
    public float spacingMult = 0;

    private static LocalizedTextDebug instance = null;

    public LocalizedTextDebug()
    {
        instance = this;
    }

    private void Update()
    {
        if (instance == this)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                LanguageManager.SetLanguage(LanguageManager.currentLanguageIndex + 1);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                LanguageManager.SetLanguage(LanguageManager.currentLanguageIndex - 1);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.up * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.down * Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.left * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.right * Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.U))
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.left * Time.deltaTime * moveSpeed * spacingMult;
        }
        if (Input.GetKey(KeyCode.I))
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.right * Time.deltaTime * moveSpeed * spacingMult;
        }
    }
}
