/*
 * Made by Dolkar
 * Redistribution without permission is prohibited
 * 
 */ 
using System;
using UnityEngine;

namespace Inspector {

    [AttributeUsage(AttributeTargets.Field)]
    public abstract class InspectorAttribute : PropertyAttribute {
        public readonly String label;

        public String tooltip;
        public String useProperty;
        public String visibleCheck;
        public String enabledCheck;
        public int indentLevel;

        public InspectorAttribute()
            : this(null) {
        }

        public InspectorAttribute(String label) {
            this.label = label;
        }
    }

    public class FieldAttribute : InspectorAttribute {
        public bool allowSceneObjects = true;

        public FieldAttribute()
            : base(null) {
        }

        public FieldAttribute(String label)
            : base(label) {
        }
    }

    public class SliderAttribute : InspectorAttribute {
        public float minValue;
        public float maxValue;

        public SliderAttribute()
            : base(null) {
        }

        public SliderAttribute(String label)
            : base(label) {
        }

        public SliderAttribute(String label, float minValue, float maxValue)
            : base(label) {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
	}
	
//	public class IntSliderAttribute : InspectorAttribute {
//		public int minValue;
//		public int maxValue;
//		
//		public IntSliderAttribute()
//		: base(null) {
//		}
//		
//		public IntSliderAttribute(String label)
//		: base(label) {
//		}
//		
//		public IntSliderAttribute(String label, int minValue, int maxValue)
//		: base(label) {
//			this.minValue = minValue;
//			this.maxValue = maxValue;
//		}
//	}

    public class ToggleAttribute : InspectorAttribute {
        public bool flipped;

        public ToggleAttribute()
            : base(null) {
        }

        public ToggleAttribute(String label)
            : base(label) {
        }
    }

    public class GroupAttribute : InspectorAttribute {
        public bool drawFoldout = true;

        public GroupAttribute()
            : base(null) {
        }

        public GroupAttribute(String label)
            : base(label) {
        }
    }

    public class EnumMaskAttribute : InspectorAttribute {
        public EnumMaskAttribute()
            : base(null) {
        }

        public EnumMaskAttribute(String label)
            : base(label) {
        }
    }

    public class MinMaxSliderAttribute : InspectorAttribute {
        public float minValue;
        public float maxValue;
        public bool showFields = true;

        public MinMaxSliderAttribute()
            : base(null) {
        }

        public MinMaxSliderAttribute(String label)
            : base(label) {
        }

        public MinMaxSliderAttribute(String label, float minValue, float maxValue)
            : base(label) {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }

    namespace Decorations {

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        public abstract class DecorationAttribute : Attribute {
            public readonly int order;
            public String visibleCheck;

            public DecorationAttribute(int order) {
                this.order = order;
            }
        }

        public class SpaceAttribute : DecorationAttribute {
            public readonly int height;

            public SpaceAttribute(int order, int height)
                : base(order) {
                this.height = height;
            }
        }

        public class HeaderAttribute : DecorationAttribute {
            public readonly String header;

            public HeaderAttribute(int order, String header)
                : base(order) {
                this.header = header;
            }
        }

        public class LineSeparatorAttribute : DecorationAttribute {
            public float padding = 60.0f;

            public LineSeparatorAttribute(int order) : base(order) { }
        }

        public class ButtonAttribute : DecorationAttribute {
            public readonly String label;
            public readonly String callback;

            public float width = 200.0f;
            public float height = 28.0f;
            public String tooltip;

            public ButtonAttribute(int order, String label, String callback)
                : base(order) {
                this.label = label;
                this.callback = callback;
            }
        }

        public abstract class HelpBoxAttribute : DecorationAttribute {
            public readonly String message;

            public float width = 420.0f;

            public HelpBoxAttribute(int order, String message)
                : base(order) {
                this.message = message;
            }
        }

        public class InfoBoxAttribute : HelpBoxAttribute {
            public InfoBoxAttribute(int order, String message)
                : base(order, message) {
            }
        }

        public class WarningBoxAttribute : HelpBoxAttribute {
            public WarningBoxAttribute(int order, String message)
                : base(order, message) {
            }
        }

        public class ErrorBoxAttribute : HelpBoxAttribute {
            public ErrorBoxAttribute(int order, String message)
                : base(order, message) {
            }
        }
    }
}