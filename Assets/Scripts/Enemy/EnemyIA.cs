using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyIA : MonoBehaviour, Inputs.IEnemyActions
{
    private Inputs enemyInputs;
    public int HP;

    // Start is called before the first frame update
    void Awake()
    {
        enemyInputs = new Inputs();
        enemyInputs.Enemy.SetCallbacks(this);
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

    public void OnDealDamageToEnemy(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            HP--;
            Debug.Log("Deal Damage to Enemy");
        }
        
    }
}
