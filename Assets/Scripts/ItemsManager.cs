using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class ItemsManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _gridArea;
    [SerializeField] private Transform ActiveFoods;
    [SerializeField] private Transform ActivePowerUps;
    [SerializeField] private List<GameObject> Foods = new List<GameObject>();
    [SerializeField] private List<GameObject> PowerUps = new List<GameObject>();

    GameObject newFood;
    GameObject newPowerUp;

    public bool stop;
    private void Start()
    {
        stop = false;
        _gridArea = GameObject.FindGameObjectWithTag("SpawnGridArea").GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!stop)
        {
            if (ActiveFoods.childCount == 0)
            {
                Invoke(nameof(SpawnFood), 2);
            }

            if (ActivePowerUps.childCount == 0)
            {
                Invoke(nameof(SpawnPowerUp), 10);
            }

            if (ActiveFoods.childCount > 1)
            {
                CancelInvoke("SpawnFood");
            }

            if (ActivePowerUps.childCount > 0)
            {
                CancelInvoke("SpawnPowerUp");
            }
        }
        
    }

    private void SpawnFood()
    {
        int ind = Random.Range(0, Foods.Count);
        newFood = Instantiate(Foods[ind], RandomSpawnPoint(), Quaternion.identity);
        newFood.transform.SetParent(ActiveFoods);
    }

    private void SpawnPowerUp()
    {
        int ind = Random.Range(0, PowerUps.Count);
        newPowerUp = Instantiate(PowerUps[ind], RandomSpawnPoint(), Quaternion.identity);
        newPowerUp.transform.SetParent(ActivePowerUps);
    }

    private Vector2 RandomSpawnPoint()
    {
        Bounds bounds = this._gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(Mathf.Round(x), Mathf.Round(y));
    }
}
