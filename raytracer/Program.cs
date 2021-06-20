using System;
using System.Collections.Generic;
using System.Numerics;

namespace raytracer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Sphere> spheres = new List<Sphere>
            {

                // position, radius, surface color, reflectivity, transparency, emission color
                new Sphere(new Vector3(0.0f, -10004, -20), 10000, new Vector3(0.2f), 0, 0.0f),
                new Sphere(new Vector3(0.0f, 0, -20), 4, new Vector3(1.00f, 0.32f, 0.36f), 1f, 0.5f),
                new Sphere(new Vector3(5.0f, -1, -15), 2, new Vector3(0.90f, 0.76f, 0.46f), 1f, 0.0f),
                new Sphere(new Vector3(5.0f, 0, -25), 3, new Vector3(0.65f, 0.77f, 0.97f), 1f, 0.0f),
                new Sphere(new Vector3(-5.5f, 0, -15), 3, new Vector3(0.90f, 0.90f, 0.90f), 1f, 0.0f),
                // light
                new Sphere(new Vector3(0.0f, 20, -30), 3, new Vector3(0.00f, 0.00f, 0.00f), new Vector3(3), 0, 0.0f)
            };
            Compute.Render(spheres);
        }
    }
}
