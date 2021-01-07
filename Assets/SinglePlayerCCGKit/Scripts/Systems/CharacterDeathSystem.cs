// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// This system handles the death of a character to end the battle and display the end-of-battle
    /// popup on the screen.
    /// </summary>
    public class CharacterDeathSystem : BaseSystem
    {
        public void OnPlayerHpChanged(int hp)
        {
            if (hp <= 0)
            {
                EndGame(true);
            }
        }

        public void OnEnemyHpChanged(int hp)
        {
            if (hp <= 0)
            {
                EndGame(false);
            }
        }

        public void EndGame(bool playerDied)
        {
            var popupOverlay = FindObjectOfType<PopupOverlay>();
            var endBattlePopup = FindObjectOfType<EndBattlePopup>();
            if (popupOverlay != null && endBattlePopup != null)
            {
                popupOverlay.Show();
                endBattlePopup.Show();

                if (playerDied)
                {
                    endBattlePopup.SetDefeatText();
                }
                else
                {
                    endBattlePopup.SetVictoryText();
                }

                var turnManagementSystem = FindObjectOfType<TurnManagementSystem>();
                turnManagementSystem.SetEndOfGame(true);
            }
        }
    }
}
