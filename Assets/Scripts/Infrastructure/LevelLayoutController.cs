using System;
using System.Collections.Generic;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using DukeOfThieves.StaticData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DukeOfThieves.Infrastructure
{
    public class LevelLayoutController : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _levelContainer;
        [SerializeField]
        private Grid _tilemapGrid;
        [SerializeField]
        private Transform _emptyCellsParent;
        [SerializeField] 
        private LevelTimerController _levelTimerController;

        private ILevelSessionDataService _sessionService;
        private CoinLogic _coinPrefab;
        private GameObject _levelObject;
        private GameObject _coinObject;
        private List<GameObject> _coins;

        public void Init(LevelStaticData levelData, CoinLogic coinPrefab, int levelIndex, ILevelSessionDataService sessionService, Action<bool> onLevelFinished)
        {
            _coins = new List<GameObject>();
            _sessionService = sessionService;
            _coinPrefab = coinPrefab;
            _levelObject = GameObject.Instantiate(levelData.GamePrefab, _levelContainer.transform);
            _levelObject.transform.localPosition = _levelContainer.transform.localPosition;
            InitializeTilemap();
            _levelTimerController.StartTimer(onLevelFinished);
        }

        public void CleanUp(GameObject heroGameObject)
        {
            DestroyImmediate(_levelObject);
            DestroyImmediate(heroGameObject);
            if (_coins.Count >= 0)
            {
                foreach (var coin in _coins)
                {
                    DestroyImmediate(coin);
                }
            }
            
            _coins.Clear();
        }
        
        private void InitializeTilemap()
        {
            var tileMap = _tilemapGrid.GetComponentInChildren<Tilemap>();
            var size = tileMap.cellBounds;

            for (int i = size.xMin; i < size.xMax; i++)
            {
                for (int j = size.yMin; j < size.yMax; j++)
                {
                    var localPlace = (new Vector3Int(i, j, (int)tileMap.transform.position.y));
                    if (!tileMap.HasTile(localPlace))
                    {
                        //No tile at "place"
                        var place = tileMap.CellToWorld(localPlace);
                        CreateCoinInCell(place, i, j);
                    }
                }
            }
        }
        
        private void CreateCoinInCell(Vector3 place, int x, int y)
        {
            _coinObject = Instantiate(_coinPrefab.gameObject, _emptyCellsParent);
            _coinObject.name = "cell_" + x.ToString() + "_" + y.ToString();
            _coinObject.transform.position = place;
            _coinObject.GetComponent<CoinLogic>().Initialize(_sessionService.AddLootInfo);
            _coins.Add(_coinObject);
        }
        
    }
}