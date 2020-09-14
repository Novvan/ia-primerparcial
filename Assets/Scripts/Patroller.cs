using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Patroller : MonoBehaviour
{
    [SerializeField] private int _chanceToShoot;
    [SerializeField] private int _chanceToPersuit;
    [SerializeField] private float _viewRange;
    [SerializeField] private float _angleSight;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _timeToAction;
    [SerializeField] private float _speed;
    [SerializeField] private float _turnSmooth = 0.1f;
    [SerializeField] private List<Transform> _path;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _bulletPoint;
    private Random _rnd = new Random();
    private bool _rltResult;
    private int _nextWaypoint;
    private int _indexModifier = 1;
    private float _turnSmoothVelocity;
    private float _timeSpent;
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;
    private float _gravity = -9.8f;
    private Dictionary<bool, int> _dictionary;

    public enum State
    {
        idle,
        patrol,
        action
    }

    private State _currentState;

    private void Start()
    {
        _dictionary = new Dictionary<bool, int>();
        _dictionary.Add(true, _chanceToShoot);
        _dictionary.Add(false, _chanceToPersuit);
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rltResult = _roulette();
        _currentState = State.idle;
    }

    private void Update()
    {
        _stateMachine();
        var _rc = Physics.Raycast(transform.position, Vector3.down, 0.6f, _layerMask);
        if (_rc) _velocity = Vector3.zero;
        else _velocity.y += _gravity * Time.deltaTime;
        transform.position += _velocity;
    }

    private void _stateMachine()
    {
        switch (_currentState)
        {
            case State.idle:
                _idleStatusBehaviour();
                break;
            case State.patrol:
                _patrolStatusBehaviour();
                break;
            case State.action:
                _actionStatusBehaviour();
                break;
        }
    }

    private void _idleStatusBehaviour()
    {
        _currentState = _playerDetection() ? State.action : _currentState;
        if (_timeSpent >= _timeToAction)
        {
            _currentState = State.patrol;
            _timeSpent = 0;
        }
        else _timeSpent += Time.deltaTime;

        _animator.SetBool("isRunning", false);
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isIdle", true);
    }

    private void _patrolStatusBehaviour()
    {
        _currentState = _playerDetection() ? State.action : _currentState;
        _patrol();
        //Animator bools
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isWalking", true);
        _animator.SetBool("isIdle", false);
    }

    private void _actionStatusBehaviour()
    {
        _currentState = _playerDetection() ? State.action : State.idle;
        
        if (_rltResult)
        {
            //Debug.Log("Pursuit");
            _pursuit();
        }
        else
        {
            //Debug.Log("Shoot");
            _shoot();
        }
        
        //_pursuit();
        //Patroller Settings
        _timeSpent = 0;
        _animator.SetBool("isRunning", true);
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isIdle", false);
    }

    private void _shoot()
    {
        var bul = Instantiate(_bullet, transform);
        //Debug.Log("instance created: " + bul);
        var dir = _playerTransform.position - transform.position;
        bul.GetComponent<Bullet>().BulletShot(dir);
    }

    private void _pursuit()
    {
        Vector3 direction = _playerTransform.position - transform.position;
        _move(direction);
    }

    private void _patrol()
    {
        var point = _path[_nextWaypoint];
        var posPoint = point.position;
        posPoint.y = transform.position.y;
        Vector3 dir = posPoint - transform.position;
        if (dir.magnitude > 0.25f && _currentState == State.patrol)
        {
            _move(dir);
        }
        else
        {
            _currentState = State.idle;
            if (_nextWaypoint >= _path.Count - 1) _nextWaypoint = 0;
            else _nextWaypoint++;
        }
    }


    private void _move(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            float _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity,
                _turnSmooth);
            transform.rotation = Quaternion.Euler(0f, _angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
            transform.position += moveDirection * _speed * Time.deltaTime;
        }
        else
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private bool _playerDetection()
    {
        Vector3 difference = _playerTransform.position - transform.position;
        float distance = difference.magnitude;
        if (distance > _viewRange) return false;
        float angleToTarget = Vector3.Angle(transform.forward, difference);
        if (angleToTarget > _angleSight / 2) return false;
        return !Physics.Raycast(transform.position, difference.normalized, distance, _layerMask);
    }

    private bool _roulette()
    {
        float totalProbs = 0;
        foreach (var var in _dictionary)
        {
            totalProbs += var.Value;
        }

        float random = _rnd.Next(0, (int) totalProbs);
        foreach (var var in _dictionary)
        {
            random -= var.Value;
            if (random < 0)
            {
                return var.Key;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * _viewRange);
        Gizmos.DrawWireSphere(transform.position, _viewRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _angleSight / 2, 0) * transform.forward * _viewRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_angleSight / 2, 0) * transform.forward * _viewRange);
    }
}