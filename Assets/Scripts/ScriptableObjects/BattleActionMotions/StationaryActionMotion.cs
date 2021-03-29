using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "StationaryActionMotion.asset", menuName = "Battle/Action Motion/Stationary Action Motion")]
public class StationaryActionMotion : ActionMotionType
{
    public override IEnumerator PreActionMotion(BattleParticipant performer, BattleParticipant target)
    {
        yield return null;
    }
    
    public override IEnumerator PostActionMotion(BattleParticipant performer, BattleParticipant target)
    {
        yield return null;
    }
}
