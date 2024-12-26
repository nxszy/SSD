using UnityEngine;

public class Car : MonoBehaviour
{
    public GridCell currentCell;
    public float speed = 1f;

    [SerializeField]
    private float maxSpeed = 5f;

    [Header("Route references")]
    [SerializeField]
    private GridManager gridManager;

    [Header("Route")]
    private GridCell[,] grid;
    private Vector2 startPos;
    private Vector2 endPos;

    #region gameObject creation

    public void Instantiate(Vector2 start, Vector2 end)
    {
        startPos = start;
        endPos = end;
    }

    #endregion

    void Start()
    {
        grid = gridManager.grid;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (speed < maxSpeed)
        {
            speed += 1;
        }

    }

    #region Route

    private void generateRoute()
    {
        
    }

    #endregion
}
