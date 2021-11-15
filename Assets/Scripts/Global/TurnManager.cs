using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public delegate void FinishedTurn();
    public static event FinishedTurn OnFinishedTurn; //Evento disparado sempre que um turno finaliza

    [Header("References")]
    public Character[] charactersInBattle; //Lista com todos os personagens na atual batalha
    public Character[] charactersPortrait; //Lista com todos os retratos de personagem

    [Header("Battle HUD Status")]
    public GameObject hudAction; //Objeto que guarda a a HUD de opções em batalha.
    public GameObject battleHudObj; //Objeto de prefab para instanciar as HUDs de batalha.
    public GameObject battleHudParent;  //Objeto que vai receber as HUDs.
    public List<BattleHUD> bHUDInstantiateds; //Lista que vai guardar os objetos instanciados.

    [Header("Turn Order UI")]
    public GameObject turnPanel; //Painel que irá armazenar os retratos e a ordem em turnos
    public GameObject portraitTurnUI; //Prefab que irá instanciar cada retrato.

    void Start()
    {
        SetupBattle();
        SetupTurnUI(3);
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

    void SetupTurnUI(int calls) //A função ainda tá primitiva, mas já trouxe uma primeira versão visual da ordenação dos turnos
    {
        if (turnPanel.transform.childCount >= charactersInBattle.Length)
            for (int i = 0; i < turnPanel.transform.childCount; i++)
            {
                Destroy(turnPanel.transform.GetChild(i).gameObject);
            }

        for (int c = 0; c < calls; c++)
        {
            int index = 0;

            for (int i = ControlGlobalVariables.currentTurnCharacterValue;
            index < charactersInBattle.Length; i++)
            {
                if (i >= charactersInBattle.Length) i = 0;

                if (charactersInBattle[i].CurrentHP >= 0)
                {
                    GameObject portrait = Instantiate(portraitTurnUI, turnPanel.transform);
                    portrait.name = charactersInBattle[i].Id.ToString();
                    portrait.transform.GetChild(0).GetComponent<Image>().sprite = charactersInBattle[i].Portrait;
                    portrait.GetComponent<ControlMarkedSprite>().pos = charactersInBattle[i].PosPortrait;
                }

                index++;
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
            SetupTurnUI(3); //A chamada aqui ainda tá inconsistente caso algum inimigo morra na batalha.
        }
    }
}