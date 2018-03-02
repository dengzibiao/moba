using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using Inspector;
using Object = UnityEngine.Object;

namespace InspectorDrawers {
    public abstract class InspectorAttributeDrawer : PropertyDrawer {
        private static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Static | BindingFlags.Instance |
                                                   BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod;

        protected object[] parentObjects;

        protected object parentObject {
            get { return parentObjects[0]; }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            InspectorAttribute attr = (InspectorAttribute)attribute;
            SetupProperty(property);

            float height = 0;

            if (attr.visibleCheck == null || GetMemberBoolean(attr.visibleCheck)) {
                height = GetControlHeight(property, CustomizeLabel(label));
            } else {
                height -= EditorGUIUtility.standardVerticalSpacing;
            }

            foreach (var drawer in DecorationAttributeDrawer.GetDecorationAttributeDrawers(fieldInfo, parentObjects)) {
                height += drawer.GetControlHeight() + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        public override void OnGUI(Rect startPosition, SerializedProperty property, GUIContent label) {
            SetupProperty(property);
            Rect position = new Rect(startPosition);

            bool didDrawMainControl = false;
            foreach (var drawer in DecorationAttributeDrawer.GetDecorationAttributeDrawers(fieldInfo, parentObjects)) {
                if (!didDrawMainControl && drawer.attribute.order > attribute.order) {
                    position = OnMainGUI(position, property, label);
                    didDrawMainControl = true;
                }

                position.height = drawer.GetControlHeight();
                drawer.DrawControl(position);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!didDrawMainControl) {
                OnMainGUI(position, property, label);
            }

        }

        protected bool GetMemberBoolean(String memberName) {
            return fieldInfo.DeclaringType.GetMemberBoolean(parentObject, memberName);
        }

        private void SetupProperty(SerializedProperty property) {
            parentObjects = FindParentObjects(property.propertyPath, property.serializedObject);
        }

        private Rect OnMainGUI(Rect position, SerializedProperty property, GUIContent defaultLabel) {
            InspectorAttribute attr = (InspectorAttribute)attribute;
            if (attr.visibleCheck != null && !GetMemberBoolean(attr.visibleCheck)) {
                return position;
            }

            // Create value getters and setters to handle both field and property backed controls
            Func<object, object> valueGetter = fieldInfo.GetValue;
            Action<object, object> valueSetter = fieldInfo.SetValue;

            if (attr.useProperty != null) {
                PropertyInfo pInfo = fieldInfo.DeclaringType.GetProperty(attr.useProperty, bindingFlags);
                if (pInfo == null) {
                    throw new KeyNotFoundException(String.Format("Property '{0}' not found on object of type '{1}'.",
                        attr.useProperty, fieldInfo.DeclaringType));
                } else if (!fieldInfo.FieldType.IsAssignableFrom(pInfo.PropertyType)) {
                    throw new InvalidCastException(
                        String.Format("The requested property '{1}.{0}' needs to be castable to the source field.",
                            attr.useProperty, fieldInfo.DeclaringType));
                }

                if (pInfo.CanRead) {
                    valueGetter = (o) => pInfo.GetValue(o, null);
                }
                if (pInfo.CanWrite) {
                    valueSetter = (o, v) => pInfo.SetValue(o, v, null);
                }
            }

            EditorGUI.BeginProperty(position, defaultLabel, property);
            GUIContent label = CustomizeLabel(defaultLabel);

            object displayValue = valueGetter(parentObject);

            // Because we are bypassing properties, changes are not registered and we have to check for mixed values ourselves.
            EditorGUI.showMixedValue = false;
            for (int i = 1; i < parentObjects.Length; i++) {
                object value = valueGetter(parentObjects[i]);
                if (!System.Object.Equals(displayValue, value)) {
                    EditorGUI.showMixedValue = true;
                    break;
                }
            }

            // Do the actual drawing
            EditorGUI.BeginDisabledGroup(attr.enabledCheck != null && !GetMemberBoolean(attr.enabledCheck));
            EditorGUI.indentLevel += attr.indentLevel;

            position.height = GetControlHeight(property, label);
            object newValue = DrawControl(position, property, label, displayValue);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel -= attr.indentLevel;
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
            EditorGUI.showMixedValue = false;

            // React if the value changed
            if (!System.Object.Equals(displayValue, newValue)) {
                foreach (Object target in property.serializedObject.targetObjects) {
                    Undo.RecordObject(target, label.text);
                    EditorUtility.SetDirty(target);
                }

                foreach (object parent in parentObjects) {
                    valueSetter(parent, newValue);
                }
            }

            return position;
        }

        private object[] FindParentObjects(string path, SerializedObject sObject) {
            string[] parts = path.Split('.');
            int targets = sObject.targetObjects.Length;

            object[] parents = new object[targets];
            for (int i = 0; i < targets; i++) {
                parents[i] = sObject.targetObjects[i];
            }

            for (int i = 0; i < parts.Length - 1; i++) {
                string fieldName = parts[i];

                Type parentType = parents[0].GetType();
                FieldInfo field = parentType.GetField(fieldName);

                if (parts[i + 1] == "Array") {
                    // This field is an array / list

                    if (i + 3 < parts.Length) {
                        // There's still more stuff behind it
                        for (int j = 0; j < targets; j++) {
                            string elName = parts[i + 2];
                            int openingPos = elName.IndexOf("[", StringComparison.Ordinal);
                            int endingPos = elName.IndexOf("]", StringComparison.Ordinal);
                            int index = Convert.ToInt32(elName.Substring(openingPos + 1, endingPos - openingPos - 1));

                            IList array = (IList)field.GetValue(parents[j]);
                            parents[j] = array[index];
                        }
                        i += 2;
                    } else {
                        // The child is an element of the array... stop and return the parents
                        return parents;
                    }
                } else {
                    for (int j = 0; j < targets; j++) {
                        parents[j] = field.GetValue(parents[j]);
                    }
                }
            }

            return parents;
        }

        private GUIContent CustomizeLabel(GUIContent defaultLabel) {
            InspectorAttribute attr = (InspectorAttribute)attribute;

            GUIContent label = new GUIContent(defaultLabel);
            if (attr.label != null) {
                label.text = attr.label;
            }
            if (attr.tooltip != null) {
                label.tooltip = attr.tooltip;
            }

            return label;
        }

        public virtual float GetControlHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public abstract object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value);
    }

