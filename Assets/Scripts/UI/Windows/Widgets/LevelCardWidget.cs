using DukeOfThieves.StaticData;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Widgets
{
    public class LevelCardWidget : UIForm
    {
        [SerializeField] 
        private Button _selectButton;
        [SerializeField] 
        private Image _levelIcon;
        [SerializeField] 
        private TMP_Text _collectedGoldRecordLabel;
        [SerializeField] 
        private TMP_Text _levelNameLabel;
        [SerializeField] 
        private GameObject _selectedFrame;

        public void Init(LevelStaticData levelData, int collectedCoins)
        {
            _levelIcon.sprite = levelData.MenuImage;
            _levelNameLabel.text = levelData.LevelKey; // Connect key to localization
            _collectedGoldRecordLabel.text = $"x{collectedCoins}";
        }
    }
}