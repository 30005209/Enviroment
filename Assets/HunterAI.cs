using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.PropertyField(position, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));
        }
        catch
        {
            EditorGUI.PropertyField(position, property);
        }
    }
}

[RequireComponent(typeof(ColourController))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class HunterAI : MonoBehaviour
{
    enum _setting { _default, _current, _max };

    [SerializeField]
    private Transform _target;

    [SerializeField]
    [NamedArrayAttribute(new string[] { "Default", "Current", "Max" })]
    private float[] _speed;

    [SerializeField]
    [NamedArrayAttribute(new string[] { "Default", "Current", "Max" })]
    private float[] _radius;

    [SerializeField]
    [NamedArrayAttribute(new string[] { "Default", "Current", "Max" })]
    private float[] _time;

    [SerializeField]
    private float _nextWaypointDistance = 3.0f;

    private Path _path;

    private int _currentWaypoint = 0;

    private bool _reachedEndOfPath = false;

    private Seeker _seeker;

    private Rigidbody2D _rb;

    [SerializeField]
    private bool _foundPlayer = false;

    [SerializeField]
    private PatrolPoints _pp;

    [SerializeField]
    private int _perception;

    [SerializeField]
    private CircleCollider2D _triggerRange;


    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        _pp = FindObjectOfType<PatrolPoints>();

        InvokeRepeating("UpdatePath", 0f, 0.25f);

        _radius = new float[] { 1f, 1f, 3f };
        _speed  = new float[] { 400f, 400f, 600f };
        _time = new float[] { 10f, 10f, 10f };
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
        _time[(int)_setting._current] -= Time.deltaTime;
    }

    void UpdatePath()
    {
        if(_seeker.IsDone())
        {
            _seeker.StartPath(_rb.position, _target.position, OnPathComplete);
        }

        if(!_foundPlayer)
        {
            if(_time[(int)_setting._current] < 0)
            {
                ResetTimer();
                PickRandomTarget();
                ResetSpeed();
            }
        }
        else
        {
            if(_time[(int)_setting._current] < 0)
            {
                ResetTimer();
                _speed[(int)_setting._current] *= 0.8f;
            }
        }
    }

    void ResetTimer()
    {
        _time[(int)_setting._current] = (float)Random.Range(_time[(int)_setting._max] * 0.5f , _time[(int)_setting._max]);

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
        _triggerRange.radius = _radius[(int)_setting._max];
    }

    void ResetRange()
    {
        _triggerRange.radius = _radius[(int)_setting._default];
    }

    void IncreaseSpeed()
    {
        _speed[(int)_setting._current] = _speed[(int)_setting._max];
    }

    void ResetSpeed()
    {
        _speed[(int)_setting._current] = _speed[(int)_setting._default];
    }

    void FoundPlayer()
    {
        _foundPlayer = true;
        IncreaseSpeed();
        IncreaseRange();
    }
    
    void LostPlayer()
    {
        _foundPlayer = false;
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

    void FixedUpdate()
    {
        if(_path != null)
        {
            _reachedEndOfPath =_currentWaypoint >= _path.vectorPath.Count;
            
            Vector2 _direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
            Vector2 _force = _direction * _speed[(int)_setting._current] * Time.deltaTime;

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
