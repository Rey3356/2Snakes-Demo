using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _gridArea;

    private void RandomSpawnPoint()
    {
        Bounds bounds = this._gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector2(Mathf.Round(x), Mathf.Round(y));
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision!");
        if(other.tag  == "Snake")
        {
            RandomSpawnPoint();
        }
    }
}