    public static class ReflectionExtensions {
        private static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Static | BindingFlags.Instance |
                                                   BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod;

        private static MemberTypes validValueTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;

        private struct TypeMemberPair {
            public Type classType;
            public string memberName;

            public TypeMemberPair(Type classType, string memberName) {
                this.classType = classType;
                this.memberName = memberName;
            }
        }
        private static Dictionary<TypeMemberPair, MemberInfo> memberInfoCache = new Dictionary<TypeMemberPair, MemberInfo>();

        public static bool GetMemberBoolean(this Type classType, object parentObject, string memberName) {
            MemberInfo member;

            TypeMemberPair cacheKey = new TypeMemberPair(classType, memberName);
            if (!memberInfoCache.TryGetValue(cacheKey, out member)) {
                // This member has not been searched for yet
                MemberInfo[] members = classType.GetMember(memberName, validValueTypes, bindingFlags);
                if (members.Length == 0) {
                    throw new KeyNotFoundException(
                        String.Format("No field, property or method called '{0}' was found on object of type '{1}'.",
                            memberName, classType));
                } else if (members.Length > 1) {
                    throw new AmbiguousMatchException(
                        String.Format("There are multiple members called '{0}' on object of type '{1}'.",
                            memberName, classType));
                }

                member = members[0];
                // Let's cache the damn thing
                memberInfoCache.Add(cacheKey, member);
            }

            if (member.MemberType == MemberTypes.Field) {
                FieldInfo field = (FieldInfo)member;
                if (typeof(bool).IsAssignableFrom(field.FieldType)) {
                    return (bool)field.GetValue(parentObject);
                } else {
                    throw new InvalidCastException(String.Format("The requested field '{1}.{0}' needs to be a boolean.",
                        memberName, classType));
                }

            } else if (member.MemberType == MemberTypes.Method) {
                MethodInfo method = (MethodInfo)member;
                if (typeof(bool).IsAssignableFrom(method.ReturnType) && method.GetParameters().Length == 0) {
                    return (bool)method.Invoke(parentObject, null);
                } else {
                    throw new InvalidCastException(String.Format(
                        "The requested method '{1}.{0}' needs to have a bool return type and no parameters.",
                        memberName, classType));
                }
            } else {
                PropertyInfo property = (PropertyInfo)member;
                if (typeof(bool).IsAssignableFrom(property.PropertyType) && property.CanRead) {
                    return (bool)property.GetValue(parentObject, null);
                } else {
                    throw new InvalidCastException(String.Format(
                        "The requested property '{1}.{0}' needs to be a boolean and have a get method defined.",
                        memberName, classType));
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(FieldAttribute))]
    public class InspectorFieldDrawer : InspectorAttributeDrawer {
        private static GUIWrapper[] guiWrappers = {
            new GUIWrapper<float>(EditorGUI.FloatField),
            new GUIWrapper<double>(EditorGUI.DoubleField),
            new GUIWrapper<int>(EditorGUI.IntField),
            new GUIWrapper<long>(EditorGUI.LongField),
            new GUIWrapper<string>(EditorGUI.TextField),
            new GUIWrapper<Enum>(EditorGUI.EnumPopup),
            new GUIWrapper<Bounds>(EditorGUI.BoundsField),
            new GUIWrapper<Rect>(EditorGUI.RectField),
            new GUIWrapper<Color>(EditorGUI.ColorField),
            new GUIWrapper<AnimationCurve>(EditorGUI.CurveField),
            new GUIWrapper<Vector2>(EditorGUI.Vector2Field),
            new GUIWrapper<Vector3>(EditorGUI.Vector3Field),
            new GUIWrapper<Vector4>(DrawVector4Field),
        };

        public override float GetControlHeight(SerializedProperty property, GUIContent label) {
            float fieldHeight = EditorGUIUtility.singleLineHeight;

            if (typeof(Bounds).IsAssignableFrom(fieldInfo.FieldType)) {
                fieldHeight = EditorGUIUtility.singleLineHeight * 3;
            } else if (typeof(Rect).IsAssignableFrom(fieldInfo.FieldType)) {
                fieldHeight = EditorGUIUtility.singleLineHeight * 2;
            }

            if (EditorGUIUtility.currentViewWidth < 333 &&
                (typeof(Rect).IsAssignableFrom(fieldInfo.FieldType) ||
                typeof(Vector2).IsAssignableFrom(fieldInfo.FieldType) ||
                typeof(Vector3).IsAssignableFrom(fieldInfo.FieldType))) {
                fieldHeight += EditorGUIUtility.singleLineHeight;
            }

            return fieldHeight;
        }

        public override object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value) {
            foreach (GUIWrapper wrapper in guiWrappers) {
                if (wrapper.IsTypeCompatible(fieldInfo.FieldType)) {
                    return wrapper.Invoke(position, label, value);
                }
            }

            // Arbitrary unity object...
            if (typeof(Object).IsAssignableFrom(fieldInfo.FieldType)) {
                bool allowSceneObjects = ((FieldAttribute)attribute).allowSceneObjects;
                return EditorGUI.ObjectField(position, label, (Object)value, fieldInfo.FieldType,
                    allowSceneObjects);
            }

            throw new InvalidCastException(String.Format(
                "Field '{0}' of type '{1}' cannot be cast to any type supported by the Inspector.Field attribute.",
                fieldInfo.Name, fieldInfo.FieldType));
        }

        private static Vector4 DrawVector4Field(Rect position, GUIContent label, Vector4 value) {
            // Unity renders Vector4s in a different manner from Vector3 and Vector2... lets fix that!
            Rect controlPosition = EditorGUI.PrefixLabel(position, label);
            controlPosition.y -= controlPosition.height;
            return EditorGUI.Vector4Field(controlPosition, GUIContent.none, value);
        }

        // Comfort objects
        private abstract class GUIWrapper {
            public abstract bool IsTypeCompatible(Type otherType);
            public abstract object Invoke(Rect position, GUIContent label, object value);
        }

        private class GUIWrapper<T> : GUIWrapper {
            private Func<Rect, GUIContent, T, T> guiCall;

            public GUIWrapper(Func<Rect, GUIContent, T, T> guiCall) {
                this.guiCall = guiCall;
            }

            public override bool IsTypeCompatible(Type otherType) {
                return typeof(T).IsAssignableFrom(otherType);
            }

            public override object Invoke(Rect position, GUIContent label, object value) {
                return guiCall.Invoke(position, label, (T)value);
            }
        }
    }

