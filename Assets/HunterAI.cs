using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


[RequireComponent(typeof(ColourController))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class HunterAI : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private float _minSpeed = 200f;

    [SerializeField]
    private float _defaultSpeed;

    [SerializeField]
    private float _curSpeed = 200f;

    [SerializeField]
    private float _nextWaypointDistance = 3.0f;

    private Path _path;

    private int _currentWaypoint = 0;

    private bool _reachedEndOfPath = false;

    private Seeker _seeker;

    private Rigidbody2D _rb;

    [SerializeField]
    private float _maxTime = 10.0f;

    [SerializeField]
    private float _curTime = 10.0f;

    [SerializeField]
    private bool _foundPlayer = false;

    [SerializeField]
    private PatrolPoints _pp;

    [SerializeField]
    private int _perception;

    [SerializeField]
    private CircleCollider2D _triggerRange;

    [SerializeField]
    private float _maxRadius = 3;

    [SerializeField]
    private float _curRadius;

    [SerializeField]
    private float _defaultRadius = 1;


    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        _pp = FindObjectOfType<PatrolPoints>();

        InvokeRepeating("UpdatePath", 0f, 0.25f);

        _curRadius = _defaultRadius;
        _curSpeed = _defaultSpeed;
        _maxSpeed = _defaultSpeed * 1.5f;

    }

    void PickRandomTarget()
    {
        PickNewTarget(_pp.GetPointPos(Random.Range(0, _pp.GetLength())));
    }

    void PickNewTarget(Transform _newTransform)
    {
        _target = _newTransform;
    }

    private void Update()
    {
        _curTime -= Time.deltaTime;
    }

    void UpdatePath()
    {
        if(_seeker.IsDone())
        {
            _seeker.StartPath(_rb.position, _target.position, OnPathComplete);
        }

        if(!_foundPlayer)
        {
            if(_curTime < 0)
            {
                ResetTimer();
                PickRandomTarget();
                ResetSpeed();
            }
        }
        else
        {
            if(_curTime < 0)
            {
                ResetTimer();
                _curSpeed = _curSpeed * 0.8f;
            }
        }
    }

    void ResetTimer()
    {
        _curTime = (float)Random.Range(_maxTime / 2, _maxTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Movement>() != null)
        {
            if (Random.Range(0, 10) - _perception < 0)
            {
                PickNewTarget(collision.transform);
                FoundPlayer();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Movement>() != null && _foundPlayer)
        {
            _foundPlayer = false;
            PickRandomTarget();
            ResetRange();
        }
    }

    void IncreaseRange()
    {
        _triggerRange.radius = _maxRadius;
    }

    void ResetRange()
    {
        _triggerRange.radius = _defaultRadius;
    }

    void ResetSpeed()
    {
        _curSpeed = _defaultSpeed;
    }

    void FoundPlayer()
    {
        _foundPlayer = true;
        _curSpeed = _maxSpeed;
        IncreaseRange();
    }
    
    void LostPlayer()
    {
        _foundPlayer = false;
        _curSpeed = _defaultSpeed;
        ResetRange();
        ResetSpeed();
    }


    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_path != null)
        {
            _reachedEndOfPath =_currentWaypoint >= _path.vectorPath.Count;
            
            Vector2 _direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
            Vector2 _force = _direction * _curSpeed * Time.deltaTime;

            _rb.AddForce(_force);

            float _distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);

            if(_distance < _nextWaypointDistance)
            {
                _currentWaypoint++;
            }
        
        }
        else
        {
            PickRandomTarget();
        }
    }
}
