/*
Custom-Primitive-Colliders
Copyright (c) 2018 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using System.Text;
using UnityEngine;

namespace CustomPrimitiveColliders
{

    [AddComponentMenu("CustomPrimitiveColliders/3D/Fan Cylinder Collider"), RequireComponent(typeof(MeshCollider))]
    public sealed class FanCylinderCollider : Base3DCustomCollider
    {

        const float MIN_RAD = 0.01f;
        const float MIN_HEIGHT = 0.01f;
        const int MIN_ANGLE = 1;
        const int MAX_ANGLE = 360;
        const int MIN_VERTICES = 4;

        #region Fields

        [SerializeField]
        private float m_radius = 1f;
        [SerializeField]
        private float m_height = 1f;
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

        #region Properties

        public float Radius => m_radius;
        public float Height => m_height;
        public int FanAngle => m_fanAngle;
        public int NumVertices => m_numVertices;

        #endregion

        #region Methods

        public override void Recreate()
        {
            Configure(m_radius, m_height, m_fanAngle, m_numVertices);
        }

        public void Configure(float radius, float height, int fanAngle, int numVertices = 32)
        {
            m_radius = Mathf.Max(radius, MIN_RAD);
            m_height = Mathf.Max(height, MIN_HEIGHT);
            m_fanAngle = Mathf.Clamp(fanAngle, MIN_ANGLE, MAX_ANGLE);
            m_numVertices = Mathf.Max(numVertices, MIN_VERTICES);

            Mesh mesh;
            MeshCollider meshCollider;
            this.GetMeshCollider(out meshCollider, out mesh);
            CreateMesh(mesh, m_radius, m_height, m_fanAngle, m_numVertices);
            meshCollider.sharedMesh = mesh;
        }

        private static Mesh CreateMesh(Mesh mesh, float radius, float height, int fanAngle, int numVertices)
        {
#if UNITY_EDITOR
            StringBuilder sbName = new StringBuilder("Cylinder");
            sbName.Append(numVertices);
            sbName.Append("_radius_");
            sbName.Append(radius);
            sbName.Append("_height_");
            sbName.Append(height);
            sbName.Append("_fanAngle_");
            sbName.Append(fanAngle);
            mesh.name = sbName.ToString();
#endif

            Vector3[] vertices = new Vector3[(numVertices * 2) + 2];
            Vector3[] normals = new Vector3[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[numVertices * 4 * 3];

            float halfHeight = height / 2f;
            Vector3 center = Vector3.zero;

            Quaternion quatStep = Quaternion.Euler(0f, fanAngle / (float)(fanAngle == 360 ? numVertices : (numVertices - 1)), 0f);

            vertices[0] = new Vector3(0f, -halfHeight, 0f); ;
            vertices[vertices.Length - 1] = new Vector3(0f, halfHeight, 0f);

            normals[0] = Vector3.down;
            normals[normals.Length - 1] = Vector3.up;

            uvs[0] = new Vector2(0.5f, 0.5f);
            uvs[uvs.Length - 1] = uvs[0];

            int triangleCount = 0;

            for (int i = 1; i <= numVertices; i++)
            {
                if (i == 1)
                {
                    vertices[i] = new Vector3(radius, -halfHeight, 0f);
                    vertices[i + numVertices] = new Vector3(radius, halfHeight, 0f);
                }
                else
                {
                    vertices[i] = quatStep * vertices[i - 1];
                    vertices[i + numVertices] = quatStep * vertices[i + numVertices - 1];
                }

                if (i == numVertices)
                {
                    if (fanAngle >= 360)
                    {
                        triangles[triangleCount++] = 1;
                        triangles[triangleCount++] = i;
                        triangles[triangleCount++] = 0;

                        triangles[triangleCount++] = numVertices * 2;
                        triangles[triangleCount++] = 1 + numVertices;
                        triangles[triangleCount++] = vertices.Length - 1;

                        triangles[triangleCount++] = 1;
                        triangles[triangleCount++] = 1 + numVertices;
                        triangles[triangleCount++] = numVertices;

                        triangles[triangleCount++] = 1 + numVertices;
                        triangles[triangleCount++] = numVertices * 2;
                        triangles[triangleCount++] = numVertices;
                    }
                    else
                    {
                        triangles[triangleCount++] = 1;
                        triangles[triangleCount++] = 1 + numVertices;
                        triangles[triangleCount++] = 0;

                        triangles[triangleCount++] = 1 + numVertices;
                        triangles[triangleCount++] = vertices.Length - 1;
                        triangles[triangleCount++] = 0;

                        triangles[triangleCount++] = 0;
                        triangles[triangleCount++] = vertices.Length - 1;
                        triangles[triangleCount++] = numVertices;

                        triangles[triangleCount++] = vertices.Length - 1;
                        triangles[triangleCount++] = numVertices * 2;
                        triangles[triangleCount++] = numVertices;
                    }
                }
                else
                {
                    triangles[triangleCount++] = i + 1;
                    triangles[triangleCount++] = i;
                    triangles[triangleCount++] = 0;

                    triangles[triangleCount++] = i + numVertices;
                    triangles[triangleCount++] = i + numVertices + 1;
                    triangles[triangleCount++] = vertices.Length - 1;

                    triangles[triangleCount++] = i + 1;
                    triangles[triangleCount++] = i + numVertices + 1;
                    triangles[triangleCount++] = i;

                    triangles[triangleCount++] = i + numVertices + 1;
                    triangles[triangleCount++] = i + numVertices;
                    triangles[triangleCount++] = i;
                }
            }

            Vector3 meshForward;
            int centerIndex = Mathf.FloorToInt(numVertices / 2);
            if (numVertices % 2 == 0)
            {
                meshForward = (vertices[centerIndex] - vertices[0]) + (vertices[centerIndex + 1] - vertices[0]);
            }
            else
            {
                meshForward = vertices[centerIndex + 1] - vertices[0];
            }

            Quaternion quat = Quaternion.FromToRotation(meshForward, Vector3.forward);
            for (int i = 0; i < vertices.Length; i++)
            {
                float y = vertices[i].y;
                vertices[i] = quat * vertices[i];
                vertices[i].y = y;
            }

            for (int i = 1; i <= numVertices; i++)
            {
                normals[i] = vertices[i] - center;
                normals[i + numVertices] = vertices[i + numVertices] - center;

                uvs[i] = new Vector2(0.5f + vertices[i].x / (2 * radius), 0.5f + vertices[i].z / (2 * radius));
                uvs[i + numVertices] = new Vector2(0.5f + vertices[i + numVertices].x / (2 * radius), 0.5f + vertices[i + numVertices].z / (2 * radius));
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            return mesh;
        }

        #endregion

    }

}