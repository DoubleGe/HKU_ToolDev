using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Shared Bool", menuName = "Custom/Variables/Custom Bool")]
public class SharedBoolScriptable : ScriptableObject
{
    public bool value;
}
