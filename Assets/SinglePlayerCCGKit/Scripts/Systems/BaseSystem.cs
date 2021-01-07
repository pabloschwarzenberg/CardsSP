// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public abstract class BaseSystem : MonoBehaviour
    {
        protected CharacterObject Player;
        protected CharacterObject Enemy; 

        public virtual void Initialize(CharacterObject player, CharacterObject enemy)
        {
            Player = player;
            Enemy = enemy;
        }
    }
}