using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmScript : _WorkableBuildingScript {

    private const int _MAX_FARM_WORKERS = 2;
    private Vector3 _ICON_VELOCITY = new Vector3(0, .15f, 0);
    private const float _OFFSET = 1.05f;
    private float _FADE_SPEED = 0.3f;

    public GameObject _foodIcon0;
    private GameObject _world;
    private Color color0;
    private Color color1;
    private Transform playerT;

    // Use this for initialization
    new void Start () {
		base.Start();
        GameObject player = GameObject.Find("OVRPlayerController");
        playerT = player.transform;
        _foodIcon0 = Instantiate(_foodIcon0, transform.position, Quaternion.identity, transform);
        _world = GameObject.Find("World");
        color0 = _foodIcon0.GetComponent<MeshRenderer>().materials[0].color;
        color1 = _foodIcon0.GetComponent<MeshRenderer>().materials[1].color;
    }

	// Update is called once per frame
	void Update () {
        _foodIcon0.transform.LookAt(playerT);
        _foodIcon0.transform.position += _ICON_VELOCITY * Time.deltaTime;
        color0.a -= Time.deltaTime * _FADE_SPEED;
        color1.a = color0.a;

        if (color0.a <= 0.0f)
            _foodIcon0.GetComponent<MeshRenderer>().enabled = false;
        else
        {
            _foodIcon0.GetComponent<MeshRenderer>().enabled = true;
            _foodIcon0.GetComponent<MeshRenderer>().materials[0].color = color0;
            _foodIcon0.GetComponent<MeshRenderer>().materials[1].color = color1;
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
        _foodIcon0.transform.position = iconLocation;

        color0.a = 1.0f;
        color1.a = 1.0f;

        _foodIcon0.GetComponent<MeshRenderer>().enabled = true;
        _foodIcon0.GetComponent<MeshRenderer>().materials[0].color = color0;
        _foodIcon0.GetComponent<MeshRenderer>().materials[1].color = color1;
    }

    public override int getMaxWorkers() { return _MAX_FARM_WORKERS; }
}
