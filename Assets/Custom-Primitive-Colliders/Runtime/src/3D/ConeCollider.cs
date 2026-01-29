/*
Custom-Primitive-Colliders
Copyright (c) 2018 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;

namespace CustomPrimitiveColliders
{

    [AddComponentMenu("CustomPrimitiveColliders/3D/Cone Collider"), RequireComponent(typeof(MeshCollider))]
    public sealed class ConeCollider : Base3DCustomCollider
    {

        const float MIN_RAD = 0.01f;
        const float MIN_LEN = 0.01f;
        const float MIN_ANGLE = 0.01f;
        const float MAX_ANGLE = 179f;
        const int MIN_VERTICES = 4;

        #region Fields

        [SerializeField, Tooltip("Optional MeshFilter to update with mesh.")]
        private MeshFilter _meshFilter;

        [SerializeField]
        private bool m_useOpenAngle = true;
        [SerializeField, Range(MIN_ANGLE, MAX_ANGLE)]
        private float m_openAngle = 45;
        [SerializeField, Min(MIN_RAD)]
        private float m_radius = 0.5f;
        [SerializeField, Min(MIN_LEN)]
        private float m_length = 1f;
        [SerializeField, Min(MIN_VERTICES)]
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

        #region Properties

        public bool UseOpenAngle => m_useOpenAngle;
        public float Radius => m_radius;
        public float Length => m_length;
        public float OpenAngle => m_openAngle;
        public int NumVertices => m_numVertices;

        #endregion

        #region Methods

        public override void Recreate()
        {
            if (m_useOpenAngle)
            {
                this.ConfigureOpenAngle(m_openAngle, m_length, m_numVertices);
            }
            else
            {
                this.ConfigureRadius(m_radius, m_length, m_numVertices);
            }
        }

        public void ConfigureRadius(float radius, float length, int numVertices = 32)
        {
            m_radius = Mathf.Max(radius, 0.01f);
            m_length = Mathf.Max(length, 0.01f);
            m_useOpenAngle = false;
            m_openAngle = 2f * Mathf.Atan(radius / length) * Mathf.Rad2Deg;
            m_numVertices = Mathf.Max(numVertices, 4);

            Mesh mesh;
            MeshCollider meshCollider;
            this.GetMeshCollider(out meshCollider, out mesh);
            CreateMesh(mesh, m_radius, m_length, m_numVertices);
            meshCollider.sharedMesh = mesh;
            if (_meshFilter) _meshFilter.sharedMesh = mesh;
        }

        public void ConfigureOpenAngle(float angle, float length, int numVertices = 32)
        {
            angle = Mathf.Clamp(angle, 0.01f, 179f);
            float radius = length * Mathf.Tan(angle * Mathf.Deg2Rad / 2f);

            m_radius = radius;
            m_length = length;
            m_useOpenAngle = true;
            m_openAngle = angle;
            m_numVertices = Mathf.Max(numVertices, 4);

            Mesh mesh;
            MeshCollider meshCollider;
            this.GetMeshCollider(out meshCollider, out mesh);
            CreateMesh(mesh, m_radius, m_length, m_numVertices);
            meshCollider.sharedMesh = mesh;
            if (_meshFilter) _meshFilter.sharedMesh = mesh;
        }

        private static void CreateMesh(Mesh mesh, float radius, float length, int numVertices)
        {
#if UNITY_EDITOR
            mesh.name = $"ConeCollider-Mesh-{numVertices}_radius_{radius}_length_{length}";
#else
            mesh.name = "ConeCollider-Mesh";
#endif

            //Vector3[] vertices = new Vector3[numVertices * 3];
            //Vector3[] normals = new Vector3[numVertices * 3];
            //Vector2[] uvs = new Vector2[numVertices * 3];
            //int[] triangles = new int[numVertices * 6];

            int vert_len = numVertices * 3;
            int tri_len = numVertices * 6;
            Vector3[] vertices;
            Vector3[] normals;
            Vector2[] uvs;
            int[] triangles;
            ArrayPool.GetReusableMeshArrays(vert_len, tri_len, out vertices, out normals, out uvs, out triangles);

            try
            {
                float slope = Mathf.Atan(radius / length);
                float slopeSin = Mathf.Sin(slope);
                float slopeCos = Mathf.Cos(slope);

                int triangleCount = 0;

                for (int i = 0; i < numVertices; i++)
                {
                    float angle = 2f * Mathf.PI * i / numVertices;
                    float angleSin = Mathf.Sin(angle);
                    float angleCos = Mathf.Cos(angle);
                    float angleHalf = 2f * Mathf.PI * (i + 0.5f) / numVertices;
                    float angleHalfSin = Mathf.Sin(angleHalf);
                    float angleHalfCos = Mathf.Cos(angleHalf);

                    vertices[i] = Vector3.zero;
                    vertices[i + numVertices] = new Vector3(radius * angleCos, radius * angleSin, length);
                    vertices[i + numVertices * 2] = new Vector3(0, 0, length);

                    normals[i] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                    normals[i + numVertices] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                    normals[i + numVertices * 2] = Vector3.forward;

                    uvs[i] = new Vector2(i / (float)numVertices, 1f);
                    uvs[i + numVertices] = new Vector2(i / (float)numVertices, 0f);
                    uvs[i + numVertices * 2] = new Vector2(0.5f, 0.5f);

                    triangles[triangleCount++] = i + numVertices;
                    triangles[triangleCount++] = i;
                    triangles[triangleCount++] = i == numVertices - 1 ? numVertices : i + numVertices + 1;

                    triangles[triangleCount++] = i + numVertices;
                    triangles[triangleCount++] = i == numVertices - 1 ? numVertices : i + numVertices + 1;
                    triangles[triangleCount++] = i + numVertices * 2;
                }

                mesh.SetVertices(vertices, 0, vert_len);
                mesh.SetNormals(normals, 0, vert_len);
                mesh.SetUVs(0, uvs, 0, vert_len);
                mesh.SetTriangles(triangles, 0, tri_len, 0);
            }
            finally
            {
                ArrayPool.ReleaseMeshArrays(vertices, normals, uvs, triangles);
            }
        }

        #endregion

    }

}