    [CustomPropertyDrawer(typeof(SliderAttribute))]
    public class InspectorSliderDrawer : InspectorAttributeDrawer {
        public override object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value) {
            var attr = (SliderAttribute)attribute;

            if (typeof(float).IsAssignableFrom(fieldInfo.FieldType)) {
                return EditorGUI.Slider(position, label, (float)value, attr.minValue, attr.maxValue);
            } else if (typeof(int).IsAssignableFrom(fieldInfo.FieldType)) {
                return EditorGUI.IntSlider(position, label, (int)value, (int)attr.minValue, (int)attr.maxValue);
            } else {
                throw new InvalidCastException(String.Format(
                    "Field '{0}' of type '{1}' cannot be cast to any type supported by the Inspector.Slider attribute.",
                    fieldInfo.Name, fieldInfo.FieldType));
            }
        }
    }

    [CustomPropertyDrawer(typeof(ToggleAttribute))]
    public class InspectorToggleDrawer : InspectorAttributeDrawer {
        public override object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value) {
            var attr = (ToggleAttribute)attribute;

            if (typeof(bool).IsAssignableFrom(fieldInfo.FieldType)) {
                if (attr.flipped) {
                    return EditorGUI.ToggleLeft(position, label, (bool)value);
                } else {
                    return EditorGUI.Toggle(position, label, (bool)value);
                }
            } else {
                throw new InvalidCastException(String.Format(
                    "Field '{0}' of type '{1}' cannot be cast to any type supported by the Inspector.Toggle attribute.",
                    fieldInfo.Name, fieldInfo.FieldType));
            }
        }
    }

