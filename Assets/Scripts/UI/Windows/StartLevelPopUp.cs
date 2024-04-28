﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DukeOfThieves.Data;
using DukeOfThieves.Infrastructure;
using DukeOfThieves.Services;
using TMPro;
using UI.Windows.Widgets;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class StartLevelPopUp : Window
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private List<LevelCardWidget> _levelCardWidgets;

        private int _selectedLevelIndex;
        private PlayerProgress _playerProgress;

        public override void OnPush()
        {
            _startGameButton.onClick.AddListener(HandleStartGameButton);
            _playerProgress = AllServices.Container.Single<IPersistentProgressService>().Progress;
            InitWidgets(_playerProgress.WorldData);

            PushFinished();
        }

        public override void OnPop()
        {
            
            _startGameButton.onClick.RemoveAllListeners();
            PopFinished();
        }

        private void HandleStartGameButton()
        {
            AllServices.Container.Single<IGameStateMachine>().Enter<LoadLevelState, int>(_selectedLevelIndex);
        }

        private void InitWidgets(WorldData worldData)
        {
            var levelStorage = AllServices.Container.Single<IStaticDataService>().LevelStorage;
            UIHelper.InitWidgets(_levelCardWidgets, levelStorage.GetTotalLevelAmount(),
                (w, index) =>
                {
                    w.Init(levelStorage.GetLevelByIndex(index), worldData.LootDataForLevel(index));
                });
        }
    }
}