using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance
    {
        get
        {
            return instance;
        }
    }

    public HomeController _home;
    public OptsController _options;
    public CreditsController _credits;

    private GameObject _currentActive;

	// Use this for initialization
	void Awake () {
        instance = this;

        OnChangeScreen(_home.gameObject);
	}
	
	public void OnChangeScreen(GameObject newScreen)
    {
        if(_currentActive != null)
            _currentActive.SetActive(false);

        newScreen.SetActive(true);

        _currentActive = newScreen;
    }
}
