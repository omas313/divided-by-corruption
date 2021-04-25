using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbsorbDefinition.asset", menuName = "Battle/Action Definition/Absorb Definition")]
public class AbsorbDefinition : ActionDefinition
{
    public float AbsorbtionPercentage => _absorbtionPercentage;
    public List<SegmentData> SegmentsData => _segmentsData;

    [SerializeField] float _absorbtionPercentage = 0.3f;
    [SerializeField] AbsorbEffect _effectPrefab;
    [SerializeField] List<SegmentData> _segmentsData;

    public IEnumerator SpawnEffect(Vector3 sourcePosition, Vector3 destinationPosition)
    {
        var effect = Instantiate(_effectPrefab, sourcePosition, Quaternion.identity, GameObject.FindWithTag("Junk").transform);
        yield return effect.Play(sourcePosition, destinationPosition);
    }
}
