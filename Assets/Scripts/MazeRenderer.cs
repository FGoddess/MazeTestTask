using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeRenderer : MonoBehaviour
{
    private static MazeRenderer _instance;
    public static MazeRenderer Instance { get => _instance; }

    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private Transform _wallTemplate;
    [SerializeField] private Transform _startTemplate;
    [SerializeField] private Transform _endTemplate;
    [SerializeField] private Transform _dangerZoneTemplate;

    [SerializeField] private int _dangerZonesCount = 3;
    private float _size = 1f;
    private float _yOffset = -0.3f;

    private int _minWallsToRemove = 10;
    private int _maxWallsToRemove = 20;


    [SerializeField] private NavMeshSurface _navMeshSurface;

    public event Action<Transform> RenderComplete;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        var maze = MazeGenerator.Generate(_width, _height);
        Draw(maze);
    }

    private void Draw(WallState[,] maze)
    {
        var player = Instantiate(_startTemplate, transform);
        player.position = new Vector3(-_width / 2, 0, -_height / 2);
        
        var endZone = Instantiate(_endTemplate, transform);
        endZone.position = new Vector3(-_width / 2 + _width - 1, _yOffset, -_height / 2 + _height - 1);

        var randomCell = maze[UnityEngine.Random.Range(0, _width), UnityEngine.Random.Range(0, _height)];

        int counter = 0;
        while (counter < _dangerZonesCount)
        {
            var dZone = Instantiate(_dangerZoneTemplate, transform);
            dZone.position = new Vector3(-_width / 2 + UnityEngine.Random.Range(0, _width), _yOffset, -_height / 2 + UnityEngine.Random.Range(0, _height));
            if((dZone.position.x == Mathf.Round(player.position.x) && dZone.position.z == Mathf.Round(player.position.z)) || (dZone.position.x == endZone.position.x && dZone.position.z == endZone.position.z))
            {
                Destroy(dZone.gameObject);
                continue;
            }
            counter++;
        }

        List<Transform> walls = new List<Transform>(); 

        for (int i = 0; i < _width; ++i)
        {
            for (int j = 0; j < _height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-_width / 2 + i, 0, -_height / 2 + j);

                if (cell.HasFlag(WallState.Up))
                {
                    var topWall = Instantiate(_wallTemplate, transform);
                    topWall.position = position + new Vector3(0, 0, _size / 2);
                    topWall.localScale = new Vector3(_size, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.Left))
                {
                    var leftWall = Instantiate(_wallTemplate, transform);
                    leftWall.position = position + new Vector3(-_size / 2, 0, 0);
                    leftWall.localScale = new Vector3(_size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                    if (i > 0 && j > 0)
                    {
                        walls.Add(leftWall);
                    }
                }

                if (i == _width - 1)
                {
                    if (cell.HasFlag(WallState.Right))
                    {
                        var rightWall = Instantiate(_wallTemplate, transform);
                        rightWall.position = position + new Vector3(_size / 2, 0, 0);
                        rightWall.localScale = new Vector3(_size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.Down))
                    {
                        var bottomWall = Instantiate(_wallTemplate, transform);
                        bottomWall.position = position + new Vector3(0, 0, -_size / 2);
                        bottomWall.localScale = new Vector3(_size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }

        var randX = UnityEngine.Random.Range(_minWallsToRemove, _maxWallsToRemove);

        counter = 0;

        while(counter < randX)
        {
            var rand = UnityEngine.Random.Range(0, walls.Count);
            if (walls[rand].position.z == 3.5f)
            {
                continue;
            }

            walls[rand].gameObject.SetActive(false);
            counter++;
        }

        _navMeshSurface.BuildNavMesh();
        RenderComplete?.Invoke(_dangerZoneTemplate);
        Shield.Instance.PlayerMesh = player.GetComponent<MeshRenderer>();
    }
}
