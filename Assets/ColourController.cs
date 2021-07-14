using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourController : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField]
    //The current colour
    private Color _curColour;

    [SerializeField]
    //The colour it defaults to
    private Color _originalColour;

    [SerializeField]
    //Ability to change
    private bool _canChange = true;

    [SerializeField]
    [Range(1, 10)]
    //Current Timer
    private float _curTime = 1;

    [SerializeField]
    [Range(1, 10)]
    //Max Timer
    private float _maxTime = 1;

    [SerializeField]
    private bool _source;

    [SerializeField]
    private Colours _colours;

    private Color _hunter;

    private Color _default;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        _curColour = sr.color;
        _originalColour = _curColour;
        _hunter = _colours.GetColour(4, 1);
        _default = _colours.GetColour(4, 0);

    }

    private void Update()
    {
        sr.color = _curColour;

        if (sr.color != _hunter)
        {
            if (!_source && !_canChange) 
            {
                _curTime -= Time.deltaTime;

                if (_curTime <= 0)
                {
                    _curTime = _maxTime;
                    _canChange = true;
                }
            }
        }
    }

    private bool ChangeColour(Color _newColour)
    {
        Color _temp = _newColour;

        bool _matching = (_temp == _curColour);

        if (!_matching)
        {

            if(_temp == _hunter)
            {
                if(!_source && _curColour != _default)
                {
                    _temp = _default;
                }
            }
            
            _curColour = _temp;
            _canChange = false;
        }

        return _matching;
    }

    public Color GetColour()
    {
        return _curColour;
    }

    bool IsSource()
    {
        return _source;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_canChange)
        {
            if (collision.gameObject.GetComponent<ColourController>() != null)
            {
                ColourController _temp = collision.gameObject.GetComponent<ColourController>();

                if (_temp.IsSource())
                {
                    ChangeColour(_temp.GetColour());
                }
            }
        }
    }
}
