using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour {

	public void OnClose()
    {
        UIController.Instance.OnChangeScreen(UIController.Instance._home.gameObject);
    }
}
