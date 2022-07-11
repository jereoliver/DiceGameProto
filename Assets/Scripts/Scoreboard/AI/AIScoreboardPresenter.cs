using System.Collections.Generic;
using ScorePossibilities;
using TMPro;
using UnityEngine;
using Zenject;

namespace Scoreboard.AI
{
    public class AIScoreboardPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text redPointsText;
        [SerializeField] private TMP_Text yellowPointsText;
        [SerializeField] private TMP_Text greenPointsText;
        [SerializeField] private TMP_Text bluePointsText;
        [SerializeField] private TMP_Text totalPointsText;
        [SerializeField] private TMP_Text errorPointsText;
        [SerializeField] private GameObject slotsParent;
        [SerializeField] private GameObject ownTurnIndicator;
        [SerializeField] private List<ErrorPresenter> errorPresenters;

        private readonly List<SlotPresenter> slotPresenters = new List<SlotPresenter>();
        private bool currentlyOwnTurn;
        
        [Inject(Id = "AI")] private IScoreboardController scoreboardController;
        [Inject(Id = "AI")] private IScoreboardModel scoreboardModel;
        [Inject] private readonly IScorePossibilitiesController scorePossibilitiesController;
        [Inject] private SignalBus signalBus;



    }
}