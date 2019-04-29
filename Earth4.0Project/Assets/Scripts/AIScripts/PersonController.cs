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

    private enum Buildings { None, Factory, Farm, House, Tree };

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
            _targetObject = getTargetObject();
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

    private GameObject getTargetObject()
    {
        Vector3 targetLocation = transform.position;  // don't move if no target
        GameObject targetObject = null;

        if (_fatigue >= 100)
        {
            // Return to house
            targetObject = _attachedHouse;
        }
        else if (_fatigue <= 0)
        {
            // Ready to work
            if (Random.Range(0.0f, 1.0f) <= 0.5f) {
                int farmCount = _world.GetComponent<GameManager>()._farmList.Count;
                if (farmCount > 0) {
                    int farmIndex = Random.Range(0, farmCount);
                    targetObject = _world.GetComponent<GameManager>()._farmList[farmIndex];
                }
            }
            if (targetObject == null) {
                int factoryCount = _world.GetComponent<GameManager>()._factoryList.Count;
                if (factoryCount > 0) {
                    int factoryIndex = Random.Range(0, factoryCount);
                    targetObject = _world.GetComponent<GameManager>()._factoryList[factoryIndex];
                }
            }
        }

        return targetObject;
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
