using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerRay : MonoBehaviour {

    public float length_multiplier = 10;
    public float lineWidth = 0.3f;

    private LineRenderer lineRenderer;

    public GameObject factoryPrefab;
    public GameObject farmPrefab;
    public GameObject housePrefab;

    private GameObject world;
    private GameObject mostRecentInstance;
    private int layerMask;

    private RaycastHit hit;

    //0 = none
    //1 = factory
    //2 = farm
    //3 = house
    private int equippedBuilding = 0;

    void Start () {

        world = GameObject.Find("World");

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 0));

        //Ensure any gameObjects in layer 2 are ignored
        layerMask = 1 << 2;
        layerMask = ~layerMask;
    }

    void Update () {


        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            var collidedObject = hit.collider.gameObject;

            if(collidedObject.tag == "Button" && equippedBuilding == 0)
            {
                //Make controller vibrate when hovering over selectable buttons
                OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);

                if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == true)
                {
                    if (collidedObject.name == "FactoryButton")
                    {
                        equippedBuilding = 1;
                    }
                    if(collidedObject.name == "FarmButton")
                    {
                        equippedBuilding = 2;
                    }
                    if(collidedObject.name == "HouseButton")
                    {
                        equippedBuilding = 3;
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

                mostRecentInstance = null;
                equippedBuilding = 0;
                }

            }
           
        
            //Make line visible to user and turn green
            lineRenderer.SetPosition(1, Vector3.forward * hit.distance);
            lineRenderer.material.color = Color.green;

        }
        else
        {
            //If playerer released the trigger and isn't hovering over the world, cancel item placement
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) == false)
            {
                equippedBuilding = 0;
            }
            
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            //Make line visible to user and turn red
            lineRenderer.SetPosition(1, Vector3.forward*length_multiplier);
            lineRenderer.material.color = Color.red;
        }


       
    }
 
                    

 
    void instantiateOnWorld(){
        GameObject newItem = null;
        Vector3 normalOffSphere = hit.normal;
        Quaternion rotation = Quaternion.LookRotation(hit.normal);

        if(equippedBuilding == 0){
            return;
        }
        if(equippedBuilding == 1){
            newItem = Instantiate(factoryPrefab, hit.point, rotation);
            world.GetComponent<GameManager>().addFactory(newItem);
        }
        if(equippedBuilding == 2)
        {
            newItem = Instantiate(farmPrefab,hit.point, rotation);
            world.GetComponent<GameManager>().addFarm(newItem);

        }
        if(equippedBuilding == 3)
        {
            newItem = Instantiate(housePrefab,hit.point, rotation);
            world.GetComponent<GameManager>().addHouse(newItem);
        }
        newItem.transform.Rotate(90, 0, 0);
        newItem.transform.parent = world.transform;
        mostRecentInstance = newItem;
    }
    
}
