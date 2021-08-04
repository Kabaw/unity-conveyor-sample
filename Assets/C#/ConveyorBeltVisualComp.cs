using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ConveyorBeltVisualComp : MonoBehaviour
{
    [SerializeField, Range(0.1f, 100)] private float turnPointRadius;    

    [Header("GameObject References")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform belt;
    [SerializeField] private Transform leftTurnPoint;
    [SerializeField] private Transform rightTurnPoint;
    [SerializeField] List<Transform> beltSections;

    private int direction;
    private float angularSpeed;
    private ConveyorController conveyorController;
    private List<Transform> addToBeltSections;
    private List<Transform> addToLeftTurnPointSections;
    private List<Transform> addToRightTurnPointSections;

    private float speed => conveyorController.speed;

    void Start()
    {
        conveyorController = GetComponentInParent<ConveyorController>();
        conveyorController.onConveyorDiractionChange += OnConveyorDiractionChange;

        Init();
    }

    private void Init()
    {
        addToBeltSections = new List<Transform>();
        addToLeftTurnPointSections = new List<Transform>();
        addToRightTurnPointSections = new List<Transform>();

        direction = conveyorController.direction;

        CalculateTurnPointsAngularSpeed();
    }

    void Update()
    {
        CalculateTurnPointsAngularSpeed();
        MoveBelt();
        RotateTurnPoint(leftTurnPoint);
        RotateTurnPoint(rightTurnPoint);
        ManageSectionparents();
    }

    private void OnConveyorDiractionChange(ConveyorController conveyorController)
    {
        if (conveyorController != this.conveyorController) return;

        direction = conveyorController.direction;
    }

    private void CalculateTurnPointsAngularSpeed()
    {
        float totalRevolutionSpeed = 2 * Mathf.PI * turnPointRadius;

        angularSpeed = (360 * speed) / totalRevolutionSpeed;
    }

    private void ManageSectionparents()
    {
        foreach (Transform section in addToLeftTurnPointSections)
            section.parent = leftTurnPoint;
        addToLeftTurnPointSections.Clear();

        foreach (Transform section in addToRightTurnPointSections)
            section.parent = rightTurnPoint;
        addToRightTurnPointSections.Clear();

        foreach (Transform section in addToBeltSections)
            section.parent = belt;
        addToBeltSections.Clear();
    }

    private void MoveBelt()
    {
        int beltCount = 0;
        int sectionDirection;
        Vector3 sectionPosition;
        Transform section;
        Transform turnPoint;
        List<Transform> addToTurnPointSections;

        while (beltCount < beltSections.Count)
        {
            section = beltSections[beltCount];
            sectionDirection = (section.position.y > centerPoint.position.y ? 1 : -1) * direction;
            sectionPosition = section.position;

            sectionPosition.x = sectionPosition.x += speed * sectionDirection * Time.deltaTime;

            turnPoint = sectionDirection > 0 ? rightTurnPoint : leftTurnPoint;

            if(sectionDirection > 0)
            {
                turnPoint = rightTurnPoint;
                addToTurnPointSections = addToRightTurnPointSections;
            }
            else
            {
                turnPoint = leftTurnPoint;
                addToTurnPointSections = addToLeftTurnPointSections;
            }

            if (Mathf.Abs(sectionPosition.x) >= Mathf.Abs(turnPoint.position.x))
            {
                CalculateSectionTurnPosition(section, turnPoint, Mathf.Abs(turnPoint.position.x - sectionPosition.x), sectionDirection);

                beltSections.RemoveAt(beltCount);
                addToTurnPointSections.Add(section);
            }
            else
            {
                beltCount++;
            }
            
            section.position = sectionPosition;                      
        }        
    }

    private void CalculateSectionTurnPosition(Transform section, Transform turnPoint, float rotateDistance, int sectionDirection)
    {
        Vector2 turnPointPosition = turnPoint.position;
        Vector2 rotationStartPosition = turnPoint.position + (Vector3.up * turnPointRadius * sectionDirection);
        Vector2 direction = (rotationStartPosition - turnPointPosition).normalized;

        float turnPointPerimeter = 2 * Mathf.PI * turnPointRadius;
        float rotationDegrees = (360 * rotateDistance) / turnPointPerimeter;

        direction = Quaternion.Euler(0, 0, rotationDegrees) * direction;

        section.position = turnPointPosition + (direction * turnPointRadius);
        section.rotation *= Quaternion.Euler(0, 0, rotationDegrees);
    }

    private void RotateTurnPoint(Transform turnPoint)
    {
        turnPoint.Rotate(Vector3.forward * -angularSpeed * direction * Time.deltaTime);

        int sectionDirection;
        Vector3 sectionPosition;
        Vector3 sectionPositionAbs;
        Vector3 turnPointPositionAbs;

        turnPointPositionAbs = MathUtil.Abs(turnPoint.position);

        foreach (Transform section in turnPoint)
        {
            sectionDirection = section.position.y > centerPoint.position.y ? 1 : -1;
            sectionPosition = section.position;
            sectionPositionAbs = MathUtil.Abs(sectionPosition);

            if (sectionPositionAbs.y - turnPointRadius < turnPointPositionAbs.y && sectionPositionAbs.x <= turnPointPositionAbs.x)
            {
                sectionPosition.x = turnPoint.position.x;
                sectionPosition.y = turnPoint.position.y + (turnPointRadius * sectionDirection);
                section.position = sectionPosition;

                section.rotation = Quaternion.Euler(Vector3.forward * -180 * direction);

                section.parent = belt;

                beltSections.Add(section);
            }
        }
    }
}