using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerRay : MonoBehaviour
{
    #region gameValues
    public float _length_multiplier = 10;
    public float _lineWidth;

    private LineRenderer _lineRenderer;

    private GameObject _outlineBuilding = null;
    public int _buildingCollisions = 0;

    private GameObject _world;
    private int _layerMask;

    private RaycastHit _hit;

    private enum Buildings { None, Factory, Farm, House, Tree, Cotton };
    private Buildings _equippedBuilding = Buildings.None;

    private bool _triggerHasBeenReleased = true;

    public List<GameObject> _overlappedTrees = new List<GameObject>();

    public GameObject _factoryInfo, _farmInfo, _cottonInfo, _treeInfo, _houseInfo;
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

        RaycastHit sphereCast;

        Physics.SphereCast(transform.position, 10, transform.TransformDirection(Vector3.forward), out sphereCast, Mathf.Infinity, _layerMask);
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
            //_lineRenderer.material.color = Color.green;

            //If a button is hovered over then show the relevant info panel
            #region showBuildingInfo

            Debug.Log(collidedObject.tag);
            if (collidedObject.tag == "button")
            {
                Debug.Log(collidedObject.name);
                if (collidedObject.name == "FactoryButton")
                {
                    _factoryInfo.SetActive(true);
                }
                else if (collidedObject.name == "FarmButton")
                {
                    _farmInfo.SetActive(true);

                }
                else if (collidedObject.name == "HouseButton")
                {
                    _houseInfo.SetActive(true);
                }
                else if (collidedObject.name == "TreeButton")
                {
                    _treeInfo.SetActive(true);

                }
                else if (collidedObject.name == "CottonButton")
                {
                    _cottonInfo.SetActive(true);

                }
            }
            else
            {
                _factoryInfo.SetActive(false);
                _farmInfo.SetActive(false);
                _houseInfo.SetActive(false);
                _treeInfo.SetActive(false);
                _cottonInfo.SetActive(false);
            }
            #endregion
            //Display where the building is about to be placed on the world if the cursor is over the world
            #region buildingHighlighter
            if (collidedObject.name == "World")
            {
                //Instantiate a building outline if none exists yet
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
                    //If there are no building in the way of placing
                    if (_buildingCollisions == 0)
                    {
                        //Make object green
                        foreach (Transform child in _outlineBuilding.transform)
                        {
                            //child.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                    else
                    {
                        foreach (Transform child in _outlineBuilding.transform)
                        {
                            //child.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        }
                        _outlineBuilding.transform.Rotate(Vector3.up, 15 * Time.deltaTime);
                    }
                    _outlineBuilding.transform.position = _hit.point;
                    _outlineBuilding.transform.rotation = rotation;
                    _outlineBuilding.transform.Rotate(90, 0, 0);

                }
            }
            #endregion
            //If user is pressing the trigger
            #region onTriggerPress
            //If that object ray collides with is a button set active Item to that button item
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == true && _triggerHasBeenReleased)
            {
                _triggerHasBeenReleased = false;

                if (collidedObject.name == "World")
                {
                    //If no buildings in the way
                    if (_buildingCollisions == 0)
                    {
                        addObjToGameManager();

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
                _triggerHasBeenReleased = true;
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

    private void addObjToGameManager()
    {
        Vector3 normalOffSphere = _hit.normal;
        Quaternion rotation = Quaternion.LookRotation(_hit.normal);
        bool buildingPlacedSuccessfully = false;
        //Instatiate the correct building if there is enough money and the GameManager returns true
        if (_equippedBuilding == Buildings.None)
        {
            return;
        }
        else if (_equippedBuilding == Buildings.Factory)
        {
            buildingPlacedSuccessfully = _world.GetComponent<GameManager>().addFactory(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.Farm)
        {
            buildingPlacedSuccessfully = _world.GetComponent<GameManager>().addFarm(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.House)
        {
            buildingPlacedSuccessfully = _world.GetComponent<GameManager>().addHouse(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.Tree)
        {
            buildingPlacedSuccessfully = _world.GetComponent<GameManager>().addTree(_hit.point, rotation);
        }
        else if (_equippedBuilding == Buildings.Cotton)
        {
            buildingPlacedSuccessfully = _world.GetComponent<GameManager>().addCotton(_hit.point, rotation);
        }
        else
        {
            Debug.Log("ERROR: Invalid building type");
        }
        Debug.Log("Building added: " + _equippedBuilding);
        Debug.Log("Successfully placed: " + buildingPlacedSuccessfully);
        //Remove the trees if a building is successfully placed
        if (buildingPlacedSuccessfully && _equippedBuilding != Buildings.Tree)
        {
            removeCollidingTrees();
        }

    }

    private void removeCollidingTrees()
    {
        foreach (GameObject tree in _overlappedTrees)
        {
            Debug.Log(tree);
            _world.GetComponent<GameManager>().removeTree(tree);
        }
    }
}

