// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for managing the game's turn sequence, which always follows
    /// the order:
    ///     - Player turn.
    ///     - Enemies turn.
    /// It creates event-like entities with the Event component attached to them when the turn
    /// sequence advances (beginning of player turn, end of player turn, beginning of enemies
    /// turn, end of enemies turn).
    /// </summary>
    public class ManaResetSystem : BaseSystem
    {
#pragma warning disable 649
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
#pragma warning restore 649

        public void OnPlayerTurnBegan()
        {
            playerConfig.Mana.SetValue(3);
        }
    }
}
