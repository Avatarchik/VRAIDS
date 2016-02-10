using UnityEngine;
using System.Collections;

public class Item_window : MonoBehaviour {
        
        public Rect windowSize= new Rect(15,15,250, 250);
        private void OnGUI()
        {
            GUI.Window(0, windowSize, MyWindow,"My window");
        }
        private void MyWindow(int id)
        {
            GUI.Button(new Rect(15, 15, 50, 50), "Button");
        }
}
