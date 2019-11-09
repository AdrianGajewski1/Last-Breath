﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class ZombieAI : AI
{
    [Header("Locomotion")]
    public float WalkSpeed;
    public float RunningSpeed;
    public float PatrollingSpeed;
    [HideInInspector] public float Speed;
    [SerializeField] Transform[] waypoints;
    bool hasPath = false;

    [Header("FOV")]
    public float Angle;
    public float Radius;

    [Header("Sound Effects")]
    public AudioClip AttackSFX;
    public AudioClip WalkAndPatrolSFX;
    public AudioClip RunSFX;

    [Header("------------------------")]

    public bool makeZombieIdle;

    Animator animator;

    AudioSource playerWeaponSounds;
    public LayerMask layerMask;

    Rigidbody[] ragdolls;

    [Header("Stats")]
    public int Damage;

    NavMeshAgent agent;
    AudioSource audioSource;

    enum ZombieState
    {
        Idle,
        Walking,
        Running,
        Attacking,
        Patrolling
    }

    [SerializeField] ZombieState state;

    Transform player;
    Transform localPlayer;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        ragdolls = GetComponentsInChildren<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        localPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        DisableRagdoll();
    }

    private void OnDrawGizmos()
    {
        DrawGizmos(this.gameObject.transform, Radius, Angle);
    }

    public void Update()
    {
        player = ScanForTarget<FirstPersonController>(this.gameObject.transform, layerMask, Radius, Angle);

        SetState();
        CheckForPotentialTarget(localPlayer);

        if (player != null)
        {
            RotateToTarget(this.gameObject.transform, player.transform.Find("LookAtTarget"), 3f);
            Move();
            SetUpNavMeshAgent(false);
        }
        else
        {
            if (state != ZombieState.Idle && makeZombieIdle == false)
            {
                SetUpNavMeshAgent(true);
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            SetDestination(ref agent, waypoints);
                        }
                    }
                }
            }
            else
            {
                SetUpNavMeshAgent(false);
                state = ZombieState.Idle;
                Speed = 0;
                PlayAnimation(ZombieState.Idle);
            }
        }
        
        AttackPlayer();
        playerWeaponSounds = PlayerInventoryManager.Singleton.CurrentWeapon.transform.GetComponent<AudioSource>();
    }

    //Check if zombie don't see player but can hear him
    void CheckForPotentialTarget(Transform target)
    {
        if (IsInLineOfSight(target, this.gameObject.transform, Angle, Radius, layerMask) || player != null)
            return;
        var distance = Vector3.Distance(gameObject.transform.position, target.position);

        if (distance <= Radius && playerWeaponSounds.isPlaying)
            player = target;
    }

    void SetUpNavMeshAgent(bool enabled)
    {
        agent.enabled = enabled;
        agent.speed = Speed;
    }

    void SetState()
    {

        if (player != null)
        {

            if (animator.GetBool("IsWalking") == true || animator.GetBool("IsRunning") == true)
                return;
            else
            {


                var behaviour = Random.Range(0, 2);

                //Random zombie behavior so it's not the same every time
                if (behaviour == 0)
                {
                    Speed = WalkSpeed;
                    state = ZombieState.Walking;
                }

                if (behaviour == 1)
                {
                    Speed = RunningSpeed;
                    state = ZombieState.Running;
                }

            }
        }
        else
        {

            Speed = PatrollingSpeed;
            state = ZombieState.Patrolling;
        }


        PlaySoundEffects(state);
        PlayAnimation(state);
    }

    void PlayAnimation(ZombieState state)
    {
        switch (state)
        {
            case ZombieState.Idle:
                {
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsPatrolling", false);

                }
                break;

            case ZombieState.Walking:
                {
                    animator.SetBool("IsPatrolling", false);
                    animator.SetBool("IsWalking", true);
                }
                break;

            case ZombieState.Running:
                {
                    animator.SetBool("IsPatrolling", false);
                    animator.SetBool("IsRunning", true);
                }
                break;
            case ZombieState.Patrolling:
                {
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsPatrolling", true);
                }
                break;
        }
    }

    void Move()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    void DisableRagdoll()
    {
        foreach (var rbody in ragdolls)
        {
            rbody.isKinematic = true;
        }
    }

    void AttackPlayer()
    {
        if (player == null)
            return;

        var distanceToPlayer = Vector3.Distance(gameObject.transform.position, player.transform.position);


        if (distanceToPlayer <= 2f)
        {
            Speed = 0;
            animator.SetBool("Attack", true);
        }
        else
        {
            if (state == ZombieState.Walking)
                Speed = WalkSpeed;
            else
                Speed = RunningSpeed;

            animator.SetBool("Attack", false);
        }

    }

    public void GiveDamageToPlayer()
    {
        player.GetComponent<IHealth>().GiveDamage(Damage);
    }

    void PlaySoundEffects(ZombieState state)
    {
        switch (state)
        {
            case ZombieState.Idle:
                break;
            case ZombieState.Walking:
                {
                    audioSource.clip = WalkAndPatrolSFX;
                    audioSource.Play();
                }
                break;
            case ZombieState.Running:
                {
                    audioSource.clip = RunSFX;
                    audioSource.Play();
                }
                break;
            case ZombieState.Attacking:
                {
                    audioSource.clip = AttackSFX;
                    audioSource.Play();
                }
                break;
            case ZombieState.Patrolling:
                {
                    audioSource.clip = WalkAndPatrolSFX;
                    audioSource.Play();
                }
                break;
            default:
                break;
        }
    }
}
