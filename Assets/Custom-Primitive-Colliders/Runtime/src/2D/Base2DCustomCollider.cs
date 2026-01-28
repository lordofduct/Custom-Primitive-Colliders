using UnityEngine;
using System.Collections.Generic;

namespace CustomPrimitiveColliders
{

    public abstract class Base2DCustomCollider : BaseCustomCollider
    {

        #region Fields

        private PolygonCollider2D m_polygonCollider2d;

        #endregion

        #region Properties

        protected PolygonCollider2D polygonCollider2d
        {
            get
            {
                if (m_polygonCollider2d == null)
                {
                    m_polygonCollider2d = GetComponent<PolygonCollider2D>();

                    if (m_polygonCollider2d == null)
                    {
                        m_polygonCollider2d = gameObject.AddComponent<PolygonCollider2D>();
                    }
                }
                return m_polygonCollider2d;
            }
        }

        #endregion

    }

}
