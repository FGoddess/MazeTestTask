using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMover : MonoBehaviour
{
    private Transform _target;
    private LineRenderer _line;
    private NavMeshAgent _agent;

    [SerializeField] private float _startDelay = 2f;

    private void OnEnable()
    {
        MazeRenderer.Instance.RenderComplete += OnRenderComplete;
    }

    private void OnDisable()
    {
        MazeRenderer.Instance.RenderComplete -= OnRenderComplete;
    }

    private void OnRenderComplete(Transform target)
    {
        StartCoroutine(SetTarget(target));
    }

    private IEnumerator SetTarget(Transform target)
    {
        yield return new WaitForSeconds(_startDelay);
        _target = target;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _line = GetComponent<LineRenderer>();
    }

    private void GetPath()
    {
        _line.SetPosition(0, transform.position);
        DrawPath(_agent.path);
    }

    private void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return;

        _line.positionCount = path.corners.Length;

        for (var i = 1; i < path.corners.Length; i++)
        {
            _line.SetPosition(i, path.corners[i]);
        }
    }

    private void Update()
    {
        if (_target != null)
        {
            _agent.SetDestination(_target.position);
            GetPath();
        }
    }

    public void Die()
    {
        Instantiate(this.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        Destroy(gameObject);
    }
}
