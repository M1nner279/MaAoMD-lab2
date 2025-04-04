using System;
using System.Collections.Generic;
using System.Linq;

namespace lab2.Models;

public class KMeans
{
    private readonly List<Point> _points;
    private readonly int _k;
    private List<Point> _centroids;
    private readonly Random _random;

    public KMeans(List<Point> points, int k)
    {
        _points = points;
        _k = k;
        _centroids = new List<Point>();
        _random = new Random();
    }

    public void InitializeCentroids()
    {
        _centroids.Clear();
        var indices = Enumerable.Range(0, _points.Count).OrderBy(x => _random.Next()).Take(_k).ToList();

        foreach (var index in indices)
        {
            _centroids.Add(new Point(_points[index].X, _points[index].Y));
        }
    }

    public bool AssignClusters()
    {
        bool changed = false;
        foreach (var point in _points)
        {
            double minDistances = double.MaxValue;
            int closestCentroid = -1;

            for (int i = 0; i < _centroids.Count; i++)
            {
                double dist = point.DistanceTo(_centroids.ElementAt(i));
                if (dist < minDistances)
                {
                    closestCentroid = i;
                    minDistances = dist;
                }
            }

            if (point.Cluster != closestCentroid)
            {
                point.Cluster = closestCentroid;
                changed = true;
            }
        }

        return changed;
    }

    public bool UpdateCentroids()
    {
        bool moved = false;
        var newCentroids = new List<Point>();

        for(int i = 0; i < _k; i++)
        {
            // точки относ к кластеру
            var clusterPoints = _points.Where(p => p.Cluster == i).ToList();
            if (clusterPoints.Count == 0)
            {
                newCentroids.Add(_centroids[i]);
                continue;
            }

            double avgX = clusterPoints.Average(p => p.X);
            double avgY = clusterPoints.Average(p => p.Y);
            var avgPoint = new Point(avgX, avgY);
            var centerPoint = _centroids[i];
            double minDistances = double.MaxValue;
            foreach (var point in clusterPoints)
            {
                double dist = point.DistanceTo(avgPoint);
                if (dist < minDistances)
                {
                    minDistances = dist;
                    centerPoint = point;
                }
            }

            if (centerPoint != _centroids[i])
                moved = true;
            
            newCentroids.Add(centerPoint);
        }

        _centroids = newCentroids;
        return moved;
    }
    
    public (List<Point> Points, List<Point> Centroids) GetCurrentState()
    {
        return (_points, _centroids);
    }
}