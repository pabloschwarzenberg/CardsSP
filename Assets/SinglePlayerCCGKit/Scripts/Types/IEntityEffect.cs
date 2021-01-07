﻿// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// Base interface of the effects that affect an entity or a set of entities.
    /// </summary>
    public interface IEntityEffect
    {
        void Resolve(RuntimeCharacter instigator, RuntimeCharacter target);
    }
}
