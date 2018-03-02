using UnityEngine;
using System.Collections;

public class HpFieldMap : MonoBehaviour {

    public static HpFieldMap instance;

    void Awake()
    {
        instance = this;
    }

    public HPBar FieldPlayerHpBar;
}
