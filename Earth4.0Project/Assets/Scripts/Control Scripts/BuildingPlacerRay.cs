using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerRay : MonoBehaviour {

    public float _length_multiplier = 10;
    public float _lineWidth = 0.3f;

    private LineRenderer _lineRenderer;

    private GameObject _world;
    private int _layerMask;

    private RaycastHit _hit;

    private enum Buildings { None, Factory, Farm, House, Tree };
    private Buildings _equippedBuilding = Buildings.None;

    void Start () {

        _world = GameObject.Find("World");

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.widthMultiplier = _lineWidth;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, new Vector3(0, 0, 0));

        //Ensure any gameObjects in layer 2 are ignored
        _layerMask = 1 << 2;
        _layerMask = ~_layerMask;
    }

    void Update () {

        rayCollisionHandler();

    }

    void rayCollisionHandler(){

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _hit, Mathf.Infinity, _layerMask))
        {
            var collidedObject = _hit.collider.gameObject;

            if(collidedObject.tag == "Button" && _equippedBuilding == 0)
            {
                //Make controller vibrate when hovering over selectable buttons
                OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);

                if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == true)
                {
                    if (collidedObject.name == "FactoryButton")
                    {
                        _equippedBuilding = Buildings.Factory;
                    }
                    if(collidedObject.name == "FarmButton")
                    {
                        _equippedBuilding = Buildings.Farm;
                    }
                    if(collidedObject.name == "HouseButton")
                    {
                        _equippedBuilding = Buildings.House;
                    }
                    if(collidedObject.name == "TreeButton")
                    {
                        _equippedBuilding = Buildings.Tree;
                    }
                }
            }

            //If the trigger is released while the cursor is over the world instantiate a building
            if (collidedObject.name == "World")
            {

                if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == false)
                {
                    //Instantiated equipped building on the face of the world
                    instantiateOnWorld();

                _equippedBuilding = Buildings.None;
                }

            }


            //Make line visible to user and turn green
            _lineRenderer.SetPosition(1, Vector3.forward * _hit.distance);
            _lineRenderer.material.color = Color.green;

        }
        else
        {
            //If playerer released the trigger and isn't hovering over the world, cancel item placement
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == false)
            {
                _equippedBuilding = Buildings.None;
            }

            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            //Make line visible to user and turn red
            _lineRenderer.SetPosition(1, Vector3.forward*_length_multiplier);
            _lineRenderer.material.color = Color.red;
        }

    }


    void instantiateOnWorld(){
        Vector3 normalOffSphere = _hit.normal;
        Quaternion rotation = Quaternion.LookRotation(_hit.normal);

        //Instatiate the correct building if there is enough money and the GameManager returns true
        if(_equippedBuilding == Buildings.None)
        {
            return;
        }
        else if (_equippedBuilding == Buildings.Factory)
        {
            _world.GetComponent<GameManager>().addFactory(_hit.point, rotation);
        }
        else if(_equippedBuilding == Buildings.Farm)
        {
            _world.GetComponent<GameManager>().addFarm(_hit.point, rotation);
        }
        else if(_equippedBuilding == Buildings.House)
        {
            _world.GetComponent<GameManager>().addHouse(_hit.point, rotation);
        }
        else if(_equippedBuilding == Buildings.Tree)
        {
            _world.GetComponent<GameManager>().addTree(_hit.point, rotation);
        }
        else
        {
            Debug.Log("ERROR: [BuildingPlacerRay.cs - instantiateOnWorld] invalid building type");
        }
    }

}
