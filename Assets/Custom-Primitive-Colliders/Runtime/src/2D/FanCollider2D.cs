/*
Custom-Primitive-Colliders
Copyright (c) 2018 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;

namespace CustomPrimitiveColliders
{

    [AddComponentMenu("CustomPrimitiveColliders/2D/Fan Collider 2D"), RequireComponent(typeof(PolygonCollider2D))]
    public sealed class FanCollider2D : Base2DCustomCollider
    {

        const float MIN_RAD = 0.01f;
        const int MIN_ANGLE = 1;
        const int MAX_ANGLE = 360;
        const int MIN_VERTICES = 4;

        #region Fields

        [SerializeField]
        private float m_radius = 1f;
        [SerializeField, Range(1, 360)]
        private int m_fanAngle = 135;
        [SerializeField]
        private int m_numVertices = 32;

        #endregion

        #region CONSTRUCTOR

        private void Awake()
        {
            this.Recreate();
        }

#if UNITY_EDITOR

        private void Reset()
        {
            this.Recreate();
        }

        private void OnValidate()
        {
            this.Recreate();
        }

#endif

        #endregion

        #region Methods

        public override void Recreate()
        {
            this.Configure(m_radius, m_fanAngle, m_numVertices);
        }

        public void Configure(float radius, int fanAngle, int numVertices = 32)
        {
            m_radius = Mathf.Max(radius, MIN_RAD);
            m_fanAngle = Mathf.Clamp(fanAngle, MIN_ANGLE, MAX_ANGLE);
            m_numVertices = Mathf.Max(numVertices, MIN_VERTICES);

            Vector2[] points = CreatePoints(m_radius, m_fanAngle, m_numVertices);

            polygonCollider2d.points = null;
            polygonCollider2d.points = points;
        }

        private static Vector2[] CreatePoints(float radius, int fanAngle, int numVertices)
        {
            Vector2[] points = new Vector2[numVertices + (fanAngle == 360 ? 2 : 1)];

            Quaternion quatStep = Quaternion.Euler(0f, 0f, fanAngle / (float)(fanAngle == 360 ? numVertices : (numVertices - 1)));

            points[0] = Vector2.zero;

            for (int i = 1; i <= numVertices; i++)
            {
                if (i == 1)
                {
                    points[i] = new Vector2(radius, 0f);
                }
                else
                {
                    points[i] = quatStep * points[i - 1];
                }
            }

            if (fanAngle == 360)
            {
                points[points.Length - 1] = points[1];
            }

            Vector2 meshForward;
            int centerIndex = Mathf.FloorToInt(numVertices / 2);
            if (numVertices % 2 == 0)
            {
                meshForward = (points[centerIndex] - points[0]) + (points[centerIndex + 1] - points[0]);
            }
            else
            {
                meshForward = points[centerIndex + 1] - points[0];
            }

            Quaternion quat = Quaternion.FromToRotation(meshForward, Vector3.up);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = quat * points[i];
            }

            return points;
        }

        #endregion

    }

}