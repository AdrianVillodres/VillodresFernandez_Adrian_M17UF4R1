using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyIA : MonoBehaviour, Inputs.IEnemyActions
{
    public int HP;
    private Inputs enemyInputs;
    public GameObject target;
    public bool attack;
    public bool chase;
    public StateSO currentNode;
    public List<StateSO> Nodes;
    public int LostHP;

    [Header("Patrol System")]
    public Transform mainPoint;        // Punto principal del patrón de patrulla
    public Transform[] patrolPoints;   // Puntos de patrulla secundarios
    public float patrolSpeed = 3f;     // Velocidad de patrulla
    public float chaseSpeed = 5f;      // Velocidad al perseguir
    public float visionRange = 10f;    // Rango de visión
    public float stopChaseTime = 3f;   // Tiempo antes de volver a patrullar
    private int currentPatrolIndex = 0;
    private bool lostPlayer = false;
    private Vector3 lastSeenPosition;
    private float lostPlayerTimer = 0;

    void Awake()
    {
        enemyInputs = new Inputs();
        enemyInputs.Enemy.SetCallbacks(this);
    }

    private void OnEnable()
    {
        enemyInputs.Enable();
    }

    private void OnDisable()
    {
        enemyInputs.Disable();
    }

    private void Update()
    {
        if (chase && target != null)
        {
            ChasePlayer();
        }
        else if (lostPlayer)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= stopChaseTime)
            {
                MovePatrolArea(lastSeenPosition);
                lostPlayer = false;
                lostPlayerTimer = 0;
            }
        }
        else
        {
            Patrol();
        }

        CheckEndingConditions();
        currentNode.OnStateUpdate(this);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
            chase = true;
            lostPlayer = false;
        }
        CheckEndingConditions();
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            chase = false;
            lastSeenPosition = target.transform.position;
            lostPlayer = true;
        }
        CheckEndingConditions();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            attack = true;
        CheckEndingConditions();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            attack = false;
        CheckEndingConditions();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPatrolIndex].position, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, chaseSpeed * Time.deltaTime);
        }
    }

    private void MovePatrolArea(Vector3 newPosition)
    {
        Vector3 offset = newPosition - mainPoint.position;
        mainPoint.position = newPosition;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPoints[i].position += offset;
        }
    }

    public void CheckEndingConditions()
    {
        foreach (ConditionSO condition in currentNode.EndConditions)
        {
            if (condition.CheckCondition(this) == condition.answer)
            {
                ExitCurrentNode();
            }
        }
    }

    public void ExitCurrentNode()
    {
        foreach (StateSO stateSO in Nodes)
        {
            if (stateSO.StartCondition == null)
            {
                EnterNewState(stateSO);
                break;
            }
            else if (stateSO.StartCondition.CheckCondition(this) == stateSO.StartCondition.answer)
            {
                EnterNewState(stateSO);
                break;
            }
        }
        currentNode.OnStateEnter(this);
    }

    private void EnterNewState(StateSO state)
    {
        currentNode.OnStateExit(this);
        currentNode = state;
        currentNode.OnStateEnter(this);
    }

    public void OnDealDamageToEnemy(InputAction.CallbackContext context)
    {
        if (context.performed)
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
