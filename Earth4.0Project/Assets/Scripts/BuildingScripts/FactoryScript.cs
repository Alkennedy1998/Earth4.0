using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : _WorkableBuildingScript {

    private const int _MAX_FACTORY_WORKERS = 3;

	private GameObject _world;
    private ParticleSystem _smoke;

    private Vector3 _ICON_VELOCITY = new Vector3(0, .15f, 0);
    private const float _OFFSET = 1.1f;
    private float _FADE_SPEED = 0.3f;

    public GameObject _moneyIcon;
    private Color color;
    private Transform playerT;

    // Use this for initialization
    new void Start () {
		base.Start();  // Run BaseClass' Start() method
		_world = GameObject.Find("World");
        _smoke = this.GetComponentsInChildren<ParticleSystem>()[0];

        GameObject player = GameObject.Find("OVRPlayerController");
        playerT = player.transform;
        _moneyIcon = Instantiate(_moneyIcon, transform.position, Quaternion.identity, transform);
        color = _moneyIcon.GetComponent<MeshRenderer>().material.color;      
    }

	// Update is called once per frame
	void Update () {
        var emission = _smoke.emission;
        if (_currentWorkers > 0)
            emission.enabled = true;
        else
            emission.enabled = false;

        _moneyIcon.transform.LookAt(playerT);
        _moneyIcon.transform.position += _ICON_VELOCITY * Time.deltaTime;
        color.a -= Time.deltaTime * _FADE_SPEED;

        if (color.a <= 0.0f)
            _moneyIcon.GetComponent<MeshRenderer>().enabled = false;
        else
        {
            _moneyIcon.GetComponent<MeshRenderer>().enabled = true;
            _moneyIcon.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void OnTick()
    {
        if (_currentWorkers > 0)
            showIcon();
    }

    private void showIcon()
    {
        Vector3 positionFromCenter = transform.position - _world.transform.position;
        Vector3 iconPositionFromCenter = _OFFSET * positionFromCenter;
        Vector3 iconLocation = iconPositionFromCenter + _world.transform.position;
        _moneyIcon.transform.position = iconLocation;

        color.a = 1.0f;

        _moneyIcon.GetComponent<MeshRenderer>().enabled = true;
        _moneyIcon.GetComponent<MeshRenderer>().material.color = color;
    }

    public override int getMaxWorkers() { return _MAX_FACTORY_WORKERS; }
}
