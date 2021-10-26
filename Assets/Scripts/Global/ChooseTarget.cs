using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTarget : MonoBehaviour
{
    bool waitForTarget;

    public void Update()
    {
        if (waitForTarget)
            FindTarget();
    }

    void WaitTarget()
    {
        waitForTarget = true;
    }

    public GameObject FindTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 64);

            if (hit.collider != null)
            {
                waitForTarget = false;
                return hit.transform.gameObject;
            }
            else return null;
        }
        else
            return null;
    }

    void OnEnable()
    {
        Character.OnPlayerAttacked += WaitTarget;
    }

    void OnDisable()
    {
        TurnManager.OnFinishedTurn -= WaitTarget;
    }
}
