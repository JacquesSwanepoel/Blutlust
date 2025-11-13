using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettingSaveObject : ScriptableObject
{
    [TextArea(20, 25)]
    public string savedBindings = string.Empty;
}
