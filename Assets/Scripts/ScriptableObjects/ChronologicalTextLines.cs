using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChronologicalTextLines.asset", menuName = "Chronological Text Lines")]
public class ChronologicalTextLines : ScriptableObject
{
    public string[] TextLines => _textLines;

    [SerializeField] [TextArea(3, 5)] string[] _textLines;
}
