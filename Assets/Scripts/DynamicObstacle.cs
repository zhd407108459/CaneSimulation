using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DynamicObstacle : MonoBehaviour
{
    [SerializeField] private Transform[] navLocations;
    [SerializeField] private float[] waitTimes;
    [SerializeField] private bool randomNav;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private int _currentIndex;
    private bool _waiting;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.SetDestination(navLocations[0].position);
        _currentIndex = 0;
        _waiting = false;
        if(_animator) _animator.SetBool(IsWalking, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent.remainingDistance < _agent.stoppingDistance && !_waiting)
        {
            if (randomNav) StartCoroutine(WaitThenRandomNav());
            else StartCoroutine(WaitThenNav());
        }
    }

    IEnumerator WaitThenNav()
    {
        _waiting = true;
        if(_animator) _animator.SetBool(IsWalking, false);
        yield return new WaitForSeconds(waitTimes[_currentIndex]);
        _waiting = false;
        if(_animator) _animator.SetBool(IsWalking, true);
        _currentIndex = (_currentIndex + 1) % navLocations.Length;
        _agent.SetDestination(navLocations[_currentIndex].position);
    }
    
    IEnumerator WaitThenRandomNav()
    {
        _waiting = true;
        if(_animator) _animator.SetBool(IsWalking, false);
        yield return new WaitForSeconds(waitTimes[_currentIndex]);
        _waiting = false;
        if(_animator) _animator.SetBool(IsWalking, true);
        _currentIndex = Random.Range(0, navLocations.Length);
        _agent.SetDestination(navLocations[_currentIndex].position);
    }
}
