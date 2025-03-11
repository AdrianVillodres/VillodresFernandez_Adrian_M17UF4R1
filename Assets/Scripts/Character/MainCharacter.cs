using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class MainCharacter : MonoBehaviour, Inputs.IPlayerActions
{
    private Inputs playerInputs;
    public int HP;
    public Vector3 ipMove;
    private Rigidbody rb;
    private MainCharacter character;
    [SerializeField]
    private int speed = 1;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = new Inputs();
        playerInputs.Player.SetCallbacks(this);
        character = GetComponent<MainCharacter>();
    }

    // Update is called once per frame
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
        ipMove = context.ReadValue<Vector3>();
    }
}
