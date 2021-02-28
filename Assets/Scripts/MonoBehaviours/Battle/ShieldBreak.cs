using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBreak : MonoBehaviour
{
    [SerializeField] Vector3 _offset = new Vector3(0f, 2.5f, 0f);
    Animation _animation;

    public void PlayAt(Vector3 position)
    {
        transform.position = position + _offset;
        _animation.Play();
        Debug.Log("playing shield break");
    }

    void OnArmourBreak(BattleParticipant battleParticipant)
    {
        PlayAt(battleParticipant.transform.position);
    }

    void Awake()
    {
        _animation = GetComponent<Animation>();
        BattleEvents.ArmourBreak += OnArmourBreak;
    }

    void OnDestroy()
    {
        BattleEvents.ArmourBreak -= OnArmourBreak;
    }
}
