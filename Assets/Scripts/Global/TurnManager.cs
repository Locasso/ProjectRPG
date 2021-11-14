using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public delegate void FinishedTurn();
    public static event FinishedTurn OnFinishedTurn;

    [Header("References")]
    public Character[] charactersInBattle;

    [Header("HUD")]
    public GameObject hudAction;
    public GameObject battleHudObj; //Objeto de prefab para instanciar as HUDs de batalha.
    public GameObject battleHudParent;  //Objeto que vai receber as HUDs.
    public List<BattleHUD> bHUDInstantiateds; //Lista que vai guardar os objetos instanciados.

    [Header("Control")]
    public GameObject currentTurnCharacter;

    void Start()
    {
        SetupBattle();
    }

    void SetupBattle()
    {
        charactersInBattle = FindObjectsOfType<Character>();
        charactersInBattle = charactersInBattle.OrderByDescending(i => i.Speed).ToArray(); //Reordenando a lista de chars por speed
        StartCoroutine(CheckTurn());

        for (int i = 0; i < charactersInBattle.Length; i++)
        {
            if (charactersInBattle[i].GetComponent<Character>().Type == CharacterType.Player)
            {
                BattleHUD bHUD = Instantiate(battleHudObj, battleHudParent.transform).GetComponent<BattleHUD>();
                bHUD.SetHUD(charactersInBattle[i].GetComponent<Character>());
                bHUDInstantiateds.Add(bHUD);
            }
        }
    }

    void DispelHudButtons()
    {
        void ControlHud()
        {
            hudAction.SetActive(false);
        }

        for (int i = 0; i < hudAction.transform.childCount; i++)
            if (!hudAction.transform.GetChild(i).GetComponent<Button>().IsNull())
            {
                hudAction.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                hudAction.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => ControlHud());
            }
    }

    IEnumerator CheckTurn()
    {
        for (; ; )
        {
            if (ControlGlobalVariables.currentTurnCharacterValue >= charactersInBattle.Length) ControlGlobalVariables.currentTurnCharacterValue = 0;

            var character = charactersInBattle[ControlGlobalVariables.currentTurnCharacterValue];

            if (character.gameObject.activeInHierarchy)
            {
                if (character.Type == CharacterType.Player)
                {
                    hudAction.SetActive(true);
                    character.Cursor.gameObject.SetActive(true);
                    for (int i = 0; i < hudAction.transform.childCount; i++)
                    {
                        if (!hudAction.transform.GetChild(i).GetComponent<Button>().IsNull())
                        {
                            if (hudAction.transform.GetChild(i).gameObject.name == "attack_btn")
                            {
                                hudAction.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                                character.GetComponent<Character>().InvokeAttackSkill());
                            }
                        }
                        if (hudAction.transform.GetChild(i).gameObject.name == "current_char")
                            hudAction.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = character.Portrait;
                    }
                }
                else if (character.Type == CharacterType.Enemy)
                {
                    character.GetComponent<EnemyBehavior>().NormalBehavior();
                }

                yield return new WaitUntil(() => charactersInBattle[ControlGlobalVariables.currentTurnCharacterValue].GetComponent<Character>().FinishedTurn);
            }

            character.Cursor.SetActive(false);
            OnFinishedTurn?.Invoke();
            DispelHudButtons();

            ControlGlobalVariables.currentTurnCharacterValue++;
        }
    }
}
