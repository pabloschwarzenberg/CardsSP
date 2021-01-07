// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CCGKit
{
    public class EffectResolutionSystem : BaseSystem
    {
        public HandPresentationSystem HandPresentationSystem;

        public void ResolveCardEffects(RuntimeCard card, RuntimeCharacter playerSelectedTarget)
        {
            foreach (var effect in card.Template.Effects)
            {
                var targetableEffect = effect as TargetableEffect;
                if (targetableEffect != null)
                {
                    var targets = GetTargets(targetableEffect, playerSelectedTarget, true);
                    foreach (var target in targets)
                    {
                        targetableEffect.Resolve(Player.Character, target);
                        foreach (var group in targetableEffect.SourceActions)
                        {
                            foreach (var action in group.Group.Actions)
                            {
                                action.Execute(Player.gameObject);
                            }
                        }
                        foreach (var group in targetableEffect.TargetActions)
                        {
                            foreach (var action in group.Group.Actions)
                            {
                                action.Execute(Enemy.gameObject);
                            }
                        }
                    }
                }
            }
        }

        public void ResolveCardEffects(RuntimeCard card)
        {
            foreach (var effect in card.Template.Effects)
            {
                var targetableEffect = effect as TargetableEffect;
                if (targetableEffect != null)
                {
                    targetableEffect.Resolve(Player.Character, Player.Character);
                    foreach (var group in targetableEffect.SourceActions)
                    {
                        foreach (var action in group.Group.Actions)
                        {
                            action.Execute(Player.gameObject);
                        }
                    }
                    foreach (var group in targetableEffect.TargetActions)
                    {
                        foreach (var action in group.Group.Actions)
                        {
                            action.Execute(Enemy.gameObject);
                        }
                    }
                }

                var drawCardsEffect = effect as DrawCardsEffect;
                if (drawCardsEffect != null)
                {
                    StartCoroutine(DrawCardsFromDeck(drawCardsEffect.Amount));
                }
            }
        }

        private IEnumerator DrawCardsFromDeck(int amount)
        {
            // Card drawing effects need to wait for the played card to be moved to the discard pile.
            yield return new WaitForSeconds(HandPresentationSystem.CardToDiscardPileAnimationTime + 0.1f);
            var deckDrawingSystem = FindObjectOfType<DeckDrawingSystem>();
            deckDrawingSystem.DrawCardsFromDeck(amount);
        }

        public void ResolveEnemyEffects(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                var targetableEffect = effect as TargetableEffect;
                if (targetableEffect != null)
                {
                    targetableEffect.Resolve(Enemy.Character, Player.Character);
                    foreach (var group in targetableEffect.SourceActions)
                    {
                        foreach (var action in group.Group.Actions)
                        {
                            action.Execute(Enemy.gameObject);
                        }
                    }
                    foreach (var group in targetableEffect.TargetActions)
                    {
                        foreach (var action in group.Group.Actions)
                        {
                            action.Execute(Player.gameObject);
                        }
                    }
                }
            }
        }

        private List<RuntimeCharacter> GetTargets(
            TargetableEffect effect,
            RuntimeCharacter playerSelectedTarget,
            bool playerInstigator)
        {
            var targets = new List<RuntimeCharacter>(4);

            if (playerInstigator)
            {
                switch (effect.Target)
                {
                    case EffectTargetType.Self:
                        targets.Add(Player.Character);
                        break;

                    case EffectTargetType.TargetEnemy:
                        targets.Add(playerSelectedTarget);
                        break;

                    case EffectTargetType.AllEnemies:
                        targets.Add(Enemy.Character);
                        break;

                    case EffectTargetType.All:
                        targets.Add(Player.Character);
                        targets.Add(Enemy.Character);
                        break;
                }
            }
            else
            {
                switch (effect.Target)
                {
                    case EffectTargetType.Self:
                        targets.Add(Enemy.Character);
                        break;

                    case EffectTargetType.AllEnemies:
                        targets.Add(Player.Character);
                        break;

                    case EffectTargetType.All:
                        targets.Add(Player.Character);
                        targets.Add(Enemy.Character);
                        break;
                }
            }

            return targets;
        }
    }
}
