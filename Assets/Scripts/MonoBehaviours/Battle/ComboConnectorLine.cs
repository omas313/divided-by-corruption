using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboConnectorLine : MonoBehaviour
{
    public bool IsActive => _line.enabled;
    public PartyMember[] PartyMembers { get; private set; } = new PartyMember[2];

    [SerializeField] LineRenderer _line;

    public void Hide() => _line.enabled = false;
    public void Show() => _line.enabled = true;

    public void SetBetweenPartyMembers(PartyMember member1, PartyMember member2)
    {
        PartyMembers[0] = member1;
        PartyMembers[1] = member2;

        // var randomOffset = new Vector3(
        //     UnityEngine.Random.Range(-0.1f, 0.1f),
        //     UnityEngine.Random.Range(-0.1f, 0.1f),
        //     0f);
        // var midPoint = member1.CurrentPosition - member2.CurrentPosition + randomOffset;
        
        _line.SetPosition(0, member1.CurrentPosition);
        // _line.SetPosition(1, midPoint);
        _line.SetPosition(1, member2.CurrentPosition);
        // _line.SetPosition(2, member2.CurrentPosition);

        Show();
    }
}
