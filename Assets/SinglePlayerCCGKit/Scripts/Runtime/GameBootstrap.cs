// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

using Random = UnityEngine.Random;

namespace CCGKit
{
    /// <summary>
    /// This component is responsible for bootstrapping the game/battle scene. This process
    /// mainly consists on:
    ///     - Creating the player character and the associated UI widgets.
    ///     - Creating the enemy character/s and the associated UI widgets.
    ///     - Starting the turn sequence.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
        [SerializeField]
        private List<NonPlayableCharacterConfiguration> enemyConfig;

        [SerializeField]
        private DeckDrawingSystem deckDrawingSystem;
        [SerializeField]
        private HandPresentationSystem handPresentationSystem;
        [SerializeField]
        private TurnManagementSystem turnManagementSystem;
        [SerializeField]
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;
        [SerializeField]
        private EnemyBrainSystem enemyBrainSystem;
        [SerializeField]
        private EffectResolutionSystem effectResolutionSystem;
        [SerializeField]
        private PoisonResolutionSystem poisonResolutionSystem;

        [SerializeField]
        private AssetReference characterTemplate;
        [SerializeField]
        private AssetReference enemyTemplate;

        [SerializeField]
        public Transform playerPivot;
        [SerializeField]
        public Transform enemyPivot;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private ManaWidget manaWidget;
        [SerializeField]
        private DeckWidget deckWidget;
        [SerializeField]
        private DiscardPileWidget discardPileWidget;
        [SerializeField]
        private EndTurnButton endTurnButton;

        [SerializeField]
        private ObjectPool cardPool;
#pragma warning restore 649

        private Camera mainCamera;

        private CardLibrary playerDeck;

        private GameObject player;
        private GameObject enemy;

        private void Start()
        {
            mainCamera = Camera.main;

            cardPool.Initialize();

            Addressables.InitializeAsync().Completed += op =>
            {
                CreatePlayer(characterTemplate);

                // Use the existing map info (if any) to populate the enemy template. Otherwise, use
                // the one set in the game bootstrap object.
                var template = enemyTemplate;
                /*var info = FindObjectOfType<MapInfo>();
                /if (info != null)
                {
                    template = info.EnemyTemplate;
                    Destroy(info.gameObject);
                }*/
                CreateEnemy(template);
            };
        }

        private void CreatePlayer(AssetReference templateRef)
        {
            var handle = Addressables.LoadAssetAsync<PlayerTemplate>(templateRef);
            handle.Completed += op =>
            {
                var template = op.Result;
                player = Instantiate(template.Prefab, playerPivot);
                Assert.IsNotNull(player);

                playerDeck = template.StartingDeck;

                var hp = playerConfig.Hp;
                var mana = playerConfig.Mana;
                var shield = playerConfig.Shield;
                hp.Value = template.Hp;
                mana.Value = template.Mana;
                shield.Value = 0;

                CreateHpWidget(playerConfig.HpWidget, player, hp, shield);
                CreateStatusWidget(playerConfig.StatusWidget, player);

                manaWidget.Initialize(mana);

                var obj = player.GetComponent<CharacterObject>();
                obj.Template = template;
                obj.Character = new RuntimeCharacter
                { 
                    Hp = hp, 
                    Shield = shield,
                    Mana = mana, 
                    Status = playerConfig.Status 
                };
                obj.Character.Status.Value.Clear();

                if (player && enemy)
                    InitializeGame();
            };
        }

        private void CreateEnemy(AssetReference templateRef)
        {
            var handle = Addressables.LoadAssetAsync<EnemyTemplate>(templateRef);
            handle.Completed += op =>
            {
                var template = op.Result;
                enemy = Instantiate(template.Prefab, enemyPivot);
                Assert.IsNotNull(enemy);

                var initialHp = Random.Range(template.HpLow, template.HpHigh + 1);
                var hp = enemyConfig[0].Hp;
                var shield = enemyConfig[0].Shield;
                hp.Value = initialHp;
                shield.Value = 0;

                CreateHpWidget(enemyConfig[0].HpWidget, enemy, hp, shield);
                CreateStatusWidget(enemyConfig[0].StatusWidget, enemy);
                CreateIntentWidget(enemyConfig[0].IntentWidget, enemy);

                var obj = enemy.GetComponent<CharacterObject>();
                obj.Template = template;
                obj.Character = new RuntimeCharacter 
                { 
                    Hp = hp, 
                    Shield = shield,
                    Status = enemyConfig[0].Status
                };
                obj.Character.Status.Value.Clear();

                if (player && enemy)
                    InitializeGame();
            };
        }

        private void InitializeGame()
        {
            deckDrawingSystem.Initialize(deckWidget, discardPileWidget);
            var deckSize = deckDrawingSystem.LoadDeck(playerDeck);
            deckDrawingSystem.ShuffleDeck();

            handPresentationSystem.Initialize(cardPool, deckWidget, discardPileWidget);

            var playerCharacter = player.GetComponent<CharacterObject>();
            var enemyCharacter = enemy.GetComponent<CharacterObject>();
            cardWithArrowSelectionSystem.Initialize(playerCharacter, enemyCharacter);
            enemyBrainSystem.Initialize(playerCharacter, enemyCharacter);
            effectResolutionSystem.Initialize(playerCharacter, enemyCharacter);
            poisonResolutionSystem.Initialize(playerCharacter, enemyCharacter);

            turnManagementSystem.BeginGame();
        }

        private void CreateHpWidget(GameObject prefab, GameObject character, IntVariable hp, IntVariable shield)
        {
            var hpWidget = Instantiate(prefab, canvas.transform, false);
            var pivot = character.transform;
            var canvasPos = mainCamera.WorldToViewportPoint(pivot.position + new Vector3(0.0f, -0.5f, 0.0f));
            hpWidget.GetComponent<RectTransform>().anchorMin = canvasPos;
            hpWidget.GetComponent<RectTransform>().anchorMax = canvasPos;
            hpWidget.GetComponent<HpWidget>().Initialize(hp, shield);
        }

        private void CreateStatusWidget(GameObject prefab, GameObject character)
        {
            var widget = Instantiate(prefab, canvas.transform, false);
            var pivot = character.transform;
            var canvasPos = mainCamera.WorldToViewportPoint(pivot.position + new Vector3(0.0f, -1.1f, 0.0f));
            widget.GetComponent<RectTransform>().anchorMin = canvasPos;
            widget.GetComponent<RectTransform>().anchorMax = canvasPos;
        }

        private void CreateIntentWidget(GameObject prefab, GameObject character)
        {
            var widget = Instantiate(prefab, canvas.transform, false);
            var pivot = character.transform;
            var size = character.GetComponent<BoxCollider2D>().bounds.size;
            var canvasPos = mainCamera.WorldToViewportPoint(
                pivot.position + new Vector3(-0.5f, size.y + 0.7f, 0.0f));
            widget.GetComponent<RectTransform>().anchorMin = canvasPos;
            widget.GetComponent<RectTransform>().anchorMax = canvasPos;
        }
    }
}
