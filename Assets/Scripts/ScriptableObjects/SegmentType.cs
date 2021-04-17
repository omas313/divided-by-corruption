using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SegmentType.asset", menuName = "Segment Type")]
public class SegmentType : ScriptableObject
{
    public UIAttackBarSegment Prefab => _prefab;

    [SerializeField] UIAttackBarSegment _prefab;
}
