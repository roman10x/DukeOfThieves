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

        private IPersistentProgressService _progressService;
        private CoinLogic _coinPrefab;

        public void Init(LevelStaticData levelData, CoinLogic coinPrefab, int levelIndex, IPersistentProgressService persistentProgressService)
        {
            _progressService = persistentProgressService;
            _coinPrefab = coinPrefab;
            var levelObj = GameObject.Instantiate(levelData.GamePrefab, _levelContainer.transform);
            levelObj.transform.localPosition = _levelContainer.transform.localPosition;
            InitializeTilemap();
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
            var emptyCellObj = Instantiate(_coinPrefab, _emptyCellsParent);
            emptyCellObj.name = "cell_" + x.ToString() + "_" + y.ToString();
            emptyCellObj.transform.position = place;
            emptyCellObj.GetComponent<CoinLogic>().Initialize();
        }
        
    }
}