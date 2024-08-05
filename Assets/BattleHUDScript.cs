using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.UIElements;

public class BattleHUDScript : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public Slider NatureLevelSlider;
    public Image characterPortrait;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI wildnessText;

    public void SetPlayerHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        classText.text = unit.UnitClass;
        NatureLevelSlider.maxValue = 100;
        healthText.text = unit.currentHp + "/" + unit.maxHp;
        wildnessText.text = unit.wildness + "";
        characterPortrait.sprite = unit.CharacterPortrait;
    }
    public void SetBearHP(Unit unit)
    {
        healthText.text = unit.currentHp + "/" + unit.maxHp;
    }

    
}
