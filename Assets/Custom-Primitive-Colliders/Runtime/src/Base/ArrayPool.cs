using UnityEngine;
using System.Collections.Generic;

namespace CustomPrimitiveColliders
{

    internal class ArrayPool
    {

        static readonly object LOCK = new object();
        const int MIN_ARR_LEN = 1024;
        const int MAX_ARR_LEN = 8192;
        static Vector3[] REUSE_VERTS = new Vector3[MIN_ARR_LEN];
        static Vector3[] REUSE_NORMS = new Vector3[MIN_ARR_LEN];
        static Vector2[] REUSE_UVS = new Vector2[MIN_ARR_LEN];
        static int[] REUSE_TRIS = new int[MIN_ARR_LEN];

        public static void GetReusableMeshArrays(int vertcount, int tricount, out Vector3[] verts, out Vector3[] norms, out Vector2[] uvs, out int[] tris)
        {
            lock (LOCK)
            {
                if (REUSE_VERTS?.Length >= vertcount)
                {
                    verts = REUSE_VERTS;
                    REUSE_VERTS = null;
                }
                else
                {
                    verts = new Vector3[Mathf.Max(vertcount, MIN_ARR_LEN)];
                }
                if (REUSE_NORMS?.Length >= vertcount)
                {
                    norms = REUSE_NORMS;
                    REUSE_NORMS = null;
                }
                else
                {
                    norms = new Vector3[Mathf.Max(vertcount, MIN_ARR_LEN)];
                }
                if (REUSE_UVS?.Length >= vertcount)
                {
                    uvs = REUSE_UVS;
                    REUSE_UVS = null;
                }
                else
                {
                    uvs = new Vector2[Mathf.Max(vertcount, MIN_ARR_LEN)];
                }
                if (REUSE_TRIS?.Length >= tricount)
                {
                    tris = REUSE_TRIS;
                    REUSE_TRIS = null;
                }
                else
                {
                    tris = new int[tricount];
                }
            }
        }

        public static void ReleaseMeshArrays(Vector3[] verts, Vector3[] norms, Vector2[] uvs, int[] tris)
        {
            lock (LOCK)
            {
                if (verts?.Length <= MAX_ARR_LEN &&
                    (REUSE_VERTS == null || REUSE_VERTS.Length < verts.Length))
                {
                    REUSE_VERTS = verts;
                }
                if (norms?.Length <= MAX_ARR_LEN &&
                    (REUSE_NORMS == null || REUSE_NORMS.Length < norms.Length))
                {
                    REUSE_NORMS = norms;
                }
                if (uvs?.Length <= MAX_ARR_LEN &&
                    (REUSE_UVS == null || REUSE_UVS.Length < uvs.Length))
                {
                    REUSE_UVS = uvs;
                }
                if (tris?.Length <= MAX_ARR_LEN &&
                    (REUSE_TRIS == null || REUSE_TRIS.Length < tris.Length))
                {
                    REUSE_TRIS = tris;
                }
            }
        }

    }

}
