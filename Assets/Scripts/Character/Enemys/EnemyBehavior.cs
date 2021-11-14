using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : Character
{
    public Sprite deathSprite;

    public Sprite DeathSprite { get => deathSprite; set => deathSprite = value; }

    public void NormalBehavior()
    {
        InvokeAttackSkill();
    }

    public override IEnumerator AttackSkill()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        List<Character> possibleTargets = new List<Character>();

        for (int i = 0; i < turnManager.charactersInBattle.Length; i++)
        {
            if (turnManager.charactersInBattle[i].Type == CharacterType.Player)
                possibleTargets.Add(turnManager.charactersInBattle[i]);
        }

        int targetId = Random.Range(0, possibleTargets.Count);

        target = possibleTargets[targetId];

        return base.AttackSkill();
    }

    void OnMouseOver()
    {
        Cursor.SetActive(true);
    }

    void OnMouseExit()
    {
        Cursor.SetActive(false);
    }
}
