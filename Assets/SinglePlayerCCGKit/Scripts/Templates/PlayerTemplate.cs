// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Player",
        fileName = "Player",
        order = 1)]
    public class PlayerTemplate : CharacterTemplate
    {
        public int Hp;
        public int Mana;
        public CardLibrary StartingDeck;
    }
}
