using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas currentCanvas;
    [SerializeField] private GameObject floatDamage;

    void KillSomeone(GameObject deadCharacter) //Função escrita para o evento de quando algum personagem é morto
    {
        Debug.Log(deadCharacter.GetComponent<Character>().CharacterName + " morreu!");
        deadCharacter.GetComponent<Character>().InvokeDeathFeedback();
    }

    void SpawnDamage(int dmg, GameObject character)
    {
        GameObject dmgText = Instantiate(floatDamage, character.transform.position, character.transform.rotation, 
        currentCanvas.transform);
        dmgText.GetComponentInChildren<TMP_Text>().SetText(dmg.ToString());
    }

    void OnEnable()
    {
        Character.OnSomeoneKilled += KillSomeone;
        Character.OnPlayerDamage += SpawnDamage;
    }

    void OnDisable()
    {
        Character.OnSomeoneKilled -= KillSomeone;
        Character.OnPlayerDamage -= SpawnDamage;
    }
}
