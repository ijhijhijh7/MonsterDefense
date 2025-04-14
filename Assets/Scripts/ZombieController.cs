using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stopDistance = 1.0f;           // 목표지점까지의 정지 거리
    public float detectionRadius = 0.5f;        // 주변 좀비 감지 반경
    public float attackDistance = 1.5f;         // 공격 시작 거리
    public float restackCooldown = 1.0f;

    private LaneType lane;
    private Vector3 targetPosition;
    private float fixedY;
    private float lastStackTime = -999f;

    private bool hasArrived = false;
    private bool isStacking = false;

    private Coroutine moveRoutine;
    private Animator animator;

    private void Start()
    {
        fixedY = transform.position.y;
        animator = GetComponent<Animator>();
    }

    public void SetLane(LaneType laneType)
    {
        lane = laneType;
        fixedY = LaneManager.GetLaneY(lane);
        targetPosition = LaneManager.GetLaneTargetPosition(lane);
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 애니메이션 상태 변경
        if (distanceToTarget <= attackDistance)
        {
            animator.SetBool("IsAttacking", true);
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }

        // 이동 관련 처리
        if (!hasArrived || ShouldTryRestack())
        {
            if (IsBlockedByOtherZombies(out ZombieController blocker))
            {
                if (!isStacking && moveRoutine == null && Time.time - lastStackTime >= restackCooldown)
                {
                    moveRoutine = StartCoroutine(DelayedStack(blocker));
                    lastStackTime = Time.time;
                }
                return;
            }

            MoveTowardsTarget();

            if (distanceToTarget < stopDistance)
            {
                hasArrived = true;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 current = transform.position;
        Vector3 next = new Vector3(targetPosition.x, fixedY, targetPosition.z);
        transform.position = Vector3.MoveTowards(current, next, moveSpeed * Time.deltaTime);
    }

    private bool ShouldTryRestack()
    {
        return hasArrived && !isStacking && moveRoutine == null && Time.time - lastStackTime >= restackCooldown;
    }

    private bool IsBlockedByOtherZombies(out ZombieController blockingZombie)
    {
        blockingZombie = null;
        int layerMask = LayerMask.GetMask(GetLayerNameForLane(lane));
        Collider2D[] nearbyZombies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);

        float closestDist = Mathf.Infinity;
        foreach (var col in nearbyZombies)
        {
            if (col.gameObject == gameObject) continue;

            ZombieController other = col.GetComponent<ZombieController>();
            if (other != null && other.transform.position.x < transform.position.x && other.hasArrived)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist < stopDistance && dist < closestDist)
                {
                    closestDist = dist;
                    blockingZombie = other;
                }
            }
        }

        return blockingZombie != null;
    }

    private IEnumerator DelayedStack(ZombieController targetZombie)
    {
        isStacking = true;
        yield return new WaitForSeconds(Random.Range(0.2f, 1.0f));

        if (targetZombie == null) yield break;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetZombie.transform.position.x + 0.4f, fixedY, targetZombie.transform.position.z);
        float jumpHeight = 1.5f;
        float duration = 0.5f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 horizontal = Vector3.Lerp(start, end, t);
            float arc = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(horizontal.x, fixedY + arc, horizontal.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isStacking = false;
        moveRoutine = null;
        hasArrived = true;
    }

    private string GetLayerNameForLane(LaneType lane)
    {
        return lane switch
        {
            LaneType.Lane1 => "Lane1",
            LaneType.Lane2 => "Lane2",
            LaneType.Lane3 => "Lane3",
            _ => "Default"
        };
    }

    public void OnAttack()
    {
        //이후에 트럭에 가해지는 데미지 구현
    }
}
