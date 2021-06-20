using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace raytracer
{
    public static class Compute
    {
        const int MAX_DEPTH = 10;

        static float Mix(float a, float b, float mix) 
        { 
            return b * mix + a * (1 - mix); 
        }

        static float min1(float item)
        {
           return item > 1 ? 1 : item;
        }

        public static void Render(IEnumerable<Sphere> spheres)
        {
            int width = 1920, height = 1080;

            List<Vector3> pixels = new List<Vector3>(width * height);

            float invWidth = 1 / (float)width, invHeight = 1 / (float)height;
            float fov = 30, aspectratio = width / (float)height;
            float angle = (float)Math.Tan(Math.PI * 0.5 * fov / 180);

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    float xx = (2 * ((x + 0.5f) * invWidth) - 1) * angle * aspectratio;
                    float yy = (1 - 2 * ((y + 0.5f) * invHeight)) * angle;

                    Vector3 rayDir = new Vector3(xx, yy, -1);
                    rayDir = Vector3.Normalize(rayDir);

                    pixels.Add(Trace(Vector3.Zero, rayDir, spheres, 0));
                }
            }

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Console.WriteLine($"Writing to {docPath}");

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Image.ppm")))
            {

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes($"P6\n{width} {height}\n255\n"); // If your using UTF8
                outputFile.BaseStream.Write(byteArray, 0, byteArray.Length);

                foreach (var pixel in pixels)
                {
                    var data = new byte[] {
                        (byte)(min1(pixel.X) * 255),
                        (byte)(min1(pixel.Y) * 255),
                        (byte)(min1(pixel.Z) * 255),
                    };

                    outputFile.BaseStream.Write(data, 0, 3);
                }

                outputFile.Flush();
                outputFile.Close();
            }

            Console.WriteLine("Done!!");
        }
       

        public static Vector3 Trace(Vector3 origin, Vector3 direction, IEnumerable<Sphere> spheres, int depth)
        {
            float tNear = float.MaxValue;
            Sphere sphere = null;

            foreach(var item in spheres)
            {
                float t0 = float.MaxValue;
                float t1 = float.MaxValue;

                if (!item.Intersect(origin, direction, ref t0, ref t1))
                    continue;
  
                if (t0 < 0) t0 = t1;
                if (t0 < tNear)
                {
                    tNear = t0;
                    sphere = item;
                }
            }

            if (sphere == null)
            {
                return new Vector3(2);
            }

            Vector3 surfaceColour = Vector3.Zero;
            Vector3 phit = origin + direction * tNear;
            Vector3 nhit = phit - sphere.center;
            nhit = Vector3.Normalize(nhit);

            float bias = 0.0001f;
            bool inside = false;

            if (Vector3.Dot(direction, nhit) > 0) {
                nhit = -nhit;
                inside = true;
            }

            if ((sphere.trasparency > 0 || sphere.reflection > 0) && depth < MAX_DEPTH)
            {
                float facingratio = -Vector3.Dot(direction, nhit);
                float fresneleffect = Mix((float)Math.Pow(1 - facingratio, 3), 1, 0.1f);
                // compute reflection direction (not need to normalize because all vectors
                // are already normalized)

                Vector3 reflDir = direction - nhit * 2 * Vector3.Dot(direction, nhit);
                reflDir = Vector3.Normalize(reflDir);

                Vector3 reflection = Trace(phit + nhit * bias, reflDir, spheres, depth + 1);
                Vector3 refraction = Vector3.Zero;

                // if the sphere is also transparent compute refraction ray (transmission)
                if (sphere.trasparency != 0)
                {
                    float ior = 1.1f, eta = (inside) ? ior : 1 / ior; // are we inside or outside the surface? 
                    float cosi = -Vector3.Dot(nhit, direction);
                    float k = 1 - eta * eta * (1 - cosi * cosi);
                    Vector3 refDir = direction * eta + nhit * (eta * cosi - (float)Math.Sqrt(k));
                    refDir = Vector3.Normalize(refDir);
                   
                    refraction = Trace(phit - nhit * bias, refDir, spheres, depth + 1);
                }
                // the result is a mix of reflection and refraction (if the sphere is transparent)
                surfaceColour = (
                    reflection * fresneleffect +
                    refraction * (1 - fresneleffect) * sphere.trasparency) * sphere.surfaceColour;
            } else
            {
                foreach(var item in spheres)
                {
                    if (item.emissionColour.X <= 0) continue;

                    Vector3 transmission = Vector3.One;
                    Vector3 lighDirection = item.center - phit;

                    lighDirection = Vector3.Normalize(lighDirection);

                    foreach (var inner_item in spheres)
                    {
                        if (inner_item == item) continue;

                        float t0 = 0, t1 = 0;

                        if (inner_item.Intersect(phit + nhit * bias, lighDirection, ref t0, ref t1))
                        {
                            transmission = Vector3.Zero;
                            break;
                        }
                    }

                    var extra = Vector3.Dot(nhit, lighDirection);
                    if (extra < (float)0.0) extra = 0;

                    surfaceColour += sphere.surfaceColour * transmission * extra * sphere.emissionColour;
                }
            }


            return surfaceColour + sphere.emissionColour;
        }
    }
}
