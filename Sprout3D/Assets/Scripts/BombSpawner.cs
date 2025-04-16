using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;
    private GameObject currentBomb;
    private Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = transform.position;
        SpawnBomb();
    }

    public void SpawnBomb()
    {
        if (currentBomb == null)
        {
            currentBomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
            Bomb bombScript = currentBomb.GetComponent<Bomb>();
            if (bombScript != null)
            {
                bombScript.SetSpawner(this);
            }
        }
    }

    public void BombDestroyed()
    {
        currentBomb = null;
        SpawnBomb();
    }
}