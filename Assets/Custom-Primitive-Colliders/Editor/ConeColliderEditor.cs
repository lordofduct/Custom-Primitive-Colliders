using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using CustomPrimitiveColliders;

namespace CustomPrimitiveCollidersEditor
{

    [CustomEditor(typeof(ConeCollider))]
    public class ConeColliderEditor : Editor
    {

        const string PROP_SCRIPT = "m_Script";
        const string PROP_USEOPENANGLE = "m_useOpenAngle";
        const string PROP_OPENANGLE = "m_openAngle";
        const string PROP_RADIUS = "m_radius";

        public override void OnInspectorGUI()
        {
            this.serializedObject.UpdateIfRequiredOrScript();

            bool useOpenAngle = this.serializedObject.FindProperty(PROP_USEOPENANGLE).boolValue;

            SerializedProperty iterator = this.serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope(iterator.propertyPath == PROP_SCRIPT))
                {
                    switch (iterator.propertyPath)
                    {
                        case PROP_OPENANGLE:
                            if (useOpenAngle)
                            {
                                EditorGUILayout.PropertyField(iterator, true);
                            }
                            break;
                        case PROP_RADIUS:
                            if (!useOpenAngle)
                            {
                                EditorGUILayout.PropertyField(iterator, true);
                            }
                            break;
                        default:
                            EditorGUILayout.PropertyField(iterator, true);
                            break;
                    }

                }
                enterChildren = false;
            }

            this.serializedObject.ApplyModifiedProperties();
        }

    }

}
