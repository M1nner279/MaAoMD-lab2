using System;

namespace lab2.Models;

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
    public int Cluster { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
        Cluster = -1;
    }

    public double DistanceTo(Point dst)
    {
        return Math.Sqrt(Math.Pow(X - dst.X, 2) + Math.Pow(Y - dst.Y, 2));
    }
}