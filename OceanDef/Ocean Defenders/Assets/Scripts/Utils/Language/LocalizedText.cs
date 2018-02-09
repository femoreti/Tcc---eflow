using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LocalizedText : MonoBehaviour
{
    private string key = "";
    public bool forceUpper = false;
    public string forceLanguage = "";

    private string text
    {
        get
        {
            Text t = GetComponent<Text>();
            if (t != null)
            {
                return t.text;
            }
            TextMesh g = GetComponent<TextMesh>();
            if (g != null)
            {
                return g.text;
            }
            return "";
        }
        set
        {
            Text t = GetComponent<Text>();
            if (t != null)
            {
                t.text = value;
            }
            TextMesh g = GetComponent<TextMesh>();
            if (g != null)
            {
                g.text = value;
            }
        }
    }

    private void Awake()
    {
        key = text;
        LanguageManager.Init();
        LanguageManager.RegisterLangChangeEvent(SetText);
    }

    private void OnDestroy()
    {
        LanguageManager.UnregisterLangChangeEvent(SetText);
    }

    public void SetText()
    {
        string temp;
        if (string.IsNullOrEmpty(forceLanguage))
            temp = LanguageManager.GetWord(key);
        else
            temp = LanguageManager.GetWord(forceLanguage.ToLower(), key);

        if (forceUpper) temp = temp.ToUpper();

        text = temp;
    }

    public void SetKey(string newKey)
    {
        key = newKey;
        if (LanguageManager.ready)
            SetText();
    }
}
