using UnityEngine;
using System.Collections;
using Camerat;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System;
using FaceDetection;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//public static class Program
//{
//    /// <summary>
//    /// The main entry point for the application.
//    /// </summary>

//    public static void Main()
//    {
//        Head_t b = new Head_t();
//        b.Start();
//        while (true)
//        {
//            System.Threading.Thread.Sleep(50);
//            b.Update();
//        }
//    }
//}
//public class Head_t {
public class Head_t : MonoBehaviour {
    Programhead a;
    Coordinate d;
    int count;
    public Double x1 = 0;
    public Double x2 = 0;
    public Double x3 = 0;
    public Double x4 = 0;
    public Double x5 = 0;
    public Double y1 = 0;
    public Double y2 = 0;
    public Double y3 = 0;
    public Double y4 = 0;
    public Double y5 = 0;
    public Double z1 = 0;
    public Double z2 = 0;
    public Double z3 = 0;
    public Double z4 = 0;
    public Double z5 = 0;
	// Use this for initialization
    public void Start()
    {



        //Debug.Log("debug");
        a = new Programhead();
        a.CameraCapture();
        //Programhead.threadPriority = ThreadPriority.High;
        Application.targetFrameRate = 24;
        //Application.backgroundLoadingPriority = ThreadPriority.High;
        //d.x = 0;
        //d.y = 2.1;
        //d.z = -3.1;
        //Debug.Log("start");
    }	
	// Update is called once per frame
    //int counts = 0;
    public void Update()
    {
        //System.Threading.Thread.Sleep(20);
        //Debug.Log("debug");
        d = a.getcoor();
        x5 = x4;
        x4 = x3;
        x3 = x2;
        x2 = x1;
        
        //x1 = d.x;
        //Debug.Log(d.x);

        x1 = d.x * 0.4 + x2 * 0.25 + x3 * 0.15+x4*0.1+x5*0.1;
        y5 = y4;
        y4 = y3;
        y3 = y2;
        y2 = y1;


        y1 = d.y * 0.4 + y2 * 0.25 + y3 * 0.15 + y4 * 0.1+y5*0.1;
        z5 = z4;
        z4 = z3;
        z3 = z2;
        z2 = z1;
        

        z1 = d.z * 0.4 + z2 * 0.25 + z3 * 0.15+z4*0.1 + z5*0.1;

        Vector3 temp = new Vector3((float)(x1) * 4 * 2 + 1, (float)(y1 * 4 + 4), (float)(-z1 +0.75)*50-3);

        //Vector3 temp = new Vector3((float)(d.x)*4*2+1, (float)(d.y*4+3.5), (float)(-d.z+0.5)*13*3+4);
        transform.position = temp;
        //counts++;
        //Debug.Log(counts);
        Debug.Log("x1 = " + x1 + " y1 = " + x2 + " z1 = " + x3);
        //Debug.Log("d.x = " + d.x + " d.y = " + d.y + " d.z = " + d.z);
        //Debug.Log("temp.x = " + temp.x + " temp.y = " + temp.y + " temp.z = " + temp.z);
        //Console.WriteLine("d.x = " + d.x + " d.y = " + d.y + " d.z = " + d.z);
	}
    void OnApplicationQuit()
    { 
        a.stopca();
        //Debug.Log("aaaaaaa");
    }

}
