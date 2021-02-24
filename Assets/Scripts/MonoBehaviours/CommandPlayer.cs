using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPlayer : MonoBehaviour
{
    public IEnumerator Play(List<BattleCommand> battleCommands)
    {
        Debug.Log($"commands player playing {battleCommands.Count} commands");
        yield return new WaitForSeconds(0.5f);


        foreach (var command in battleCommands)
        {
            if (command.Actor.IsDead || command.Target.IsDead)
                continue;

            yield return command.Execute();
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }
}
