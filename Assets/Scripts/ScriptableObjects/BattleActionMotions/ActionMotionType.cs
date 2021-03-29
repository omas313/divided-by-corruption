using System.Collections;
using UnityEngine;

public abstract class ActionMotionType : ScriptableObject
{
    public abstract IEnumerator PreActionMotion(BattleParticipant performer, BattleParticipant target);
    public abstract IEnumerator PostActionMotion(BattleParticipant performer, BattleParticipant target);
}
