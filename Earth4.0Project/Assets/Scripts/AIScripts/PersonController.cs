using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour {

    private Vector3 _target;
    private int _fatigue, _fatigueRate;
    private float _speed, _hitRadius;
    private float _worldRadius;

    private Vector3 _houseLocation, _farmLocation, _workLocation;

    public GameObject _attachedHouse;

    private GameObject _world;
    private GameObject _targetObject;
    private Buildings _targetBuilding;

    private enum Buildings { None, Factory, Farm, House, Tree, Cotton };

    // Use this for initialization
    void Start()
    {
        _world = GameObject.Find("World");
        _worldRadius = _world.transform.localScale.x * _world.GetComponent<SphereCollider>().radius;

        _fatigue = 0;
        _fatigueRate = 10;
        _speed = 15.0f;
        _hitRadius = 0.05f;

        _targetObject = null;

        StartCoroutine("UpdatePerson");
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetObject == null)
            setTargetObject();
        _target = getTargetLocation();
        moveTowardsLocation(_target);
    }

    private void moveTowardsLocation(Vector3 target)
    {
        Vector3 current = transform.position;
        Vector3 direction = target - current;

        if (direction.magnitude > _hitRadius)
        {  // Stop if close enough to target
            Vector3 tangentDirection = Vector3.Normalize(  // Get vector in direction of `direction` but tangent to the globe at the object
                Vector3.Dot(transform.forward, direction) * transform.forward +  // Forward component
                Vector3.Dot(transform.right, direction) * transform.right        // Right component
            );
            float angle = Vector3.Angle(transform.forward, tangentDirection);
            transform.Rotate(0, angle, 0, Space.Self);  // Look in tangentDirection
            transform.RotateAround(_world.transform.position, transform.right, _speed * Time.deltaTime);  // Move to that direction around globe
        }
    }

    private void setTargetObject()
    {
        Vector3 targetLocation = transform.position;  // don't move if no target
        GameObject targetObject = null;
        Buildings targetBuilding = Buildings.None;

        if (_fatigue >= 100)
        {
            // Return to house
            targetObject = _attachedHouse;
            if (targetObject != null) {
                targetBuilding = Buildings.House;
            }
        }
        else if (_fatigue <= 0)
        {
            List<Buildings> availableBuildings = new List<Buildings>();
            int farmCount = _world.GetComponent<GameManager>()._farmList.Count;
            int factoryCount = _world.GetComponent<GameManager>()._factoryList.Count;
            int cottonCount = _world.GetComponent<GameManager>()._cottonList.Count;

            if (farmCount > 0)
                availableBuildings.Add(Buildings.Farm);
            if (factoryCount > 0)
                availableBuildings.Add(Buildings.Factory);
            if (cottonCount > 0)
                availableBuildings.Add(Buildings.Cotton);

            if (availableBuildings.Count > 0) {
                targetBuilding = availableBuildings[Random.Range(0, availableBuildings.Count)];
                if (targetBuilding == Buildings.Farm) {
                    targetObject = _world.GetComponent<GameManager>()._farmList[Random.Range(0, farmCount)];
                } else if (targetBuilding == Buildings.Factory) {
                    targetObject = _world.GetComponent<GameManager>()._factoryList[Random.Range(0, factoryCount)];
                } else if (targetBuilding == Buildings.Cotton) {
                    targetObject = _world.GetComponent<GameManager>()._cottonList[Random.Range(0, cottonCount)];
                }
            }
        }

        _targetObject = targetObject;

        leaveBuilding(_targetBuilding);
        _targetBuilding = targetBuilding;
        enterBuilding(_targetBuilding);
    }

    private void enterBuilding(Buildings building)
    {
        if (building == Buildings.Farm) {
            _world.GetComponent<GameManager>()._currentFarmWorkers += 1;
        } else if (building == Buildings.Factory) {
            _world.GetComponent<GameManager>()._currentFactoryWorkers += 1;
        } else if (building == Buildings.Cotton) {
            _world.GetComponent<GameManager>()._currentCottonWorkers += 1;
        }
    }

    private void leaveBuilding(Buildings building)
    {
        if (building == Buildings.Farm) {
            _world.GetComponent<GameManager>()._currentFarmWorkers -= 1;
        } else if (building == Buildings.Factory) {
            _world.GetComponent<GameManager>()._currentFactoryWorkers -= 1;
        } else if (building == Buildings.Cotton) {
            _world.GetComponent<GameManager>()._currentCottonWorkers -= 1;
        }
    }

    private Vector3 getTargetLocation()
    {
        if (_targetObject == null)
            return transform.position;  // don't move if no target
        return _targetObject.transform.position;
    }

    private Vector3 randomVector()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return normalizeVectorToSphere(randomDirection);
    }

    private Vector3 normalizeVectorToSphere(Vector3 vector)
    {
        return (
            _world.transform.position +
            _worldRadius *
            Vector3.Normalize(vector)
        );
    }

    private bool atHome()
    {
        if (_attachedHouse == null)
            return false;
        return Vector3.Distance(transform.position, _attachedHouse.transform.position) <= _hitRadius;
    }

    private bool atTarget()
    {
        if (_targetObject == null)
            return false;
        return Vector3.Distance(transform.position, _targetObject.transform.position) <= _hitRadius;
    }

    IEnumerator UpdatePerson()
    {
        for (;;) {
            if (atHome()) {
                if (_fatigue <= _fatigueRate) { // JUST recovered, need new _targetObject
                    _fatigue = 0;
                    _targetObject = null;  // check if causes race condition problems
                } else {
                    _fatigue -= _fatigueRate;
                }
            } else if (atTarget()) {  // generate fatigue if at a target that is not home
                if (_fatigue >= 100 - _fatigueRate) {
                    _fatigue = 100;
                    _targetObject = null;
                } else {
                    _fatigue += _fatigueRate;
                }
            }
            yield return new WaitForSeconds(.2f);
        }
    }
}
