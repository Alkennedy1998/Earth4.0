﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour {

    private Vector3 _target;
    private int _fatigue;
    private float _speed, _hitRadius;
    private float _worldRadius;

    private Vector3 _houseLocation, _farmLocation, _workLocation;

    private GameObject _world;
    public GameObject _housePrefab, _farmPrefab, _factoryPrefab;

    private enum Buildings { None, Factory, Farm, House, Tree };

    // Use this for initialization
    void Start()
    {
        _world = GameObject.Find("World");
        _worldRadius = _world.transform.localScale.x * _world.GetComponent<SphereCollider>().radius;

        _houseLocation = randomVector();
        _farmLocation = randomVector();
        _workLocation = randomVector();

        _fatigue = 0;
        _speed = 15.0f;
        _hitRadius = 0.05f;
        _target = _workLocation;

        // instantiateObject(Buildings.House, _houseLocation);
        // instantiateObject(Buildings.Farm, _farmLocation);
        // instantiateObject(Buildings.Factory, _workLocation);

        StartCoroutine("UpdatePerson");
    }

    // Update is called once per frame
    void Update()
    {
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

    private Vector3 getTargetLocation()
    {
        Vector3 targetLocation;

        if (_fatigue >= 100)
        {
            // Return to house
            targetLocation = _houseLocation;
        }
        else if (_fatigue <= 0)
        {
            // Ready to work
            if (Random.Range(0, 1) <= 0.5f)
            {
                targetLocation = _farmLocation;
            }
            else
            {
                targetLocation = _workLocation;
            }
        }
        else
        {
            targetLocation = _target;
        }

        return targetLocation;
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

    private void instantiateObject(Buildings modelType, Vector3 location)
    {
        Vector3 normalOffSphere = location - _world.transform.position;
        Quaternion rotation = Quaternion.LookRotation(normalOffSphere);

        if (modelType == Buildings.House)
        {
            _world.GetComponent<GameManager>().addHouse(location, rotation);
        }
        else if (modelType == Buildings.Farm)
        {
            _world.GetComponent<GameManager>().addFarm(location, rotation);
        }
        else if (modelType == Buildings.Factory)
        {
            _world.GetComponent<GameManager>().addFactory(location, rotation);
        }
        else
        {
            Debug.Log("ERROR: Incorrect modelType in PersonMover script.");
        }
    }

    IEnumerator RandomizeTarget()
    {
        for (; ; )
        {
            _target = randomVector();
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator UpdatePerson()
    {
        for (; ; )
        {
            if (Vector3.Distance(transform.position, _houseLocation) <= _hitRadius)
            {
                _fatigue = _fatigue <= 0 ? 0 : _fatigue - 10;
            }
            else if (Vector3.Distance(transform.position, _workLocation) < _hitRadius ||
                     Vector3.Distance(transform.position, _farmLocation) < _hitRadius)
            {
                _fatigue = _fatigue >= 100 ? 100 : _fatigue + 10;
            }
            yield return new WaitForSeconds(.2f);
        }
    }
}
