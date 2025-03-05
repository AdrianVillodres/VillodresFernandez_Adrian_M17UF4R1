using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyIA : MonoBehaviour, Inputs.IEnemyActions
{
    private Inputs enemyInputs;
    private Rigidbody rb;
    public NavMeshAgent agent;
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
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(HP <= 0)
        {
            Destroy(gameObject);
        } 
    }
    private void OnEnable()
    {
        enemyInputs.Enable();
    }

    private void OnDisable()
    {
        enemyInputs.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        agent.SetDestination(GameObject.Find("Player").transform.position);
        chase = true;
    }

    private void OnTriggerExit(Collider other)
    {
        chase = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        attack = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        attack = false;
    }
    public void OnDealDamageToEnemy(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            HP--;
            Debug.Log("Deal Damage to Enemy");
        }
        
    }
}
