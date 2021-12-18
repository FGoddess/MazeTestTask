using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shield : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private static Shield _instance;
    public static Shield Instance { get => _instance; }

    [SerializeField] private Material _defaultMat;
    [SerializeField] private Material _shieldMat;
    [SerializeField] private float _timeToPress = 2f;

    private MeshRenderer _playerMesh;
    private PlayerMover _playerMover;

    private Button _button;
    [SerializeField] private Text _timeText;

    private Coroutine _timerCoroutine;

    private bool _isInteractable = true;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);

        _button = GetComponent<Button>();
    }

    public MeshRenderer PlayerMesh
    {
        set
        {
            _playerMesh = value;
            _playerMover = _playerMesh.GetComponent<PlayerMover>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isInteractable)
        {
            _playerMover.IsShielded = true;
            _playerMesh.material = _shieldMat;
            _timerCoroutine = StartCoroutine(PressedButtonTimer());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isInteractable)
        {
            _playerMover.IsShielded = false;
            _playerMesh.material = _defaultMat;

            StopCoroutine(_timerCoroutine);
        }
    }

    private IEnumerator PressedButtonTimer()
    {
        while (_timeToPress > 0)
        {
            _timeToPress -= Time.deltaTime;
            _timeText.text = $"Time left: {(int)(_timeToPress * 100) / 100f}";
            yield return null;
        }

        _playerMesh.material = _defaultMat;
        _playerMover.IsShielded = false;
        _isInteractable = false;
        _button.interactable = false;
    }
}
