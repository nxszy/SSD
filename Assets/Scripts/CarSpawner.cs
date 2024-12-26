using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public GridManager gridManager;
    public Vector2 spawnPosition;
    public float spawnInterval = 2f;

    void Start()
    {
        InvokeRepeating("SpawnCar", 0, spawnInterval);
    }

    void SpawnCar()
    {
        int x = (int)spawnPosition.x;
        int y = (int)spawnPosition.y;
        if (gridManager.grid[x, y].occupiedBy == null)
        {
            GameObject car = Instantiate(carPrefab, new Vector3(x, y, -0.5f), Quaternion.identity);
            Car carScript = car.GetComponent<Car>();
            gridManager.grid[x, y].occupiedBy = carScript;
            carScript.currentCell = gridManager.grid[x, y];
        }
    }
}