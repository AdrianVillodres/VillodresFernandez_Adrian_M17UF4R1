using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class MainCharacter : MonoBehaviour, Inputs.IPlayerActions
{
    private Inputs playerInputs;
    public int HP = 5;
    public Vector3 ipMove;
    private Rigidbody rb;
    private MainCharacter character;
    public Slider Healthbar;
    public int speed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = new Inputs();
        playerInputs.Player.SetCallbacks(this);
        character = GetComponent<MainCharacter>();
        Healthbar.maxValue = HP;
        Healthbar.value = HP;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + speed * Time.deltaTime * ipMove.normalized);
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ipMove = context.ReadValue<Vector3>();
        }
        else if (context.canceled)
        {
            ipMove = Vector3.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP < 0) HP = 0;
        Healthbar.value = HP;
    }
}
