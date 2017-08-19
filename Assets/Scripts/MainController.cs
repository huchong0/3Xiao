using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {
    public static MainController instance;
    private void Awake()
    {
        instance = this;
    }



    

}
