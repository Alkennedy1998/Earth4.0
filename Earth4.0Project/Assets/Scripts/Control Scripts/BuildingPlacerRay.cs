﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerRay : MonoBehaviour
{
    #region gameValues
    public float _length_multiplier = 10;
    public float _lineWidth = 0.3f;

    private LineRenderer _lineRenderer;

    private GameObject _outlineBuilding = null;
    public int _buildingCollisions = 0;

    private GameObject _world;
    private int _layerMask;

    private RaycastHit _hit;

    private enum Buildings { None, Factory, Farm, House, Tree, Cotton };
    private Buildings _equippedBuilding = Buildings.None;

    private bool triggerHasBeenReleased = true;

    #endregion

    #region unityFunctions
    void Start()
    {

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

    void Update()
    {

        rayCollisionHandler();

    }
    #endregion

    void rayCollisionHandler()
    {

        //If ray hits an object
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _hit, Mathf.Infinity, _layerMask))
        {
            var collidedObject = _hit.collider.gameObject;
            Quaternion rotation = Quaternion.LookRotation(_hit.normal);

            //Make line visible to user and turn green
            _lineRenderer.SetPosition(1, Vector3.forward * _hit.distance);
            _lineRenderer.material.color = Color.green;

            #region buildingHighlighter
            if (collidedObject.name == "World")
            {
                //Instantiate a building if none exists yet
                if (_outlineBuilding == null)
                {

                    if (_equippedBuilding == Buildings.None)
                    {
                        return;
                    }
                    else if (_equippedBuilding == Buildings.Factory)
                    {
                        _outlineBuilding = _world.GetComponent<GameManager>().instantiateOnWorld(_world.GetComponent<GameManager>()._factoryPrefab, _hit.point, rotation);
                    }
                    else if (_equippedBuilding == Buildings.Farm)
                    {
                        _outlineBuilding = _world.GetComponent<GameManager>().instantiateOnWorld(_world.GetComponent<GameManager>()._farmPrefab, _hit.point, rotation);
                    }
                    else if (_equippedBuilding == Buildings.House)
                    {
                        _outlineBuilding = _world.GetComponent<GameManager>().instantiateOnWorld(_world.GetComponent<GameManager>()._housePrefab, _hit.point, rotation);
                    }
                    else if (_equippedBuilding == Buildings.Tree)
                    {
                        _outlineBuilding = _world.GetComponent<GameManager>().instantiateOnWorld(_world.GetComponent<GameManager>()._treePrefab, _hit.point, rotation);
                    }
                    else if (_equippedBuilding == Buildings.Cotton)
                    {
                        _outlineBuilding = _world.GetComponent<GameManager>().instantiateOnWorld(_world.GetComponent<GameManager>()._cottonPrefab, _hit.point, rotation);
                    }
                    else
                    {
                        Debug.Log("ERROR: Invalid building type");
                    }
                    _outlineBuilding.AddComponent<buildingPlacementValidator>();
                }
                //If the outline building has already been instantiated
                else
                {
                    if (_buildingCollisions == 0)
                    {
                        foreach (Transform child in _outlineBuilding.transform)
                        {
                            child.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                    else
                    {
                        Debug.Log(_buildingCollisions);

                        foreach (Transform child in _outlineBuilding.transform)
                        {
                            child.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        }
                        _outlineBuilding.transform.Rotate(Vector3.up, 15 * Time.deltaTime);
                    }
                    _outlineBuilding.transform.position = _hit.point;
                    _outlineBuilding.transform.rotation = rotation;
                    _outlineBuilding.transform.Rotate(90, 0, 0);

                }
            }
            #endregion

            #region onTriggerPress
            //If that object is a button set active Item to that button item
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == true && triggerHasBeenReleased)
            {
                triggerHasBeenReleased = false;

                if (collidedObject.name == "World")
                {
                    if (_buildingCollisions == 0)
                    {
                        instantiateOnWorld();
                    }
                    else
                    {
                        Debug.Log("Can't build there!");
                        //Notify player that building placement invalid
                    }
                }
                else if (collidedObject.name == "FactoryButton")
                {
                    _equippedBuilding = Buildings.Factory;
                }
                else if (collidedObject.name == "FarmButton")
                {
                    _equippedBuilding = Buildings.Farm;
                }
                else if (collidedObject.name == "HouseButton")
                {
                    _equippedBuilding = Buildings.House;
                }
                else if (collidedObject.name == "TreeButton")
                {
                    _equippedBuilding = Buildings.Tree;
                }
                else if (collidedObject.name == "CottonButton")
                {
                    _equippedBuilding = Buildings.Cotton;
                }

            }
            else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == false)
            {
                triggerHasBeenReleased = true;
            }
            #endregion

        }
        //Ray doesn't hit anything
        else
        {
            //Destroy building highlight
            Destroy(_outlineBuilding);
            _buildingCollisions = 0;
            _lineRenderer.SetPosition(1, Vector3.forward * 7);
            _lineRenderer.material.color = Color.red;
        }

    }
















    /* 
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
                        if(collidedObject.name == "CottonButton")
                        {
                            _equippedBuilding = Buildings.Cotton;
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

    */
    void instantiateOnWorld()
    {
        Vector3 normalOffSphere = _hit.normal;
        Quaternion rotation = Quaternion.LookRotation(_hit.normal);

        //Instatiate the correct building if there is enough money and the GameManager returns true
        if (_equippedBuilding == Buildings.None)
        {
            return;
        }
        else if (_equippedBuilding == Buildings.Factory)
        {
            _world.GetComponent<GameManager>().addFactory(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.Farm)
        {
            _world.GetComponent<GameManager>().addFarm(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.House)
        {
            _world.GetComponent<GameManager>().addHouse(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.Tree)
        {
            _world.GetComponent<GameManager>().addTree(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.Cotton)
        {
            Debug.Log("Cotton placed");
            _world.GetComponent<GameManager>().addCotton(_hit.point, rotation);
        }
        else
        {
            Debug.Log("ERROR: Invalid building type");
        }
    }

}
