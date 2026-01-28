/*
Custom-Primitive-Colliders
Copyright (c) 2018 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;

namespace CustomPrimitiveColliders
{
    [AddComponentMenu("CustomPrimitiveColliders/2D/Triangle Collider 2D"), RequireComponent(typeof(PolygonCollider2D))]
    public sealed class TriangleCollider2D : Base2DCustomCollider
    {

        const float MIN_RAD = 0.01f;
        const float MIN_LEN = 0.01f;
        const float MIN_ANGLE = 0.01f;
        const float MAX_ANGLE = 179f;

        #region Fields

        [SerializeField]
        private bool m_useOpenAngle = true;
        [SerializeField, Range(MIN_ANGLE, MAX_ANGLE)]
        private float m_openAngle = 45;
        [SerializeField, Min(MIN_RAD)]
        private float m_radius = 0.5f;
        [SerializeField, Min(MIN_LEN)]
        private float m_length = 1f;

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
            if (m_useOpenAngle)
            {
                this.ConfigureOpenAngle(m_openAngle, m_length);
            }
            else
            {
                this.ConfigureRadius(m_radius, m_length);
            }
        }
        public void ConfigureRadius(float radius, float length)
        {
            m_radius = Mathf.Max(radius, 0.01f);
            m_length = Mathf.Max(length, 0.01f);
            m_useOpenAngle = false;
            m_openAngle = 2f * Mathf.Atan(radius / length) * Mathf.Rad2Deg;

            Vector2[] points = CreatePoints(m_radius, m_length);

            polygonCollider2d.points = null;
            polygonCollider2d.points = points;
        }

        public void ConfigureOpenAngle(float angle, float length)
        {
            angle = Mathf.Clamp(angle, 0.01f, 179f);
            float radius = length * Mathf.Tan(angle * Mathf.Deg2Rad / 2f);

            m_radius = radius;
            m_length = length;
            m_useOpenAngle = true;
            m_openAngle = angle;

            Vector2[] points = CreatePoints(radius, length);

            polygonCollider2d.points = null;
            polygonCollider2d.points = points;
        }

        private static Vector2[] CreatePoints(float radius, float length)
        {
            Vector2[] points = new Vector2[4];

            points[0] = Vector2.zero;
            points[1] = new Vector2(radius, length);
            points[2] = new Vector2(-radius, length);
            points[3] = Vector2.zero;

            return points;
        }

        #endregion

    }
}