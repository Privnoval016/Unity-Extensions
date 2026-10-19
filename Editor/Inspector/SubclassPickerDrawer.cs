#if !ODIN_INSPECTOR
using System;
using Extensions.Inspector;
using UnityEditor;
using UnityEngine;

namespace Extensions.EditorTools
{
    /** <summary>Draws a <see cref="SubclassPickerAttribute" /> field: a type menu on the label row, then the chosen type's fields.</summary> */
    [CustomPropertyDrawer(typeof(SubclassPickerAttribute))]
    public sealed class SubclassPickerDrawer : PropertyDrawer
    {
        #region Public API

        /** <inheritdoc /> */
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);

        /** <inheritdoc /> */
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            var row = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var current = SubclassTypes.FromManagedReferenceName(property.managedReferenceFullTypename);
            if (EditorGUI.DropdownButton(row, new GUIContent(SubclassTypes.Label(current)), FocusType.Keyboard)) ShowMenu(property, current);
            EditorGUI.PropertyField(position, property, label, true);
        }

        #endregion

        #region Helpers

        private static void ShowMenu(SerializedProperty property, Type current)
        {
            var menu = new GenericMenu();
            var target = property.Copy();
            menu.AddItem(new GUIContent(SubclassTypes.NoneLabel), current == null, () => Assign(target, null));
            foreach (var type in SubclassTypes.For(SubclassTypes.FromManagedReferenceName(property.managedReferenceFieldTypename)))
            {
                var chosen = type;
                menu.AddItem(new GUIContent(SubclassTypes.Label(type)), type == current, () => Assign(target, chosen));
            }
            menu.ShowAsContext();
        }

        private static void Assign(SerializedProperty property, Type type)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
            property.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
#endif
