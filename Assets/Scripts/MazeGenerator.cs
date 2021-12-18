using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    private static List<Neighbour> GetUnvisitedNeighbours(Position pos, WallState[,] maze, int width, int height)
    {
        var neighbours = new List<Neighbour>();

        if(pos.X > 0)
        {
            if(!maze[pos.X - 1, pos.Y].HasFlag(WallState.Visited))
            {
                neighbours.Add(new Neighbour { Position = new Position { X = pos.X - 1, Y = pos.Y }, SharedWall = WallState.Left });
            }
        }

        if(pos.Y > 0)
        {
            if(!maze[pos.X, pos.Y - 1].HasFlag(WallState.Visited))
            {
                neighbours.Add(new Neighbour { Position = new Position { X = pos.X, Y = pos.Y - 1}, SharedWall = WallState.Down });
            }
        }

        if(pos.Y < height - 1)
        {
            if(!maze[pos.X, pos.Y + 1].HasFlag(WallState.Visited))
            {
                neighbours.Add(new Neighbour { Position = new Position { X = pos.X, Y = pos.Y + 1}, SharedWall = WallState.Up });
            }
        }

        if(pos.X < width - 1)
        {
            if(!maze[pos.X + 1, pos.Y].HasFlag(WallState.Visited))
            {
                neighbours.Add(new Neighbour { Position = new Position { X = pos.X + 1, Y = pos.Y}, SharedWall = WallState.Right });
            }
        }

        return neighbours;
    }

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.Left: return WallState.Right;
            case WallState.Up: return WallState.Down;
            case WallState.Down: return WallState.Up;
            default: return WallState.Left;
        }
    }

    private static WallState[,] ApplyRecursiveBackTracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random();
        var positionStack = new Stack<Position>();
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };

        maze[position.X, position.Y] |= WallState.Visited;
        positionStack.Push(position);

        while(positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if(neighbours.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, neighbours.Count);
                var randNeighbour = neighbours[randIndex];

                var nPos = randNeighbour.Position;
                maze[current.X, current.Y] &= ~randNeighbour.SharedWall;
                maze[nPos.X, nPos.Y] &= ~GetOppositeWall(randNeighbour.SharedWall);

                maze[nPos.X, nPos.Y] |= WallState.Visited;

                positionStack.Push(nPos);
            }
        }

        return maze;
    }

    public static WallState[,] Generate(int width, int height) 
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.Right | WallState.Left | WallState.Down | WallState.Up;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
            }
        }

        return ApplyRecursiveBackTracker(maze, width, height);
    }
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public enum WallState
{
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
    Visited = 128
}