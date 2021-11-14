using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void KillSomeone(GameObject deadCharacter)
    {
        Debug.Log(deadCharacter.GetComponent<Character>().CharacterName + " morreu!");
        deadCharacter.GetComponent<Character>().InvokeDeathFeedback();
    }

    void OnEnable()
    {
        Character.OnSomeoneKilled += KillSomeone;
    }

    void OnDisable()
    {
        Character.OnSomeoneKilled -= KillSomeone;
    }
}
