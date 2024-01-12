using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.HableCurve;
using Random = UnityEngine.Random;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _gridArea;
    [SerializeField] private List<GameObject> Foods = new List<GameObject>();

    GameObject newFood;

    private void Start()
    {
        _gridArea = GameObject.FindGameObjectWithTag("SpawnGridArea").GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if(this.transform.childCount == 0)
        {
            Invoke(nameof(SpawnFood), 2);
        }
        if(this.transform.childCount > 1)
        {
            CancelInvoke("SpawnFood");
        }
        
    }

    private void SpawnFood()
    {
        int ind = Random.Range(0, Foods.Count);        
        newFood = Instantiate(Foods[ind], RandomSpawnPoint(), Quaternion.identity);
        newFood.transform.SetParent(this.transform);
    }

    private Vector2 RandomSpawnPoint()
    {
        Bounds bounds = this._gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(Mathf.Round(x), Mathf.Round(y));
    }
}
