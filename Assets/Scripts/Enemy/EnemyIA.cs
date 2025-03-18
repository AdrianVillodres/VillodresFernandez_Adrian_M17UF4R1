using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    public Transform mainPoint;
    public Transform[] patrolPoints;
    public float stopChaseTime = 3f;
    private int currentPatrolIndex = 0;
    private bool lostPlayer = false;
    private Vector3 lastSeenPosition;
    private float lostPlayerTimer = 0;

    private ChaseBehaviour chaseBehaviour;
    private NavMeshAgent agent;

    void Awake()
    {
        enemyInputs = new Inputs();
        enemyInputs.Enemy.SetCallbacks(this);
        chaseBehaviour = GetComponent<ChaseBehaviour>();
        agent = GetComponent<NavMeshAgent>();
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
            chaseBehaviour.Chase(target.transform);
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
            else
            {
                chaseBehaviour.Chase(mainPoint);
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
            if (target != null)
            {
                lastSeenPosition = target.transform.position;
            }
            lostPlayer = true;
        }
        CheckEndingConditions();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
        }
        CheckEndingConditions();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = false;
        }
        CheckEndingConditions();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        chaseBehaviour.Chase(patrolPoints[currentPatrolIndex]);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void MovePatrolArea(Vector3 newPosition)
    {
        if (HP >= 5) return;

        // Calcular el desplazamiento entre la nueva posición y la posición actual del mainPoint
        Vector3 offset = newPosition - mainPoint.position;

        // Mover el mainPoint a la nueva posición
        mainPoint.position = newPosition;

        // Mover el primer punto de patrullaje (patrolPoints[0]) para que se sincronice con el mainPoint
        if (patrolPoints.Length > 0)
        {
            patrolPoints[0].position = mainPoint.position;
        }

        // Mover los otros puntos de patrullaje con el mismo offset
        for (int i = 1; i < patrolPoints.Length; i++)
        {
            // Desplazar el punto en X y Z
            Vector3 patrolPosition = patrolPoints[i].position + offset;

            // Ajustar la altura del punto para que no se hunda en el suelo (asegurarse de que está sobre el NavMesh)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(patrolPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                // Ajustar la altura a la posición del NavMesh
                patrolPoints[i].position = new Vector3(patrolPosition.x, hit.position.y, patrolPosition.z);
            }
            else
            {
                // Si no está en el NavMesh, moverlo a una posición válida sobre el NavMesh
                patrolPoints[i].position = new Vector3(patrolPosition.x, patrolPoints[i].position.y, patrolPosition.z);
            }
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
            if (stateSO.StartCondition == null ||
                stateSO.StartCondition.CheckCondition(this) == stateSO.StartCondition.answer)
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
