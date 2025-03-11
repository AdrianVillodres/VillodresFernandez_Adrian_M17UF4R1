using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyIA : MonoBehaviour, Inputs.IEnemyActions
{
    private Inputs enemyInputs;
    public NavMeshAgent agent;
    public GameObject target;
    public int HP;
    public int LostHP;
    public bool chase;
    public bool attack;
    public StatesSO currentNode;
    public List<StatesSO> nodes;

    // Start is called before the first frame update
    void Awake()
    {
        enemyInputs = new Inputs();
        enemyInputs.Enemy.SetCallbacks(this);
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        chase = true;
        target = collision.gameObject;
        CheckEndingConditions();
    }
    private void OnTriggerExit(Collider collision)
    {
        chase = false;
        CheckEndingConditions();
    }
    private void OnCollisionEnter(Collision collision)
    {
        attack = true;
        CheckEndingConditions();
    }
    private void OnCollisionExit(Collision collision)
    {
        attack = false;
        CheckEndingConditions();
    }
    private void Update()
    {
        CheckEndingConditions();
        currentNode.OnStateUpdate(this);
    }
    public void CheckEndingConditions()
    {
        foreach (ConditionSO condition in currentNode.EndConditions)
            if (condition.CheckCondition(this) == condition.answer) ExitCurrentNode();
    }
    public void ExitCurrentNode()
    {
        foreach (StatesSO statesSO in nodes)
        {
            if (statesSO.StartCondition == null)
            {
                EnterNewState(statesSO);
                break;
            }
            else
            {
                if (statesSO.StartCondition.CheckCondition(this) == statesSO.StartCondition.answer)
                {
                    EnterNewState(statesSO);
                    break;
                }
            }
        }
        currentNode.OnStateEnter(this);
    }
    private void EnterNewState(StatesSO state)
    {
        currentNode.OnStateExit(this);
        currentNode = state;
        currentNode.OnStateEnter(this);
    }
    public void OnDealDamageToEnemy(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            HP--;
            if (HP <= 0)
            {
                LostHP = 0;
                ExitCurrentNode();
            }
            CheckEndingConditions();
            Debug.Log("Deal Damage to Enemy");
        }
        
    }
}
