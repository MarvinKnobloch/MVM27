using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private Player player;
    private Controls controls;

    [SerializeField] private int maxComboLength;
    [SerializeField] private AttackValues[] attacks;
    [SerializeField] private LayerMask enemyLayer;

    private float attackTimer;
    private float currentAttackTime;
    private float currentBufferTime;
    private bool readInput;

    private int currentAttackNumber;
    private int nextAttackNumber;
    private bool chainAttack;

    private int elementalSwitchNumber;

    //Animationen
    public enum PlayerAnimations
    {
        PlayerAttack1,
        PlayerAttack2,
        PlayerAttack3,

    }
    private void Awake()
    {
        player = GetComponent<Player>();
        controls = Keybindinputmanager.Controls;
    }

    public States state;

    public enum States
    {
        Empty,
        Attack,
    }
    private void Update()
    {
        switch(state)
        {
            case States.Empty:
                break;
            case States.Attack:
                //Attack();
                InputBuffer();
                break;
        }
    }
    public void AttackInput(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            switch (player.state)
            {
                case Player.States.Ground:
                    currentAttackNumber = 0;
                    nextAttackNumber = 0;
                    StartAttack();
                    break;
                case Player.States.GroundIntoAir:
                    currentAttackNumber = 0;
                    nextAttackNumber = 0;
                    StartAttack();
                    break;
                case Player.States.Air:
                    currentAttackNumber = 0;
                    nextAttackNumber = 0;
                    StartAttack();
                    break;
            }
        }
    }
    private void StartAttack()
    {
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;
        player.state = Player.States.Attack;

        chainAttack = false;
        readInput = false;
        attackTimer = 0;
        currentAttackTime = attacks[currentAttackNumber].attackLength;
        currentBufferTime = attacks[currentAttackNumber].attackLength - attacks[currentAttackNumber].inputBuffer;

        elementalSwitchNumber = -1;
        player.ChangeAnimationState(attacks[currentAttackNumber].animations.ToString());

        state = States.Attack;
    }
    private void Attack()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer > currentBufferTime)
        {
            InputBuffer();
            if(attackTimer > currentAttackTime)
            {
                ExecuteAttack();
            }
        }
    }
    public void ActivateInputBuffer()
    {
        readInput = true;
    }
    private void InputBuffer()
    {
        if (readInput == false) return;

        if (currentAttackNumber < (maxComboLength - 1) && chainAttack == false)
        {
            if (controls.Player.Attack.WasPerformedThisFrame())
            {
                nextAttackNumber++;
                chainAttack = true;
            }
            else if (controls.Player.Element1.WasPerformedThisFrame())
            {
                if (player.currentElementNumber != 0)
                {
                    nextAttackNumber = 3;
                    chainAttack = true;
                    elementalSwitchNumber = 0;
                }
            }
            else if (controls.Player.Element2.WasPerformedThisFrame())
            {
                if (player.currentElementNumber != 1)
                {
                    nextAttackNumber = 4;
                    chainAttack = true;
                    elementalSwitchNumber = 1;
                }
            }
            else if (controls.Player.Element3.WasPerformedThisFrame())
            {
                if (player.currentElementNumber != 2)
                {
                    nextAttackNumber = 5;
                    chainAttack = true;
                    elementalSwitchNumber = 2;
                }
            }
        }
    }
    public void ExecuteAttack()
    {
        if (player.state != Player.States.Attack) return;

        DealDamage();
        if (chainAttack == true)
        {
            if (elementalSwitchNumber != -1)
            {
                Debug.Log("number:" + elementalSwitchNumber);
                player.playerAbilties.ElementalSwitch(elementalSwitchNumber);
            }
            currentAttackNumber = nextAttackNumber;
            StartAttack();
        }
        else
        {
            state = States.Empty;
            player.SwitchToAir();
        }
    }
    private void DealDamage()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(attacks[currentAttackNumber].attackCollider.bounds.center, attacks[currentAttackNumber].attackCollider.radius, enemyLayer);

        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out Health health))
            {
                health.TakeDamage(attacks[currentAttackNumber].damage);
            }
        }
    }

    [Serializable]
    public struct AttackValues
    {
        //animation
        public float attackLength;
        public float inputBuffer;
        public int damage;
        public CircleCollider2D attackCollider;
        public PlayerAnimations animations;
    }
}
