using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptsController : MonoBehaviour
{
    private int _currSelected;

    public List<Sprite> languageImages;
    public Image LangImg;


    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnLangNext()
    {
        _currSelected++;

        if (_currSelected >= languageImages.Count)
            _currSelected = 0;

        LanguageManager.SetLanguage(_currSelected);
        LangImg.sprite = languageImages[_currSelected];

    }

    public void OnLangBack()
    {
        _currSelected--;

        if (_currSelected < 0)
            _currSelected = languageImages.Count - 1;

        LanguageManager.SetLanguage(_currSelected);
        LangImg.sprite = languageImages[_currSelected];
    }
}
