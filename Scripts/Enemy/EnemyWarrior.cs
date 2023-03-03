
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWarrior : livingEntity
{
    public NavMeshAgent agent;

    public Transform player;

    private livingEntity targetEntity; //추적대상

    private Animator enemyAnimator;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint; //도보 지점
    bool walkPointSet; //도보 지점 확인을 위한
    public float walkPointRange; //도보 지점 범위
    public float timeAfterPatrol;


    //Attacking
    public float damage = 20f; //공격력
    public float attackDelay = 1f; //공격 딜레이
    private float lastAttackTime; //마지막 공격 시점

    //States
    public float sightRange, attackRange;  //시야 범위, 공격 범위
    public bool playerInSightRange, playerInAttackRange; //bool타입 시야 범위, 공격 범위

    //anima
    private bool canMove;
    private bool canAttack;



    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;

        agent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();

    }

    private void Update()
    {

        findTarget(); //target 찾기

        //시야와 범위 확인
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);  //시야 범위
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); //공격 범위

        if (!playerInSightRange && !playerInAttackRange) //player 시야 범위, 공격 범위 내에 있지 않은 경우 (순찰)
        {
            timeAfterPatrol += Time.deltaTime;
            if (timeAfterPatrol >= 2f) {
                Patroling();
                timeAfterPatrol = 0;
            }
        }
        if (playerInSightRange && !playerInAttackRange) ChasePlayer(); //player 시야 범위안에 있지만, 공격 범위 내에 있지 않은 경우 (목적지 설정)
        if (playerInSightRange && playerInAttackRange) AttackPlayer(); //player 시야 범위, 공격 범위 내에 있음 (공격)

        enemyAnimator.SetBool("CanMove", canMove);
        enemyAnimator.SetBool("CanAttack", canAttack);
    }


    private void Patroling()
    {
        if (!walkPointSet)
        {
            canMove = false;
            canAttack = false;
            SearchWalkPoint();
        }


        if (walkPointSet)
        {
            canMove = true;
            canAttack = false;
            agent.SetDestination(walkPoint);
        }


        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            canMove = false;
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        // 범위 내 임의 지점 계산
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) //보행지점이 실제로 있는지 레이케스트로 확인
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        canMove = true;
        canAttack = false;

        agent.SetDestination(player.position);  //목적지 설정
    }

    #region attack
    private void AttackPlayer()
    {
        //자신이 사망X
        if (!dead)
        {
            //공격 반경 안에 있으면 움직임을 멈춘다.
            agent.SetDestination(transform.position); //네비움직임 멈춤
            canMove = false;

            //추적 대상 바라보기         
            this.transform.LookAt(player);

            //최근 공격 시점에서 attackDelay 이상 시간이 지나면 공격 가능
            if (lastAttackTime + attackDelay <= Time.time)
            {
                canAttack = true;
                OnDamage();
            }

            //공격 반경 안에 있지만, 딜레이가 남아있을 경우
            else
            {
                canMove = false;
                canAttack = false;
            }
        }
    }



    //데미지를 입었을 때 실행할 처리
    public override void OnDamage(float damage)
    {
        //피격 애니메이션 재생
        enemyAnimator.SetTrigger("Hit");

        //livingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage);

        //체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        if (health <= 0 && !dead)
        {
            Die();
            enemyAnimator.SetTrigger("Death");
        }
    }

    //데미지 주기
    public void OnDamage()
    {
        livingEntity attackTarget = targetEntity.GetComponent<livingEntity>();

        attackTarget.OnDamage(damage);
        lastAttackTime = Time.time;
    }

    #endregion

    void findTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsPlayer);

        //모든 콜라이더를 순회하면서 살아 있는 LivingEntity 찾기
        for (int i = 0; i < colliders.Length; i++)
        {
            //콜라이더로부터 LivingEntity 컴포넌트 가져오기
            livingEntity livingEntity = colliders[i].GetComponent<livingEntity>();

            //LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아 있다면
            if (livingEntity != null && !livingEntity.dead)
            {
                //추적 대상을 해당 LivingEntity로 설정
                targetEntity = livingEntity;
            }
        }

    }


    private void OnDrawGizmosSelected() //범위 확인
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

    /*    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //여기에 공격 스크립트( 전사, 궁수등등)

            //공격 스크립트 끝
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }*/

    /*//적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newHealth, float newDamage, float newSpeed)
    {
        //체력 설정
        startingHealth = newHealth;
        health = newHealth;

        //공격력 설정
        damage = newDamage;

        //네비메쉬 에이전트의 이동 속도 설정
        agent.speed = newSpeed;
    }*/

    /*public void OnDamage() //공격 입히기
    {
        //공격 대상을 지정할 추적 대상의 LivingEntity 컴포넌트 가져오기
        livingEntity attackTarget = GetComponent<livingEntity>();

        //공격 처리
        attackTarget.OnDamage(damage);

        //최근 공격 시간 갱신
        lastAttackTime = Time.time;
    }*/

    /*//유니티 애니메이션 이벤트로 공격하는 모션일 때 데미지 적용시키기
    public void OnDamageEvent()
    {
        //공격 대상을 지정할 추적 대상의 LivingEntity 컴포넌트 가져오기
        livingEntity attackTarget = targetEntity.GetComponent<livingEntity>();

        //공격 처리
        attackTarget.OnDamage(damage);

        //최근 공격 시간 갱신
        lastAttackTime = Time.time;
    }*/