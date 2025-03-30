using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Move))]
public class FactionAnimator : MonoBehaviour
{
    private Animator anim;
    private Move moveScript;

    [Header("Team Settings")]
    //[SerializeField] private string myTeamTag = "TeamA";
    [SerializeField] private string enemyTeamTag = "TeamB";

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionInterval = 0.3f;
    [SerializeField][Range(0.1f, 1f)] private float directionSmoothTime = 0.2f;

    // Animation Parameters
    private Vector2 currentDirection;
    private Vector2 directionVelocity;
    private Transform currentTarget;
    private bool isAttacking;

    void Awake()
    {
        anim = GetComponent<Animator>();
        moveScript = GetComponent<Move>();
        currentDirection = Vector2.down;
        StartCoroutine(DetectionRoutine());
    }

    void Update()
    {
        UpdateAnimationParameters();
    }

    IEnumerator DetectionRoutine()
    {
        while (true)
        {
            UpdateAttackTarget();
            HandleCombatState();
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    void UpdateAnimationParameters()
    {
        Vector2 targetDirection = GetTargetDirection();

        // Smooth direction transition
        currentDirection = Vector2.SmoothDamp(
            currentDirection,
            targetDirection,
            ref directionVelocity,
            directionSmoothTime
        );

        // Set animation parameters
        anim.SetFloat("MoveX", currentDirection.x);
        anim.SetFloat("MoveY", currentDirection.y);
        anim.SetFloat("Speed", moveScript.CurrentSpeed);
        anim.SetBool("Attacking", isAttacking);
    }

    Vector2 GetTargetDirection()
    {
        if (isAttacking && currentTarget != null)
        {
            return GetAttackDirection(currentTarget.position);
        }
        return GetMovementDirection();
    }

    Vector2 GetMovementDirection()
    {
        if (moveScript.MoveDirection.magnitude < 0.1f)
            return currentDirection;

        return GetCardinalDirection(moveScript.MoveDirection);
    }

    Vector2 GetAttackDirection(Vector3 targetPosition)
    {
        Vector2 attackDirection = (targetPosition - transform.position).normalized;
        return GetCardinalDirection(attackDirection);
    }

    Vector2 GetCardinalDirection(Vector2 rawDirection)
    {
        if (rawDirection.magnitude < 0.1f) return currentDirection;

        float angle = Vector2.SignedAngle(Vector2.up, rawDirection);
        angle = (angle + 360) % 360;

        return angle switch
        {
            >= 315f or < 45f => Vector2.up,    // Up (315бу-45бу)
            >= 45f and < 135f => Vector2.left,  // Left (45бу-135бу)
            >= 135f and < 225f => Vector2.down, // Down (135бу-225бу)
            _ => Vector2.right                  // Right (225бу-315бу)
        };
    }

    void UpdateAttackTarget()
    {
        currentTarget = null;
        Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position, attackRange);

        float minDistance = float.MaxValue;
        foreach (var candidate in candidates)
        {
            if (candidate.CompareTag(enemyTeamTag))
            {
                float distance = Vector2.Distance(transform.position, candidate.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentTarget = candidate.transform;
                }
            }
        }
    }

    void HandleCombatState()
    {
        if (currentTarget != null && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.SetTrigger("AttackTrigger");

        // Attack windup (non-interruptible)
        yield return new WaitForSeconds(0.2f);

        // Attack logic placeholder
        // Call your damage dealing method here

        // Attack recovery (interruptible)
        float timer = 0f;
        while (timer < 0.3f)
        {
            if (moveScript.CurrentSpeed > 0.1f)
            {
                CancelAttack();
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    void CancelAttack()
    {
        StopCoroutine(AttackRoutine());
        isAttacking = false;
        anim.ResetTrigger("AttackTrigger");
    }

    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw current direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, currentDirection * 2);
    }
}