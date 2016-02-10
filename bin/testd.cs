using UnityEngine;
using System.Collections;
using DLLTest;
    

public class testd : MonoBehaviour {

	// Use this for initialization

     void Start () {
        MyUtilities utils = new MyUtilities();
        utils.AddValues(2, 3);
        Debug.Log("2 + 3 = " + utils.c);
     }
    
     void Update () {


         Debug.Log(MyUtilities.GenerateRandom(0, 100));
     }

}
