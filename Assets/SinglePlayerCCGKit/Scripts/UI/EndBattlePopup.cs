// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This component is attached to the EndBattlePopup prefab.
    /// </summary>
    public class EndBattlePopup : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
#pragma warning restore 649

        private CanvasGroup canvasGroup;

        private const string VictoryText = "Victory!";
        private const string VictoryDescriptionText = "Congratulations! This section will include rewards in a future update.";
        private const string DefeatText = "Defeat!";
        private const string DefeatDescriptionText = "Better luck next time!";
        private const float FadeInTime = 0.4f;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1.0f, FadeInTime);
        }

        public void SetVictoryText()
        {
            titleText.text = VictoryText;
            descriptionText.text = VictoryDescriptionText;
        }

        public void SetDefeatText()
        {
            titleText.text = DefeatText;
            descriptionText.text = DefeatDescriptionText;
        }

        public void OnContinueButtonPressed()
        {
            Transition.LoadLevel("Map", 0.5f, Color.black);
        }
    }
}
