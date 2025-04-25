using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneralFollower : MonoBehaviour
{
    [Header("Following Settings")]
    [SerializeField] private string targetGeneralName = "";
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float detectionInterval = 1.0f;
    [SerializeField] public FormationType formationType = FormationType.RandomOffset;
    [SerializeField] private float spacing = 2.0f;
    [SerializeField] private float randomOffset = 0.5f;

    private Transform targetGeneral;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private FormationManager formationManager;
    private NavMeshAgent navMeshAgent;
    public int soldierIndex;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        formationManager = GetComponentInParent<FormationManager>();
        if (formationManager == null)
        {
            formationManager = FindObjectOfType<FormationManager>();
        }

        if (formationManager != null)
        {
            soldierIndex = formationManager.soldiers.IndexOf(this);
        }

        StartCoroutine(DetectGeneralRoutine());
    }

    void Update()
    {
        if (hasTarget)
        {
            UpdateTargetPosition();
        }
    }

    IEnumerator DetectGeneralRoutine()
    {
        while (true)
        {
            FindTargetGeneral();
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    void FindTargetGeneral()
    {
        if (!string.IsNullOrEmpty(targetGeneralName))
        {
            GameObject general = GameObject.Find(targetGeneralName);
            if (general != null)
            {
                targetGeneral = general.transform;
                hasTarget = true;
                return;
            }
        }

        // 如果没有指定名称，寻找最近的将军
        GameObject[] generals = GameObject.FindGameObjectsWithTag("General");
        float minDistance = float.MaxValue;
        Transform closestGeneral = null;

        foreach (GameObject general in generals)
        {
            float distance = Vector3.Distance(transform.position, general.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestGeneral = general.transform;
            }
        }

        if (closestGeneral != null)
        {
            targetGeneral = closestGeneral;
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
            Debug.LogWarning("No general found");
        }
    }

    void UpdateTargetPosition()
    {
        if (!hasTarget || targetGeneral == null) return;

        Vector3 generalPosition = targetGeneral.position;

        if (formationManager != null && soldierIndex >= 0)
        {
            Vector3 formationOffset = CalculateFormationOffset();
            targetPosition = generalPosition + formationOffset;

            if (navMeshAgent != null)
            {
                navMeshAgent.SetDestination(targetPosition);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, followDistance * Time.deltaTime);
            }
        }
    }

    Vector3 CalculateFormationOffset()
    {
        switch (formationType)
        {
            case FormationType.Square:
                return CalculateSquareOffset();
            case FormationType.Triangle:
                return CalculateTriangleOffset();
            case FormationType.Circle:
                return CalculateCircleOffset();
            case FormationType.VShape:
                return CalculateVShapeOffset();
            case FormationType.RandomOffset:
                return CalculateRandomOffset();
            default:
                return CalculateRandomOffset();
        }
    }

    Vector3 CalculateSquareOffset()
    {
        int sideLength = Mathf.CeilToInt(Mathf.Sqrt(formationManager.soldiers.Count));
        int x = soldierIndex % sideLength - sideLength / 2;
        int y = soldierIndex / sideLength - sideLength / 2;
        return new Vector3(x * spacing, 0, y * spacing);
    }

    Vector3 CalculateTriangleOffset()
    {
        int row = 1;
        int index = 0;
        float offsetY = 0;

        while (index < soldierIndex)
        {
            offsetY -= spacing * 0.866f;
            row++;
            index += row;
        }

        int col = soldierIndex - (index - row);
        float offsetX = (col - (row - 1) / 2.0f) * spacing;
        return new Vector3(offsetX, 0, offsetY);
    }

    Vector3 CalculateCircleOffset()
    {
        float radius = spacing * Mathf.Sqrt(formationManager.soldiers.Count) / 2.0f;
        float angle = soldierIndex * 360.0f / formationManager.soldiers.Count;
        float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        float z = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
        return new Vector3(x, 0, z);
    }

    Vector3 CalculateVShapeOffset()
    {
        float angle = 60.0f;
        float halfAngle = angle / 2.0f;
        float length = spacing * formationManager.soldiers.Count / 2.0f;
        float offset = (soldierIndex - (formationManager.soldiers.Count - 1) / 2.0f) * spacing;
        float x = offset * Mathf.Tan(Mathf.Deg2Rad * halfAngle);
        return new Vector3(x, 0, -soldierIndex * spacing);
    }

    Vector3 CalculateRandomOffset()
    {
        Vector3 baseOffset = CalculateSquareOffset();
        float randomX = Random.Range(-randomOffset, randomOffset);
        float randomZ = Random.Range(-randomOffset, randomOffset);
        return baseOffset + new Vector3(randomX, 0, randomZ);
    }
}

public enum FormationType
{
    Square,
    Triangle,
    Circle,
    VShape,
    RandomOffset
}