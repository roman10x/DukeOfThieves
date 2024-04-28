using System;
using DukeOfThieves.Data;
using DukeOfThieves.StaticData;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Windows.Widgets
{
    public class LevelCardWidget : UIForm, ISelectHandler
    {
        /*[SerializeField] 
        private Button _selectButton;*/
        [SerializeField] 
        private Image _levelIcon;
        [SerializeField] 
        private TMP_Text _collectedGoldRecordLabel;
        [SerializeField] 
        private TMP_Text _levelNameLabel;
        [SerializeField] 
        private GameObject _selectedFrame;
        
        private int _levelId;
        private Action<int> _onSelect;
        private static Action<int> _onSelectedInternal; // To check if another widget was selected

        public void Init(LevelStaticData levelData, Action<int> onSelect, LootData lootData, int levelIndex, bool startSelected = false)
        {
            _levelId = levelIndex;
            _onSelect = onSelect;
            
            if(startSelected)
                EventSystem.current.SetSelectedGameObject(gameObject);
            
            _levelIcon.sprite = levelData.MenuImage;
            _levelNameLabel.text = levelData.LevelKey; // Connect key to localization
            var collectedCoins = lootData.CollectedCoins;
            _collectedGoldRecordLabel.text = $"x{collectedCoins}";
            
            _onSelectedInternal -= HandleWidgetSelection;
            _onSelectedInternal += HandleWidgetSelection;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            _selectedFrame.gameObject.SetActive(true);
            _onSelect?.Invoke(_levelId);
            _onSelectedInternal?.Invoke(_levelId);
        }
        private void HandleWidgetSelection(int id)
        {
            if(id != _levelId && _selectedFrame != null)
                _selectedFrame.gameObject.SetActive(false);
        }
    }
}