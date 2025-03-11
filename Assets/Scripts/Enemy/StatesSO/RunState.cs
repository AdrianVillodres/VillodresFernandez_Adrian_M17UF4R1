using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RunState", menuName = "StatesSO/Run")]
public class RunState : StatesSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
        Debug.Log("Running");
    }

    public override void OnStateExit(EnemyIA ec)
    {
        ec.GetComponent<ChaseBehaviour>().StopChasing();
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        Debug.Log("Running");
        ec.GetComponent<ChaseBehaviour>().Run(ec.target.transform, ec.transform);
    }
}
