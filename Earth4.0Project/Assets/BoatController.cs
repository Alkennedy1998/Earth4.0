using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour {

    private GameObject _ocean;
    private float _oceanRadius;


	// Use this for initialization
	void Start () {
        _ocean = GameObject.Find("Ocean");
        Renderer rend = _ocean.GetComponent<Renderer>();
        _oceanRadius = rend.bounds.extents.magnitude;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private Vector3 randomVector()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return normalizeVectorToSphere(randomDirection);
    }

    private Vector3 normalizeVectorToSphere(Vector3 vector)
    {
        return (
            _ocean.transform.position +
            _oceanRadius *
            Vector3.Normalize(vector)
        );
    }
}
