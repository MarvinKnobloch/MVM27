using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private Player player;
    private Controls controls;

    [SerializeField] private AttackValues[] attacks;
    [SerializeField] private LayerMask enemyLayer;
    private float attackTimer;
    private float currentAttackTime;
    private float currentBufferTime;

    private int currentChainAttackNumber;
    private bool chainAttack;

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
                Attack();
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
                    StartAttack(0);
                    break;
                case Player.States.GroundIntoAir:
                    StartAttack(0);
                    break;
                case Player.States.Air:
                    StartAttack(0);
                    break;
            }
        }
    }
    private void StartAttack(int attackComboNumber)
    {
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;
        player.state = Player.States.Attack;

        currentChainAttackNumber = attackComboNumber;
        chainAttack = false;
        attackTimer = 0;
        currentAttackTime = attacks[currentChainAttackNumber].attackLength;
        currentBufferTime = attacks[currentChainAttackNumber].attackLength - attacks[currentChainAttackNumber].inputBuffer;

        state = States.Attack;
    }
    private void Attack()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer > currentBufferTime)
        {
            if (controls.Player.Attack.WasPerformedThisFrame() && currentChainAttackNumber < attacks.Length)
            {
                chainAttack = true;
            }

            if(attackTimer > currentAttackTime)
            {
                DealDamage();
                if (chainAttack == true)
                {
                    StartAttack(currentChainAttackNumber + 1);
                }
                else
                {
                    state = States.Empty;
                    player.SwitchToAir();
                }
            }

        }
    }
    private void DealDamage()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(attacks[currentChainAttackNumber].attackCollider.bounds.center, attacks[currentChainAttackNumber].attackCollider.radius, enemyLayer);

        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out Health health))
            {
                health.TakeDamage(attacks[currentChainAttackNumber].damage);
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
    }
}
