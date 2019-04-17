using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanelUIContoller : MonoBehaviour {

    private GameObject handPanelUI;
	// Use this for initialization
	void Start () {
        handPanelUI = GameObject.Find("PanelUI");
        handPanelUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.eulerAngles.z > 50 && transform.eulerAngles.z < 140)
        {
            handPanelUI.SetActive(true);
        }
        else
        {
            handPanelUI.SetActive(false);
        }
    }
}
