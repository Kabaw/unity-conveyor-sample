using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnConveyorDiractionChange(ConveyorController conveyorController);

public class ConveyorController : MonoBehaviour
{
    private static OnConveyorDiractionChange _onConveyorDiractionChange;

    [SerializeField, Range(0, 100)] private float _speed = 1;
    [SerializeField, ReadOnly] private int _direction = 1;

    [Header("GameObject References")]
    [SerializeField] private SurfaceEffector2D surfaceEffector2D;

    [Header("Action Triggers")]
    [SerializeField] private bool changeDirectionAction = false;

    public int direction => _direction;
    public float speed => _speed;

    public event OnConveyorDiractionChange onConveyorDiractionChange
    {
        add { _onConveyorDiractionChange += value; }
        remove { _onConveyorDiractionChange -= value; }
    }

    void Start()
    {
        Init();
    }

    private void OnValidate()
    {
        Init();

        if(changeDirectionAction)
        {
            changeDirectionAction = false;
            ChangeDirection();
        }
    }

    private void Init()
    {
        surfaceEffector2D.speed = speed * direction;
    }

    private void ChangeDirection()
    {
        _direction *= -1;
        _onConveyorDiractionChange?.Invoke(this);
        surfaceEffector2D.speed = speed * _direction;
    }
}
