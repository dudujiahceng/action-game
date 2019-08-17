using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	void Start () {
		
	}
	
	public void OnClick () {
        Debug.Log("Load");
        SceneManager.LoadScene("DemoScene");
	}
}
