using UnityEngine;
using System.Collections;

public class GUISingleInput : GUIComponentBase
{

    #region Init
    public delegate void VoidInputChange(string value);
    /// <summary>
    /// (string value)
    /// </summary>
    public VoidInputChange onChange;

    private UIInput input;
    protected override void Init()
    {
        input = this.GetComponent<UIInput>();
        input.onChange.Add(new EventDelegate(OnChange));
        AddBoxCollider();
    }
    #endregion

    public string text
    {
        set { input.value = value; }
        get { return input.value; }
    }

    private void OnChange()
    {
        if(onChange != null) onChange(text);
    }
}
