using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMarkedSprite : MonoBehaviour
{
    public Image sprite;
    public Vector2 pos;

    void Start()
    {
        sprite.transform.position = new Vector2(sprite.transform.position.x + pos.x, 
        sprite.transform.position.y + pos.y);
    }
}
