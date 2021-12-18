using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMover : MonoBehaviour
{
    private Transform _target;
    private LineRenderer _line;
    private NavMeshAgent _agent;
    [SerializeField] private Transform _destroyedVersion;

    [SerializeField] private float _startDelay = 2f;

    private Vector3 _startPoint;

    private bool _isShielded;

    public bool IsShielded { get => _isShielded; set => _isShielded = value; }

    private void OnEnable()
    {
        MazeRenderer.Instance.RenderComplete += OnRenderComplete;
    }

    private void OnDisable()
    {
        MazeRenderer.Instance.RenderComplete -= OnRenderComplete;
    }

    public void OnRenderComplete(Transform target)
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
        _startPoint = transform.position;
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
        if (_target != null && _agent.enabled)
        {
            _agent.SetDestination(_target.position);
            GetPath();
        }
    }

    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        var newPlayer = Instantiate(this.gameObject, _startPoint, Quaternion.identity);
        newPlayer.SetActive(false);

        _agent.enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        _destroyedVersion.gameObject.SetActive(true);

        foreach (Transform box in _destroyedVersion)
        {
            box.GetComponent<Rigidbody>().AddExplosionForce(500f, _destroyedVersion.position, 50f);
        }

        yield return new WaitForSeconds(3f);

        _destroyedVersion.gameObject.SetActive(false);

        newPlayer.SetActive(true);
        newPlayer.GetComponent<PlayerMover>().OnRenderComplete(_target);
        newPlayer.GetComponent<MeshRenderer>().enabled = true;
        newPlayer.GetComponent<BoxCollider>().enabled = true;
        Shield.Instance.PlayerMesh = newPlayer.GetComponent<MeshRenderer>();
        Shield.Instance.TimeToPress = 2f;

        Destroy(gameObject);
    }
}
