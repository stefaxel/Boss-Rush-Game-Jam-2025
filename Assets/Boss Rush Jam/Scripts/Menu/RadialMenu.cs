using Battle.Ability;
using Menu.RadialMenuSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu.Radial
{
    public class RadialMenu : MonoBehaviour
    {
        [SerializeField] private List<AttackAbility> allAbilities;
        [SerializeField] private Button[] menuButtons;

        public static event Action<AttackAbility> OnUpdateButton;

        public void ReplaceAbility(AttackAbility oldAbility, AttackAbility newAbility)
        {
            int buttonIndex = allAbilities.IndexOf(oldAbility);

            if(buttonIndex >= 0 && buttonIndex < menuButtons.Length)
            {
                oldAbility.isUnlocked = false;
                newAbility.isUnlocked = true;

                allAbilities[buttonIndex] = newAbility;
                UpdateMenuButtons();
            }
            
        }

        private void UpdateMenuButtons()
        {
            List<AttackAbility> unlockedAbilities = allAbilities.Where(ability => ability.isUnlocked).ToList();

            for(int i = 0; i < menuButtons.Length; i++)
            {
                if(i < unlockedAbilities.Count)
                {
                    AttackAbility abilityUnlocked = unlockedAbilities[i];
                    Button button = menuButtons[i];

                    ButtonSelect buttonSelect = button.GetComponent<ButtonSelect>();
                    if(buttonSelect != null)
                    {
                        OnUpdateButton(abilityUnlocked);
                    }
                }
            }
        }
    }
}

