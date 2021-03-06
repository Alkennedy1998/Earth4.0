﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour {

    public const float _BIRTH_RATE = 0.0f;

    // private Vector3 _target;
    private int _fatigue, _fatigueRate;
    private float _speed, _hitRadius;
    private float _worldRadius;

    private bool _hasTarget;

    private Vector3 _houseLocation, _farmLocation, _workLocation;

    public GameObject _attachedHouse;

    private GameObject _world;
    private GameObject _targetObject;
    private Buildings _targetBuilding;

    private Animator _animator;

    private enum Buildings { None, Factory, Farm, House, Tree, Cotton };

    // Use this for initialization
    void Start()
    {
        _world = GameObject.Find("World");
        _worldRadius = _world.transform.localScale.x * _world.GetComponent<SphereCollider>().radius;

        _animator = GetComponent<Animator>();

        _fatigue = 0;
        _fatigueRate = 10;
        _speed = 15.0f;
        _hitRadius = 0.15f;

        _hasTarget = false;
        _targetObject = null;
        _targetBuilding = Buildings.None;

        StartCoroutine("UpdatePerson");
        StartCoroutine("UpdatePopulation");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasTarget) {
            setTargetObject();
            if (_hasTarget) {
                _animator.SetFloat("Speed", 1.0f);
                _animator.SetBool("IsPicking", false);
            }
        }
        // _target = getTargetLocation();
        // moveTowardsLocation(_target);
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
                _hasTarget = true;
            }
        }
        else if (_fatigue <= 0)
        {
            List<Buildings> buildingTypes = new List<Buildings> {Buildings.Farm, Buildings.Factory, Buildings.Cotton};
            List<float> buildingProbabilities = new List<float> {0.0f, 0.0f, 0.0f};

            GameObject closestFarm = getClosestAvailableBuilding(_world.GetComponent<GameManager>()._farmList);
            GameObject closestFactory = getClosestAvailableBuilding(_world.GetComponent<GameManager>()._factoryList);
            GameObject closestCotton = getClosestAvailableBuilding(_world.GetComponent<GameManager>()._cottonList);

            buildingProbabilities[0] = getBuildingWeight(closestFarm, Buildings.Farm);
            buildingProbabilities[1] = getBuildingWeight(closestFactory, Buildings.Factory);
            buildingProbabilities[2] = getBuildingWeight(closestCotton, Buildings.Cotton);
            float totalProbability = sumFloats(buildingProbabilities);

            if (totalProbability > 0.0f) {
                for (int i = 0; i < buildingProbabilities.Count; i++)
                    buildingProbabilities[i] /= totalProbability;

                targetBuilding = weightedBuildingChoice(buildingTypes, buildingProbabilities);
                if (targetBuilding == Buildings.Farm) {
                    targetObject = closestFarm;
                } else if (targetBuilding == Buildings.Factory) {
                    targetObject = closestFactory;
                } else if (targetBuilding == Buildings.Cotton) {
                    targetObject = closestCotton;
                }
                _hasTarget = true;
            }
        }

        leaveBuilding(_targetObject, _targetBuilding);
        GetComponent<Pathfinding.AIDestinationSetter>().target = targetObject == null ? null : targetObject.transform;
        _targetObject = targetObject;
        _targetBuilding = targetBuilding;
        enterBuilding(_targetObject, _targetBuilding);
    }

    private void enterBuilding(GameObject obj, Buildings building)
    {
        if (obj == null || building == Buildings.None)
            return;

        if (building == Buildings.Farm) {
            _targetObject.GetComponent<FarmScript>().addWorker();
            _world.GetComponent<GameManager>()._currentFarmWorkers += 1;
        } else if (building == Buildings.Factory) {
            _targetObject.GetComponent<FactoryScript>().addWorker();
            _world.GetComponent<GameManager>()._currentFactoryWorkers += 1;
        } else if (building == Buildings.Cotton) {
            _targetObject.GetComponent<CottonScript>().addWorker();
            _world.GetComponent<GameManager>()._currentCottonWorkers += 1;
        }
    }

    private void leaveBuilding(GameObject obj, Buildings building)
    {
        if (obj == null || building == Buildings.None)
            return;

        if (building == Buildings.Farm) {
            _targetObject.GetComponent<FarmScript>().removeWorker();
            _world.GetComponent<GameManager>()._currentFarmWorkers -= 1;
        } else if (building == Buildings.Factory) {
            _targetObject.GetComponent<FactoryScript>().removeWorker();
            _world.GetComponent<GameManager>()._currentFactoryWorkers -= 1;
        } else if (building == Buildings.Cotton) {
            _targetObject.GetComponent<CottonScript>().removeWorker();
            _world.GetComponent<GameManager>()._currentCottonWorkers -= 1;
        }
    }

    private GameObject getClosestAvailableBuilding(List<GameObject> buildingList)
    {
        float shortestDistance = float.MaxValue;
        GameObject closestAvailableBuilding = null;

        foreach (GameObject building in buildingList) {
            if (!building.GetComponent<_WorkableBuildingScript>().isFull()) {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    closestAvailableBuilding = building;
                }
            }
        }

        return closestAvailableBuilding;
    }

    private float getBuildingWeight(GameObject building, Buildings type) {
        // New algorithm:
        //
        // Goal: AI should intelligently select targets based on
        // environmental factors, proximity, resources, availability.
        //
        // Produce one weight for each building type, and decide which
        // building type has the highest weight. Go to nearest available
        // building of that type.
        //
        // Each building weight should be affected by:
        //     1. Proximity (and number?) of closest available buildings
        //     2. Surplus/demand of relevant resource for that building type
        //     3. Number of people working at that building type (rate of change)
        //

        if (building == null)
            return 0.0f;

        float resource = 0.0f;
        if (type == Buildings.Farm) {
            float numPeople = _world.GetComponent<GameManager>()._personList.Count;
            resource = _world.GetComponent<GameManager>()._currentFood;
            return clamp10(resource / (25.0f * numPeople));
        } else if (type == Buildings.Factory) {
            resource = _world.GetComponent<GameManager>()._currentMoney;
            return clamp10(resource / 10000.0f);
        } else if (type == Buildings.Cotton) {
            resource = _world.GetComponent<GameManager>()._currentCotton;
            return clamp10(resource / 500.0f);
        }

        // Map resource count from 0-huge to be from 1-0
        // TODO: change to sigmoid/exp
        // TODO: take into account deltaResource
        return clamp10(resource / 1000.0f);
    }

    private float clamp10(float val) {
        if (val < 0.0f)
            val = 0.1f;
        if (val > 1.0f)
            val = 1.0f;
        return -1.0f * val + 1.1f;
    }

    private float sumFloats(List<float> nums) {
        float sum = 0.0f;
        foreach (float num in nums)
            sum += num;
        return sum;
    }

    private Vector3 getTargetLocation()
    {
        if (_targetObject == null)
            return transform.position;  // don't move if no target
        return _targetObject.transform.position;
    }

    private Buildings weightedBuildingChoice(List<Buildings> choices, List<float> probabilities)
    {
        // choices and probabilities must have the same length and sum(probabilities) == 1.0f
        float random = Random.Range(0.0f, 1.0f);
        float cumulative = 0.0f;

        for (int i = 0; i < choices.Count; i++) {
            cumulative += probabilities[i];
            if (random < cumulative)
                return choices[i];
        }
        return choices[0];
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

    private bool nearTarget()
    {
        if (_targetObject == null)
            return false;
        return Vector3.Distance(transform.position, _targetObject.transform.position) <= 10 * _hitRadius;
    }

    IEnumerator UpdatePerson()
    {
        while (true) {
            if (nearTarget()) {
                _animator.SetFloat("Speed", 0.0f);
                _animator.SetBool("IsPicking", true);
            }

            if (atHome()) {
                if (_fatigue <= _fatigueRate) { // JUST recovered, need new _targetObject
                    _fatigue = 0;
                    _hasTarget = false;  // check if causes race condition problems
                } else {
                    _fatigue -= _fatigueRate;
                }
            } else if (atTarget()) {  // generate fatigue if at a target that is not home
                if (_fatigue >= 100 - _fatigueRate) {
                    _fatigue = 100;
                    _hasTarget = false;
                } else {
                    _fatigue += _fatigueRate;
                }
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator UpdatePopulation()
    {
        while (true) {
            if (atHome() && Random.Range(0.0f, 1.0f) < _BIRTH_RATE) {  // handle birth
                Quaternion rotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
                _world.GetComponent<GameManager>().addPerson(transform.position, rotation, _attachedHouse);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}