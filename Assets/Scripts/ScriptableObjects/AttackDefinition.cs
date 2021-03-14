using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Attack Definition")]
public class AttackDefinition : ScriptableObject
{
    public string Name => _name;
    public int Damage => _damage;
    public int SegmentsCount => _segmentData.Count;
    public List<SegmentData> SegmentData => _segmentData;
    public AttackTargetType AttackTargetType => _attackTargetType;
    public AttackMotionType AttackMotionType => _attackMotionType;
    public GameObject OnHitEffectsPrefab => _onHitEffectsPrefab;
    public GameObject CastEffectsPrefab => _castEffectsPrefab;

    [SerializeField] string _name;
    [SerializeField] int _damage;
    [SerializeField] List<SegmentData> _segmentData;
    [SerializeField] AttackTargetType _attackTargetType;
    [SerializeField] AttackMotionType _attackMotionType;
    [SerializeField] GameObject _onHitEffectsPrefab;
    [SerializeField] GameObject _castEffectsPrefab;
}
