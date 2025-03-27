using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DieState", menuName = "StatesSO/Die")]
public class DieState : StateSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
    }

    public override void OnStateExit(EnemyIA ec)
    {
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        Destroy(ec.gameObject);
        Debug.Log("Abandon� este mundo de miseria y desesperaci�n");
    }
}
