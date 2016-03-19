using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blink.Utilities
{
    static class VectorMath
    {
        /// <summary>
        /// Returns a unit vector pointing from point "from" to point "to"
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector2 vectorTo(Vector2 from, Vector2 to)
        { 
            Vector2 distVector = new Vector2(to.X - from.X, from.Y - to.Y);
            distVector.Normalize();
            return distVector;
        }

        public static float rotationFromVector(Vector2 vect)
        {
            float angle = (float)Math.Atan(vect.X / vect.Y);
            angle = (vect.Y < 0 ? (float)(Math.PI) + angle : angle);

            return angle-(float)(Math.PI*0.5f);
        }

        public static float angleBetweenVectors(Vector2 a, Vector2 b)
        {
            return (float)Math.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
        }

        public static bool rectCollision(Rectangle a, float aRot, Rectangle b, float bRot)
        {
            Vector2[] aVerts = rectVerts(a, aRot);
            Vector2[] bVerts = rectVerts(b, bRot);


            return convexCollision(aVerts, a.Center.ToVector2(), bVerts, b.Center.ToVector2(), false, 3);
        }

        public static bool rectCollision(Rectangle a, float aRot, Rectangle b, float bRot, Point origin)
        {
            Vector2[] aVerts = rectVerts(a, aRot, origin);
            Vector2[] bVerts = rectVerts(b, bRot);


            return convexCollision(aVerts, a.Center.ToVector2(), bVerts, b.Center.ToVector2(), false, 3);
        }

        /// <summary>
        /// Returns the corners of the rectangle in XY pairs after rotating by rRot radians
        /// </summary>
        /// <param name="r">Rectangle to be rotated</param>
        /// <param name="rRot">Rotation in radians</param>
        /// <returns>Array of Vector2 points</returns>
        public static Vector2[] rectVerts(Rectangle r, float rRot, Point origin)
        {
            
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2((float)(origin.X + (r.Left - origin.X) * Math.Cos(rRot) - (r.Top - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Left - origin.X) * Math.Sin(rRot) + (r.Top - origin.Y) * Math.Cos(rRot)));
            points[1] = new Vector2((float)(origin.X + (r.Right - origin.X) * Math.Cos(rRot) - (r.Top - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Right - origin.X) * Math.Sin(rRot) + (r.Top - origin.Y) * Math.Cos(rRot)));
            points[2] = new Vector2((float)(origin.X + (r.Right - origin.X) * Math.Cos(rRot) - (r.Bottom - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Right - origin.X) * Math.Sin(rRot) + (r.Bottom - origin.Y) * Math.Cos(rRot)));
            points[3] = new Vector2((float)(origin.X + (r.Left - origin.X) * Math.Cos(rRot) - (r.Bottom - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Left - origin.X) * Math.Sin(rRot) + (r.Bottom - origin.Y) * Math.Cos(rRot)));
            return points;
        }

        public static Vector2[] rectVerts(Rectangle r, float rRot)
        {
            
            Point origin = r.Center;
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2((float)(origin.X + (r.Left - origin.X) * Math.Cos(rRot) - (r.Top - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Left - origin.X) * Math.Sin(rRot) + (r.Top - origin.Y) * Math.Cos(rRot)));
            points[1] = new Vector2((float)(origin.X + (r.Right - origin.X) * Math.Cos(rRot) - (r.Top - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Right - origin.X) * Math.Sin(rRot) + (r.Top - origin.Y) * Math.Cos(rRot)));
            points[2] = new Vector2((float)(origin.X + (r.Right - origin.X) * Math.Cos(rRot) - (r.Bottom - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Right - origin.X) * Math.Sin(rRot) + (r.Bottom - origin.Y) * Math.Cos(rRot)));
            points[3] = new Vector2((float)(origin.X + (r.Left - origin.X) * Math.Cos(rRot) - (r.Bottom - origin.Y) * Math.Sin(rRot)), (float)(origin.Y + (r.Left - origin.X) * Math.Sin(rRot) + (r.Bottom - origin.Y) * Math.Cos(rRot)));
            return points;
        }

        //Sphere-polygon collision is a WIP
        /*
        public static bool sphereRectCollision(Vector2 sphereLoc, float sphereRad, Vector2[] bVerts, Vector2 bOrigin, bool closed = true, int vertNum = -1)
        {
            int v = 0;
            if (vertNum == -1)
                vertNum = bVerts.Length - 1;
            if (closed)
                vertNum++;
            while (v < vertNum)
            {
                Vector2 axis1 = vectorTo(bVerts[v], bVerts[(v + 1) % bVerts.Length]);
                axis1.Y *= -1;
                bool axis1Collision = sphereAxisCollision(sphereLoc, sphereRad, bVerts, axis1);

                if (!axis1Collision)
                    break;

                v++;
            }
            return false;
        }

        public static bool sphereAxisCollision(Vector2 sphereLoc, float sphereRad, Vector2[] bVerts, Vector2 axis)
        {
            float aMin, aMax, bMin, bMax;
            float angle = angleBetweenVectors(aVerts[0], axis);
            float dist = (float)Math.Cos(angle) * aVerts[0].Length();
            aMin = dist;
            aMax = dist;
            for (int v = 1; v < aVerts.Length; v++)
            {
                angle = angleBetweenVectors(aVerts[v], axis);
                dist = (float)Math.Cos(angle) * aVerts[v].Length();
                if (dist < aMin)
                    aMin = dist;
                else if (dist > aMax)
                    aMax = dist;
            }

            angle = angleBetweenVectors(bVerts[0], axis);
            dist = (float)Math.Cos(angle) * bVerts[0].Length();
            bMin = dist;
            bMax = dist;
            for (int v = 1; v < bVerts.Length; v++)
            {
                angle = angleBetweenVectors(bVerts[v], axis);
                dist = (float)Math.Cos(angle) * bVerts[v].Length();
                if (dist < bMin)
                    bMin = dist;
                else if (dist > bMax)
                    bMax = dist;
            }

            return (aMin >= bMin && aMin <= bMax) || (aMax >= bMin && aMax <= bMax);
        }*/

        public static bool convexCollision(Vector2[] aVerts, Vector2 aOrigin, Vector2[] bVerts, Vector2 bOrigin, bool closed = true, int vertNum = -1)
        {
            int v = 0;
            if(vertNum == -1)
                vertNum = aVerts.Length - 1;
            if (closed)
                vertNum ++;
            while (v < vertNum)
            {
                Vector2 axis1 = vectorTo(aVerts[v], aVerts[(v+1)%aVerts.Length]);
                axis1.Y *= -1;
                bool axis1Collision = axisCollision(aVerts, bVerts, axis1);

                if (!axis1Collision)
                    break;

                v++;
            }
            

            //Vector2 axis2 = new Vector2(axis1.Y, -axis1.X);
            //bool axis2Collision = axisCollision(aVerts, bVerts, axis2);

            return v == aVerts.Length-1;
        }

        public static bool axisCollision(Vector2[] aVerts, Vector2[] bVerts, Vector2 axis)
        {
            float aMin, aMax, bMin, bMax;
            float angle = angleBetweenVectors(aVerts[0], axis);
            float dist = (float)Math.Cos(angle) * aVerts[0].Length();
            aMin = dist;
            aMax = dist;
            for (int v = 1; v < aVerts.Length; v++)
            {
                angle = angleBetweenVectors(aVerts[v], axis);
                dist = (float)Math.Cos(angle) * aVerts[v].Length();
                if (dist < aMin)
                    aMin = dist;
                else if (dist > aMax)
                    aMax = dist;
            }

            angle = angleBetweenVectors(bVerts[0], axis);
            dist = (float)Math.Cos(angle) * bVerts[0].Length();
            bMin = dist;
            bMax = dist;
            for (int v = 1; v < bVerts.Length; v++)
            {
                angle = angleBetweenVectors(bVerts[v], axis);
                dist = (float)Math.Cos(angle) * bVerts[v].Length();
                if (dist < bMin)
                    bMin = dist;
                else if (dist > bMax)
                    bMax = dist;
            }

            return (aMin >= bMin && aMin <= bMax) || (aMax >= bMin && aMax <= bMax);
        }
    }
}
