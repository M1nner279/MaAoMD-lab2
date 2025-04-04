using System;
using System.Collections.Generic;
using System.Linq;

namespace lab2.Models;


public class Maximin
{ 
    private readonly List<Point> _points;
    private readonly List<Point> _cores; // Ядра классов
    private readonly Random _random;

    public Maximin(List<Point> points)
    { 
        _points = points;
        _cores = new List<Point>();
        _random = new Random();
    }

    // Инициализация первого ядра
    public void Initialize()
    { 
        _cores.Clear();
        int firstCoreIndex = _random.Next(_points.Count);
        _cores.Add(new Point(_points[firstCoreIndex].X, _points[firstCoreIndex].Y));
        AssignClusters(); // Распределяем точки по первому ядру
    }

    // Выполнение одного шага алгоритма
    public bool Step()
    { 
        if (_cores.Count == 0) return false;

        // Вычисляем максимальные расстояния в каждом классе
        var maxDistances = new List<(Point point, double distance)>();
        foreach (var core in _cores)
        {
            double maxDistance = 0;
            Point farthestPoint = null;
            foreach (var point in _points.Where(p => p.Cluster == _cores.IndexOf(core)))
            {
                double distance = point.DistanceTo(core);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = point;
                }
            }
            if (farthestPoint != null) 
                maxDistances.Add((farthestPoint, maxDistance));
        }

        // Находим наибольшее максимальное расстояние
        if (!maxDistances.Any()) return false;
        var (candidatePoint, maxDist) = maxDistances.OrderByDescending(d => d.distance).First();

        // Вычисляем среднее расстояние между всеми ядрами
        double averageCoreDistance = CalculateAverageCoreDistance();
        //if (averageCoreDistance <= 0) return false; // Если только одно ядро
        
        if (maxDist > averageCoreDistance / 2)
        {
            _cores.Add(new Point(candidatePoint.X, candidatePoint.Y));
            AssignClusters();
            return true;
        }

        return false;
    }

    // Распределение точек по ближайшим ядрам
    private void AssignClusters()
    {
        foreach (var point in _points)
        { 
            double minDistance = double.MaxValue;
            int closestCore = -1;
            for (int i = 0; i < _cores.Count; i++)
            {
                double distance = point.DistanceTo(_cores[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCore = i;
                }
            }
            point.Cluster = closestCore;
        }
    }

    // Вычисление среднего расстояния между ядрами
    private double CalculateAverageCoreDistance()
    {
        if (_cores.Count < 2) return 0;

        double totalDistance = 0;
        int pairCount = 0;
        for (int i = 0; i < _cores.Count - 1; i++)
        {
            for (int j = i + 1; j < _cores.Count; j++)
            {
                totalDistance += _cores[i].DistanceTo(_cores[j]);
                pairCount++;
            }
        }
        return totalDistance / pairCount;
    }

    // Получение текущего состояния
    public (List<Point> Points, List<Point> Cores) GetCurrentState()
    { 
        return (_points, _cores);
    }
}