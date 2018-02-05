using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeController : MonoBehaviour
{
    
    public void OnClickOpts()
    {
        UIController.Instance._options.gameObject.SetActive(true);
    }
}
