using System;
using System.Numerics;
namespace raytracer
{
    public class Sphere
    {
        public Vector3 center { get; }

        public float radius { get; }
        public float radius2 { get; }

        public Vector3 surfaceColour { get; }
        public Vector3 emissionColour { get; }

        public float trasparency { get; }
        public float reflection { get; }

        public Sphere(Vector3 center, float radius, Vector3 surfaceColour, Vector3 emissionColour, float reflection = 0, float trasparency = 0)
        {
            this.center = center;
            this.radius = radius;
            radius2 = radius * radius;
            this.surfaceColour = surfaceColour;
            this.emissionColour = emissionColour;
            this.trasparency = trasparency;
            this.reflection = reflection;
        }


        public Sphere(Vector3 center, float radius, Vector3 surfaceColour, float reflection = 0, float trasparency = 0) : this(center, radius, surfaceColour, Vector3.Zero, reflection, trasparency)
        {
        }

        public bool Intersect(Vector3 rayOrigin, Vector3 rayDireciton, ref float t0, ref float t1)
        {
            var l = center - rayOrigin;
            var tca = Vector3.Dot(l, rayDireciton);
            if (tca < 0) return false;

            var d2 = Vector3.Dot(l, l) - tca * tca;
            if (d2 > radius2) return false;

            var thc = (float)Math.Sqrt(radius2 - d2);
            t0 = tca - thc;
            t1 = tca + thc;

            return true;
        }
    }
}
