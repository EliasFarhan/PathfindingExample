using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BFS : MonoBehaviour
{
	private const int sizeX = 100;
	private const int sizeY = 100;
	private const float ratio = 0.25f;
	Point[,] graph = new Point[sizeX, sizeY];
	
	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeY; j++)
			{
				graph[i,j] = new Point(i,j);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		CalculateBFS(Camera.main.ScreenToWorldPoint(Input.mousePosition));	
	}

	void CalculateBFS(Vector3 position)
	{
		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeY; j++)
			{
				graph[i, j].Visited = false;
			}
		}
		Point targetPoint = new Point((int)Math.Round(position.x*ratio), (int)Math.Round(position.y*ratio));
		targetPoint += new Point(sizeX/2, sizeY/2);
		if (targetPoint.X < 0 || targetPoint.X >= sizeX || targetPoint.Y < 0 || targetPoint.Y >= sizeY)
		{
			return;
		}
		Point startingPoint = new Point(sizeX/2, sizeY/2);
		Point currentPoint = startingPoint;
		Queue<Point> tmpNeighbors = new Queue<Point>();
		while (currentPoint != targetPoint)
		{
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					if (dx == dy || dx == -dy)
					{
						continue;
					}
					Point neighborPoint = new Point(currentPoint.X+dx, currentPoint.Y+dy);
					if (neighborPoint.X < 0 || neighborPoint.X >= sizeX || neighborPoint.Y < 0 || neighborPoint.Y >= sizeY)
					{
						continue;
					}

					if (graph[neighborPoint.X, neighborPoint.Y].Visited)
					{
						continue;
					}

					graph[neighborPoint.X, neighborPoint.Y].Visited = true;
					tmpNeighbors.Enqueue(neighborPoint);
				}
			}

		}
	}

	void OnDrawGizmos()
	{
		foreach (var point in graph)
		{
			DrawPoint((point-new Point(sizeX/2, sizeY/2)).toVec3(ratio), Color.blue);
		}
	}

	void DrawPoint(Vector3 position, Color pointColor)
	{
		Gizmos.color = pointColor;
		Gizmos.DrawLine(position-Vector3.one*ratio/4, position+Vector3.one*ratio/4);
		Gizmos.DrawLine(position - new Vector3(-1,1) * ratio / 4, position - new Vector3(1, -1) * ratio / 4);
	}
}

public class Point
{
	public int X { get; private set; }
	public int Y { get; private set; }
	public bool Visited { get; set; }
	public Point parentPoint = null;
	public Point(int x, int y) 
	{
		X = x;
		Y = y;
		Visited = false;
	}
	public static Point operator -(Point p1, Point p2)
	{
		return new Point(p1.X - p2.X, p1.Y - p2.Y);
	}
	public static Point operator +(Point p1, Point p2)
	{
		return new Point(p1.X + p2.X, p1.Y + p2.Y);
	}
	
	public static bool operator ==(Point p1, Point p2)
	{
		return p1.X==p2.X && p1.Y == p2.Y;
	}

	public static bool operator !=(Point p1, Point p2)
	{
		return !(p1 == p2);
	}

	public Vector3 toVec3(float ratio)
	{
		return new Vector3(X*ratio, Y*ratio);
	}
	public override bool Equals(object obj)
	{
		if (obj is Point)
		{
			return (Point) obj == this;
		}
		return false;
	}
	public override int GetHashCode()
	{
		return (X * 397) ^ Y;
	}
}