    [CustomPropertyDrawer(typeof(GroupAttribute))]
    public class InspectorGroupDrawer : InspectorAttributeDrawer {
        public override float GetControlHeight(SerializedProperty property, GUIContent label) {
            var attr = (GroupAttribute)attribute;

            if (attr.drawFoldout) {
                return EditorGUI.GetPropertyHeight(property);
            } else {
                property.isExpanded = true;
                float expanded = EditorGUI.GetPropertyHeight(property);
                property.isExpanded = false;
                float parentOnly = EditorGUI.GetPropertyHeight(property);
                return expanded - parentOnly;
            }
        }

        public override object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value) {
            var attr = (GroupAttribute)attribute;

            if (fieldInfo.FieldType.IsClass &&
                !typeof(Object).IsAssignableFrom(fieldInfo.FieldType) &&
                !typeof(IList).IsAssignableFrom(fieldInfo.FieldType)) {

                if (attr.drawFoldout) {
                    EditorGUI.PropertyField(position, property, label, true);
                } else {
                    property.isExpanded = true;

                    SerializedProperty endProperty = property.GetEndProperty();
                    property.NextVisible(true);

                    while (!SerializedProperty.EqualContents(property, endProperty)) {
                        position.height = EditorGUI.GetPropertyHeight(property);
                        EditorGUI.PropertyField(position, property, true);
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        property.NextVisible(false);
                    }
                }

                return value;
            } else {
                throw new InvalidCastException(String.Format(
                    "Field '{0}' of type '{1}' must be a class not derived from UnityEngine.Object to be used with the Inspector.Group attribute.",
                    fieldInfo.Name, fieldInfo.FieldType));
            }
        }
    }
    
    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public class InspectorEnumMaskDrawer : InspectorAttributeDrawer {
        public override object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value) {
            if (typeof(Enum).IsAssignableFrom(fieldInfo.FieldType)) {
                return EditorGUI.EnumMaskField(position, label, (Enum)value);
            } else {
                throw new InvalidCastException(String.Format(
                    "Field '{0}' of type '{1}' cannot be cast to any type supported by the Inspector.EnumMask attribute.",
                    fieldInfo.Name, fieldInfo.FieldType));
            }

        }
    }

    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class InspectorMinMaxSliderDrawer : InspectorAttributeDrawer {
        private static float horizontalSpacing = 5.0f;
        private static GUIContent minLabel = new GUIContent("Min:");
        private static GUIContent maxLabel = new GUIContent("Max:");

        public override float GetControlHeight(SerializedProperty property, GUIContent label) {
            var attr = (MinMaxSliderAttribute)attribute;

            if (attr.showFields) {
                return EditorGUIUtility.singleLineHeight * 2;
            } else {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override object DrawControl(Rect position, SerializedProperty property, GUIContent label, object value) {
            var attr = (MinMaxSliderAttribute)attribute;

            if (typeof(Vector2).IsAssignableFrom(fieldInfo.FieldType)) {
                Vector2 v = (Vector2) value;
                float minValue = v.x;
                float maxValue = v.y;
                if (attr.showFields) position.height /= 2.0f;

                EditorGUI.MinMaxSlider(label, position, ref minValue, ref maxValue, attr.minValue, attr.maxValue);

                if (attr.showFields) {
                    // Draw control values
                    int prevIndentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;

                    position.y += EditorGUIUtility.singleLineHeight;
                    position.xMin += EditorGUIUtility.labelWidth;

                    float minLabelWidth = GUI.skin.label.CalcSize(minLabel).x;
                    float maxLabelWidth = GUI.skin.label.CalcSize(maxLabel).x;
                    float fieldWidth = (position.width - (minLabelWidth + maxLabelWidth + 3 * horizontalSpacing)) / 2.0f;
                    position.width = fieldWidth;

                    EditorGUI.LabelField(position, minLabel);
                    position.x += minLabelWidth + horizontalSpacing;
                    minValue = EditorGUI.FloatField(position, minValue);
                    position.x += fieldWidth + horizontalSpacing;

                    EditorGUI.LabelField(position, maxLabel);
                    position.x += maxLabelWidth + horizontalSpacing;
                    maxValue = EditorGUI.FloatField(position, maxValue);

                    EditorGUI.indentLevel = prevIndentLevel;
                }

                return new Vector2(minValue, maxValue);
            } else {
                throw new InvalidCastException(String.Format(
                    "Field '{0}' of type '{1}' cannot be cast to any type supported by the Inspector.MinMaxSlider attribute.",
                    fieldInfo.Name, fieldInfo.FieldType));
            }
        }
    }
}