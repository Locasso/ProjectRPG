using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathSprite : MonoBehaviour
{
    [SerializeField] private Sprite deathSprite;

    public void SetDeathSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprite;
    }
}
