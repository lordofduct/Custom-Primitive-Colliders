using UnityEngine;
using System.Collections.Generic;

namespace CustomPrimitiveColliders
{

    public abstract class Base3DCustomCollider : BaseCustomCollider
    {

        #region Fields

        [System.NonSerialized]
        private MeshCollider m_meshCollider;
        [System.NonSerialized]
        private Mesh m_mesh;

        #endregion

        #region CONSTRUCTOR

        protected virtual void OnDestroy()
        {
            if (m_mesh)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(m_mesh);
                }
                else
                {
                    DestroyImmediate(m_mesh);
                }
#else
                Destroy(m_mesh);
#endif
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected void GetMeshCollider(out MeshCollider meshCollider, out Mesh mesh)
        {
            if (!m_meshCollider)
            {
                m_meshCollider = GetComponent<MeshCollider>();
                if (!m_meshCollider)
                {
                    m_meshCollider = gameObject.AddComponent<MeshCollider>();
                }
            }

            if (!m_mesh)
            {
                m_mesh = new Mesh();
                m_meshCollider.sharedMesh = m_mesh;
            }

            meshCollider = m_meshCollider;
            mesh = m_mesh;
        }

        #endregion

    }

}