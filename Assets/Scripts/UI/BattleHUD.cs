using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    int id;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public Slider hpSlider;
    public TMP_Text hpText;
    public Slider mpSlider;
    public TMP_Text mpText;

    int maxHp;
    int maxMp;

    public void SetHUD(Character character)
    {
        id = character.Id;
        maxHp = character.MaxHP;

        nameText.SetText(character.CharacterName);
        levelText.SetText($"Lvl.{character.Level.ToString()}");

        hpSlider.maxValue = character.MaxHP;
        hpSlider.value = character.CurrentHP;
        hpText.SetText($"{character.CurrentHP.ToString()}/{character.MaxHP.ToString()}");

        mpSlider.maxValue = character.MaxMana;
        mpSlider.value = character.CurrentMana;
        mpText.SetText($"{character.CurrentMana.ToString()}/{character.MaxMana.ToString()}");
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }

    public void SetMP(int mp)
    {
        mpSlider.value = mp;
    }

    public void SetupHpExternal(int id, int hp, Character character)
    {
        if (!hpSlider.IsNull() && id == this.id)
        {
            hpSlider.value = hp;
            hpText.SetText($"{hp.ToString()}/{character.MaxHP.ToString()}");
        }
    }

    void OnEnable()
    {
        Character.OnAdjustLife += SetupHpExternal;
    }

    void OnDisable()
    {
        Character.OnAdjustLife -= SetupHpExternal; ;
    }
}