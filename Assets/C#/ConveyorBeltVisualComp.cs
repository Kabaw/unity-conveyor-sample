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
    private List<Transform> AddToBeltSections;

    private float speed => conveyorController.speed;

    void Start()
    {
        conveyorController = GetComponentInParent<ConveyorController>();

        conveyorController.onConveyorDiractionChange += OnConveyorDiractionChange;

        Init();
    }

    private void Init()
    {
        direction = conveyorController.direction;

        CalculateTurnPointsAngularSpeed();
    }

    void Update()
    {
        CalculateTurnPointsAngularSpeed();
        MoveBelt();
        RotateTurnPoint(leftTurnPoint);
        RotateTurnPoint(rightTurnPoint);
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

    private void MoveBelt()
    {
        int beltCount = 0;
        int sectionDirection;
        Vector3 sectionPosition;
        Transform section;
        Transform turnPoint;

        while (beltCount < beltSections.Count)
        {
            section = beltSections[beltCount];
            sectionDirection = (section.position.y > centerPoint.position.y ? 1 : -1) * direction;
            sectionPosition = section.position;

            sectionPosition.x = sectionPosition.x += speed * sectionDirection * Time.deltaTime;

            turnPoint = sectionDirection > 0 ? rightTurnPoint : leftTurnPoint;

            if (Mathf.Abs(sectionPosition.x) >= Mathf.Abs(turnPoint.position.x))
            {
                sectionPosition.x = turnPoint.position.x;
                section.parent = turnPoint;
                beltSections.RemoveAt(beltCount);
            }
            else
            {
                beltCount++;
            }
            
            section.position = sectionPosition;                      
        }        
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
