/*
 * Made by Dolkar
 * Redistribution without permission is prohibited
 * 
 */ 
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inspector.Decorations;
using UnityEditor;
using HeaderAttribute = Inspector.Decorations.HeaderAttribute;
using SpaceAttribute = Inspector.Decorations.SpaceAttribute;

namespace InspectorDrawers {
    public abstract class DecorationAttributeDrawer {
        private static Type[] derivedTypes;

        public DecorationAttribute attribute;
        protected FieldInfo fieldInfo;
        protected object[] parentObjects;

        protected object parentObject {
            get { return parentObjects[0]; }
        }

        static DecorationAttributeDrawer() {
            derivedTypes = Assembly.GetAssembly(typeof(DecorationAttributeDrawer)).GetTypes()
                .Where(myType => myType.IsClass &&
                                 !myType.IsAbstract &&
                                 myType.IsSubclassOf(typeof(DecorationAttributeDrawer)) &&
                                 myType.GetCustomAttributes(typeof(CustomDecorationDrawerAttribute), true).Length > 0)
                .ToArray();
        }

        public static DecorationAttributeDrawer[] GetDecorationAttributeDrawers(FieldInfo field, object[] parentObjects) {
            var attributes = (DecorationAttribute[])field.GetCustomAttributes(typeof(DecorationAttribute), true);

            var drawers = new List<DecorationAttributeDrawer>();
            for (int i = 0; i < attributes.Length; i++) {
                var attr = attributes[i];
                var drawer = FindDrawerForAttribute(attr);

                if (drawer != null) {
                    drawer.attribute = attributes[i];
                    drawer.fieldInfo = field;
                    drawer.parentObjects = parentObjects;

                    if (attr.visibleCheck == null || drawer.GetMemberBoolean(attr.visibleCheck)) {
                        drawers.Add(drawer);
                    }
                }
            }

            drawers.Sort((l, r) => l.attribute.order.CompareTo(r.attribute.order));

            return drawers.ToArray();
        }

        private static DecorationAttributeDrawer FindDrawerForAttribute(DecorationAttribute fieldAttribute) {
            foreach (Type drawerType in derivedTypes) {
                var drawerAttribute =
                    (CustomDecorationDrawerAttribute)
                        drawerType.GetCustomAttributes(typeof(CustomDecorationDrawerAttribute), true)[0];

                if (drawerAttribute.attributeType == fieldAttribute.GetType()) {
                    return (DecorationAttributeDrawer)Activator.CreateInstance(drawerType);
                }
            }

            return null;
        }

        protected bool GetMemberBoolean(String memberName) {
            return fieldInfo.DeclaringType.GetMemberBoolean(parentObject, memberName);
        }

        public virtual float GetControlHeight() {
            return EditorGUIUtility.singleLineHeight;
        }

        public abstract void DrawControl(Rect position);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomDecorationDrawerAttribute : Attribute {
        public readonly Type attributeType;

        public CustomDecorationDrawerAttribute(Type attributeType) {
            this.attributeType = attributeType;
        }
    }

    [CustomDecorationDrawer(typeof(SpaceAttribute))]
    public class DecorationSpaceDrawer : DecorationAttributeDrawer {
        public override float GetControlHeight() {
            var attr = (SpaceAttribute)attribute;
            return attr.height;
        }

        public override void DrawControl(Rect position) {
            return;
        }
    }

    [CustomDecorationDrawer(typeof(HeaderAttribute))]
    public class DecorationHeaderDrawer : DecorationAttributeDrawer {
        private static float spacing = 5.0f;

        public override float GetControlHeight() {
            return EditorGUIUtility.singleLineHeight + spacing;
        }

        public override void DrawControl(Rect position) {
            var attr = (HeaderAttribute)attribute;

            position.yMin += spacing;
            EditorGUI.LabelField(position, new GUIContent(attr.header), EditorStyles.boldLabel);
        }
    }

    [CustomDecorationDrawer(typeof(LineSeparatorAttribute))]
    public class DecorationLineSeparatorDrawer : DecorationAttributeDrawer {
        private static float spacing = 10.0f;

        public override float GetControlHeight() {
            return spacing * 2 + 1.0f;
        }

        public override void DrawControl(Rect position) {
            var attr = (LineSeparatorAttribute)attribute;

            GUIStyle line = new GUIStyle("box");
            line.border.top = line.border.bottom = 1;

            position.yMin += spacing;
            position.yMax -= spacing;
            position.xMin += attr.padding;
            position.xMax -= attr.padding + 10.0f;

            bool enabled = GUI.enabled;
            GUI.enabled = false; // Makes the line a bit brighter

            GUI.Box(position, GUIContent.none, line);

            GUI.enabled = enabled;
        }
    }

    [CustomDecorationDrawer(typeof(ButtonAttribute))]
    public class DecorationButtonDrawer : DecorationAttributeDrawer {
        private static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Static | BindingFlags.Instance |
                                                   BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod;
        private static float spacing = 5.0f;

        public override float GetControlHeight() {
            var attr = (ButtonAttribute)attribute;
            return attr.height + spacing * 2;
        }

        public override void DrawControl(Rect position) {
            var attr = (ButtonAttribute)attribute;

            position.yMin += spacing;
            position.yMax -= spacing;
            position.xMax -= 10.0f;

            float padding = (position.width - attr.width) / 2.0f;
            position.xMin += padding;
            position.xMax -= padding;

            if (GUI.Button(position, new GUIContent(attr.label, attr.tooltip))) {
                MethodInfo method = fieldInfo.DeclaringType.GetMethod(attr.callback, bindingFlags);
                if (method == null) {
                    throw new KeyNotFoundException(
                        String.Format("No method called '{0}' was found on object of type '{1}'.",
                        attr.callback, fieldInfo.DeclaringType));
                } else if (method.GetParameters().Length != 0) {
                    throw new InvalidCastException(String.Format(
                        "The requested method '{1}.{0}' needs to be parameterless.",
                        attr.callback, fieldInfo.DeclaringType));
                } else {
                    foreach (object parent in parentObjects) {
                        method.Invoke(parent, new object[] { });
                    }
                }
            }
        }
    }

    public abstract class DecorationHelpBoxDrawer : DecorationAttributeDrawer {
        public override float GetControlHeight() {
            var attr = (HelpBoxAttribute)attribute;
            float textHeight = GUI.skin.GetStyle("HelpBox").CalcHeight(new GUIContent(attr.message), attr.width);
            return Mathf.Max(textHeight, 38.0f);
        }

        public override void DrawControl(Rect position) {
            var attr = (HelpBoxAttribute)attribute;
            position.xMax -= 10.0f;
            float padding = (position.width - attr.width) / 2.0f;
            position.xMin += padding;
            position.xMax -= padding;

            EditorGUI.HelpBox(position, attr.message, HelpBoxType());
        }

        public abstract MessageType HelpBoxType();
    }

    [CustomDecorationDrawer(typeof(InfoBoxAttribute))]
    public class DecorationInfoBoxDrawer : DecorationHelpBoxDrawer {
        public override MessageType HelpBoxType() {
            return MessageType.Info;
        }
    }

    [CustomDecorationDrawer(typeof(WarningBoxAttribute))]
    public class DecorationWarningBoxDrawer : DecorationHelpBoxDrawer {
        public override MessageType HelpBoxType() {
            return MessageType.Warning;
        }
    }

    [CustomDecorationDrawer(typeof(ErrorBoxAttribute))]
    public class DecorationErrorBoxDrawer : DecorationHelpBoxDrawer {
        public override MessageType HelpBoxType() {
            return MessageType.Error;
        }
    }
}