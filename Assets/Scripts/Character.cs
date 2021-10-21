using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Player,
    Enemy
}
public class Character : MonoBehaviour
{
    [Header("Character Status")]
    [SerializeField] private CharacterType type;
    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int speed;

    [Header("Control")]
    [SerializeField] private bool finishedTurn;

    public CharacterType Type { get => type; set => type = value; }
    public int Hp { get => hp; set => hp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool FinishedTurn { get => finishedTurn; set => finishedTurn = value; }

    void Start()
    {

    }

    void Update()
    {

    }

    public void InvokeAttackSkill()
    {
        StartCoroutine(AttackSkill());
    }

    IEnumerator AttackSkill()
    {
        if(type == CharacterType.Player)
        {
            
        }

        Debug.Log($"{gameObject.name} atacou e deu {attack} de dano");
        finishedTurn = true;
        yield return new WaitForSeconds(1f);
    }

    void SetupFinishedTurn()
    {
        finishedTurn = false;
    }

    void OnEnable()
    {
        TurnManager.OnFinishedTurn += SetupFinishedTurn;
    }

    void OnDisable()
    {
         TurnManager.OnFinishedTurn -= SetupFinishedTurn;
    }
}
