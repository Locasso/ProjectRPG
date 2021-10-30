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
    public delegate void PlayerAttacked();
    public static event PlayerAttacked OnPlayerAttacked;

    public delegate void SomeoneKilled(GameObject deadCharacter);
    public static event SomeoneKilled OnSomeoneKilled;

    public delegate void AdjustLife(int id, int currentLife, Character character);
    public static event AdjustLife OnAdjustLife;

    [Header("Character Status")]
    [SerializeField] private CharacterType type;
    [SerializeField] private string characterName;
    [SerializeField] private int currentHP;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentMana;
    [SerializeField] private int maxMana;
    [SerializeField] private int attack;
    [SerializeField] private int speed;

    [Header("Character Data")]
    int id;
    [SerializeField] private int level;
    [SerializeField] private int experience;

    [Header("Control")]
    [SerializeField] protected Character target;
    [SerializeField] private bool finishedTurn;

    public CharacterType Type { get => type; set => type = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int CurrentHP { get => currentHP; set => currentHP = value; }
    public int MaxHP { get => maxHP; set => maxHP = value; }
    public int CurrentMana { get => currentMana; set => currentMana = value; }
    public int MaxMana { get => maxMana; set => maxMana = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool FinishedTurn { get => finishedTurn; set => finishedTurn = value; }

    public int Level { get => level; set => level = value; }
    public int Experience { get => experience; set => experience = value; }

    public Character Target { get => target; set => target = value; }

    public int Id { get => id; set => id = value; }

    void Start()
    {
        id = Random.Range(0, 9999);   
    }

    void Update()
    {

    }

    public void InvokeAttackSkill()
    {
        StartCoroutine(AttackSkill());
    }

    public virtual IEnumerator AttackSkill()
    {	
        if (type == CharacterType.Player)
        {
            OnPlayerAttacked?.Invoke();

            yield return new WaitUntil(() => !FindObjectOfType<ChooseTarget>().FindTarget().IsNull());

            target = FindObjectOfType<ChooseTarget>().FindTarget().GetComponent<Character>();
        }

        bool didDie = target.TakeDamage(attack);
        if (didDie)
            OnSomeoneKilled?.Invoke(target.gameObject);

        Debug.Log($"{characterName} atacou e deu {attack} de dano");
        finishedTurn = true;
        yield return new WaitForSeconds(1f);
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        OnAdjustLife?.Invoke(id, currentHP, this);

        if (currentHP <= 0)
            return true;
        else
            return false;
        
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
