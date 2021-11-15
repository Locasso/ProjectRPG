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

    public delegate void PlayerDamage(int dmg, GameObject character);
    public static event PlayerDamage OnPlayerDamage;

    public delegate void SomeoneKilled(GameObject deadCharacter);
    public static event SomeoneKilled OnSomeoneKilled;

    public delegate void AdjustLife(int id, int currentLife, Character character);
    public static event AdjustLife OnAdjustLife;

    [Header("Character Status")]
    [SerializeField] private CharacterType type;

    [SerializeField] private Sprite portrait;
    [SerializeField] private Vector2 posPortrait;

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

    private GameObject targetStep; //Position stepped in front of every character
    private Animator anim;
    private GameObject cursor;



    public CharacterType Type { get => type; set => type = value; }
    public string CharacterName { get => characterName; set => characterName = value; }

    public Sprite Portrait { get => portrait; set => portrait = value; }
    public Vector2 PosPortrait { get => posPortrait; set => posPortrait = value; }

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
    public GameObject TargetStep { get => targetStep; set => targetStep = value; }
    public GameObject Cursor { get => cursor; set => cursor = value; }

    public int Id { get => id; set => id = value; }


    void Start()
    {
        SetupCharacter();
    }

    void SetupCharacter()
    {
        id = GetInstanceID();
        targetStep = transform.Find("step").gameObject;
        cursor = transform.Find("cursor").gameObject;
        if (type == CharacterType.Enemy) //Enquanto player não tem animator
            anim = transform.Find("animator").GetComponent<Animator>();
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

        Vector2 startPosition = transform.position;
        transform.position = target.TargetStep.transform.position;


        bool didDie = target.TakeDamage(attack, target.gameObject);
        if (didDie)
            OnSomeoneKilled?.Invoke(target.gameObject);

        Debug.Log($"{characterName} atacou e deu {attack} de dano");
        yield return new WaitForSeconds(1f);
        transform.position = startPosition;
        yield return new WaitForSeconds(1f);
        finishedTurn = true;
    }

    public bool TakeDamage(int dmg, GameObject target)
    {
        currentHP -= dmg;
        OnAdjustLife?.Invoke(id, currentHP, this);

        //Enquanto só os inimigos tem a animator. Depois essa linha vai padornizar pra todos os personagens
        SpriteRenderer sprite = type == CharacterType.Player ? GetComponent<SpriteRenderer>()
        : transform.Find("animator").GetComponent<SpriteRenderer>();
        StartCoroutine(DamageFeedback(sprite, dmg, target));

        if (currentHP <= 0)
            return true;
        else
            return false;

    }

    IEnumerator DamageFeedback(SpriteRenderer sprite, int dmg, GameObject character)
    {
        float posFeedback = type == CharacterType.Player ? 0.2f : -0.3f;
        sprite.color = Color.red;
        OnPlayerDamage?.Invoke(dmg, character);
        transform.position = new Vector2(transform.position.x + posFeedback, transform.position.y - posFeedback);
        yield return new WaitForSeconds(0.3f);
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.3f);
        transform.position = new Vector2(transform.position.x - posFeedback, transform.position.y + posFeedback);
    }

    public void InvokeDeathFeedback()
    {
        StartCoroutine(DeathFeedback(anim));
    }

    IEnumerator DeathFeedback(Animator animator)
    {
        animator.SetBool("death", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
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
