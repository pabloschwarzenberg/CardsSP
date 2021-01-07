﻿// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// The type corresponding to the "Gain X mana" card effect.
    /// </summary>
    [Serializable]
    public class GainManaEffect : IntegerEffect, IEntityEffect
    {
        public override string GetName()
        {
            return $"Gain {Value.ToString()} mana";
        }

        public override void Resolve(RuntimeCharacter instigator, RuntimeCharacter target)
        {
            var targetMana = target.Mana;
            targetMana.SetValue(targetMana.Value + Value);
        }
    }
}
