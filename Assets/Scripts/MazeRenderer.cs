using System;
using UnityEngine;
using UnityEngine.AI;

public class MazeRenderer : MonoBehaviour
{
    private static MazeRenderer _instance;
    public static MazeRenderer Instance { get => _instance; }

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private Transform _wallTemplate;
    [SerializeField] private Transform _startTemplate;
    [SerializeField] private Transform _endTemplate;
    [SerializeField] private Transform _dangerZoneTemplate;

    [SerializeField] private int dangerZonesCount = 3;
    private float size = 1f;

    [SerializeField] private NavMeshSurface _navMeshSurface;

    public event Action<Transform> RenderComplete;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        var maze = MazeGenerator.Generate(width, height);
        Draw(maze);
    }

    private void Draw(WallState[,] maze)
    {
        var start = Instantiate(_startTemplate, transform);
        start.position = new Vector3(-width / 2, 0, -height / 2);

        var endZone = Instantiate(_endTemplate, transform);
        endZone.position = new Vector3(-width / 2 + width - 1, -0.25f, -height / 2 + height - 1);

        var randomCell = maze[UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height)];

        int counter = 0;
        while (counter < dangerZonesCount)
        {
            var dZone = Instantiate(_dangerZoneTemplate, transform);
            dZone.position = new Vector3(-width / 2 + UnityEngine.Random.Range(0, width), -0.3f, -height / 2 + UnityEngine.Random.Range(0, height));
            if(dZone.position == start.position || dZone.position == endZone.position)
            {
                Destroy(dZone.gameObject);
                continue;
            }
            counter++;
        }


        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                if (cell.HasFlag(WallState.Up))
                {
                    var topWall = Instantiate(_wallTemplate, transform);
                    topWall.position = position + new Vector3(0, 0, size / 2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.Left))
                {
                    var leftWall = Instantiate(_wallTemplate, transform);
                    leftWall.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.Right))
                    {
                        var rightWall = Instantiate(_wallTemplate, transform);
                        rightWall.position = position + new Vector3(size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.Down))
                    {
                        var bottomWall = Instantiate(_wallTemplate, transform);
                        bottomWall.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }

        _navMeshSurface.BuildNavMesh();
        RenderComplete?.Invoke(_dangerZoneTemplate);
    }
}
