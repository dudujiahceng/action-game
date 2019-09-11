using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DemoSceneManager : MonoBehaviour {
    public static DemoSceneManager instance;
    public GameObject player;
    public GameObject Monster;
    void Awake()
    {
        instance = this;
    }
	void Start () {
		
	}
}
