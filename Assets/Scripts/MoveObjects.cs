    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] _prefab;
    [SerializeField] private Sprite[] _sprites;
    [Range (0.5f, 1f)] [SerializeField] private float _maxTime = 1f;

    private GameObject _spawnedObject;
    private Vector3 _beginningPos;
    private List<GameObject> _listOfCurrentChildsOfSpawnObject = new List<GameObject>(); //This list just holds current spawned object's childs
    private Dictionary<Vector2,GameObject> _dogrulukMan = new Dictionary<Vector2, GameObject>();

    private float _increasedTime = 0f; //timer for object to go down
    private bool[,] _GridExe = new bool[10,20];
    private bool _keepSpawning;
    private int _nextObjectIndex, previousOneIndex;
    private Sprite _nextSprite, _previousSprite;

    private void Awake()
    {
        _beginningPos = new Vector3(0, 9f);
        _keepSpawning = true;
        _nextObjectIndex = Random.Range(0, _prefab.Length);
        _nextSprite = _sprites[Random.Range(0, _sprites.Length)];
        ScoreAndExtras.OnScoreListYazdir();
    }
    
    private void Update()
    {
        if(_spawnedObject == null && _keepSpawning)
        {
            #region Spawning Object

            previousOneIndex = _nextObjectIndex;
            _previousSprite = _nextSprite;

            GameObject _selectedPrefabObj = _prefab[_nextObjectIndex];
            foreach (Transform item in _selectedPrefabObj.transform)
            {
                item.GetComponent<SpriteRenderer>().sprite = _nextSprite;                
            }

            while(_nextObjectIndex == previousOneIndex)
            {
                _nextObjectIndex = Random.Range(0, _prefab.Length);
            }
            while (_previousSprite == _nextSprite)
            {
                _nextSprite = _sprites[Random.Range(0, _sprites.Length)];
            }
            ScoreAndExtras.OnObjectSpawn(_nextObjectIndex, _nextSprite);

            

            GameObject _obj = Instantiate(_selectedPrefabObj, _beginningPos, Quaternion.identity);
            _spawnedObject = _obj;            
            #endregion
            
            //Adding current child objects of spawned object to list to limit movement
            _listOfCurrentChildsOfSpawnObject.Clear();
            foreach (Transform item in _spawnedObject.transform)
            {
                _listOfCurrentChildsOfSpawnObject.Add(item.gameObject);                
            }

            //Eğer spawn olduğu esnada altında obje varsa daha fazla hareket edemeyeceğinden oyun bitecek bu yüzden daha fazla spawn olması engelleniyor.
            if(!IsInsideDown(VerticalPreview()))
            {
                _keepSpawning = false;
                _spawnedObject = null;
                ScoreAndExtras.OnEnding();
            }
        }
        else
        {
            //Rotate Object
            if(Input.GetKeyDown(KeyCode.F))
            {
                _spawnedObject.transform.Rotate(Vector3.forward * -90f);
            }
            #region Yatay Movement
            bool moveLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
            bool moveRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
            
            int moveDir = moveLeft ? -1 : 1;        
            
            if(IsInsideHor(HorizontalPreview(moveDir)))
            {
                MoveHorizontal(ref moveLeft, ref moveRight, ref moveDir);                               
            }
            #endregion 
            #region Dikey Movement
            // When some amount time passes object go down 1br or press S to move down 1br            
            if(IsInsideDown(VerticalPreview()))
            {
                //Eğer artan zaman maxTime'ı geçerse obje 1br aşağı iner.
                _increasedTime += Time.deltaTime;

                if(_increasedTime > _maxTime)
                {              
                    MoveDown();     
                    _increasedTime = 0;
                }
                //S veya DownArrow'a basıldığında obje 1br aşağıya iner.
                MoveVertical();   
            }
            //If block cant move anymore 
            else
            {
                if(_keepSpawning)
                {
                    ScoreAndExtras.OnScoreChange();
                    AddPlacedObjects(_spawnedObject);
                }

                CheckIsThereACompleteLine();     
                _spawnedObject = null;         
            }
            #endregion 
        }       
    }
    private void MoveHorizontal(ref bool moveLeft, ref bool moveRight, ref int moveDir)
    {
        if(moveLeft || moveRight)
        {
            var pos = _spawnedObject.transform.position;
            pos.x += moveDir;
            _spawnedObject.transform.position = pos;
        }
    } 
    private void MoveVertical()
    {
        //When player press S or DownArrow, obj go down 1br
        bool moveDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        
        if(moveDown)
        {
            MoveDown();
        }               
    }
    private void MoveDown()
    {
        var pos = _spawnedObject.transform.position;
        pos.y--;
        _spawnedObject.transform.position = pos;
    }
    private List<Vector2> HorizontalPreview(float valueX)
    {
        List<Vector2> _changedPos = new List<Vector2>();

            foreach(GameObject _childObj in _listOfCurrentChildsOfSpawnObject)
            {
                float x = _childObj.transform.position.x;
                float y = _childObj.transform.position.y;

                x += valueX;

                var endPos = new Vector2(x, y);
                _changedPos.Add(endPos);
            }

            return _changedPos;
    }
    private List<Vector2> VerticalPreview()
    {
        List<Vector2> _changedPos = new List<Vector2>();

            foreach(GameObject _childObj in _listOfCurrentChildsOfSpawnObject)
            {
                float x = _childObj.transform.position.x;
                float y = _childObj.transform.position.y;

                y += -1;

                var endPos = new Vector2(x, y);
                _changedPos.Add(endPos);
            }

            return _changedPos;
    }
    private bool IsInsideHor(List<Vector2> _previewedVersionList)
    {
        //Spawn olan objenin içerisindeki objelerden birinin previewed pozisyonunun sınırların sağından veya solundan çıkıp çıkmadığının kontrolü
        foreach (Vector2 item in _previewedVersionList)
        {
            float x = item.x;
            float y = item.y;
            var pos = new Vector2(x,y);

            //Genel olarak çerçeve dışına çıkmaması için
            if(x < -4.5f || x > 4.5f)
            {
                return false;
            }
              
            //Eğer sağında veya solunda bir obje var ise onun içine girmemesi için, yani sağında veya solunda obje olduğunda false değer döndürerek objenin hareketini limitleyecek.
            foreach (Vector2 objePos in _dogrulukMan.Keys)
            {
                var obj = _dogrulukMan[objePos];
                if((Vector2)obj.transform.position == pos)
                {
                    return false;
                }                
            }       
        }       
        return true;
    }
    private bool IsInsideDown(List<Vector2> _previewedVersionList)
    {
        //Spawn olan objenin içerisindeki objelerden birinin previewed pozisyonunun sınırların aşağısından çıkıp çıkmadığının kontrolü
        foreach (Vector2 item in _previewedVersionList)
        {
            float x = item.x;
            float y = item.y;
            var pos = new Vector2(x, y);
            
            //Genel olarak çerçevenin aşağıdan dışına çıkmaması için
            if(y < -9.5f )
            {
                return false;
            }
              
            //Eğer altında bir obje var ise onun içine girmemesi için, yani eğer altında obje olduğunda false döndürerek objenin hareketini limitleyecek.
            foreach (Vector2 objePos in _dogrulukMan.Keys)
            {
                var obj = _dogrulukMan[objePos];
                if((Vector2)obj.transform.position == pos)
                {
                    return false;
                }                
            }    
        }
        
        return true;
    }  
    private void AddPlacedObjects(GameObject obj)
    {        
        //Sahnedeki tüm objeleri bir listeye ekleme.
        foreach (Transform _childObj in obj.transform)
        {
            var pos = FitPossesIntoGrid(_childObj.position);
            if(!_dogrulukMan.ContainsKey(pos))//değerleri değiştirilen
            {
                _dogrulukMan.Add(pos, _childObj.gameObject);//ilki değerleri değiştirilen ikincisi asıl pos 

                _GridExe[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)] = true;   
                // Debug.Log("Normal: " + pos.x + ", " + pos.y);
                // Debug.Log("Weird:" + Mathf.RoundToInt(pos.x) + ", " + Mathf.RoundToInt(pos.y));
            }            
        } 
        //Obje bırakıldıktan sonra parentından ayrılıp sade obje olarak sahnede durması.
        // for (int i = 3; i >= 0; i--)
        // {
        //     obj.transform.GetChild(i).transform.parent = null;
        // }
        // Destroy(obj);
    } 
    private Vector2 FitPossesIntoGrid(Vector2 item) //Objelerin pozisyonlarını grid'e uygun hale getirir.
    {
            float x = item.x;
            float y = item.y;

            if(x < 0)
            {
                x = x - 0.5f;
                x = x + 5;
            }
            else if(x > 0)
            {
                x = x + 0.5f;
                x = x + 4;
            }
            if(y < 0)
            {
                y = y - 0.5f;
                y = y + 10;
            }
            else if(y > 0)
            {
                y = y + 0.5f;
                y = y + 9;
            }

            return new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }
     private void CheckIsThereACompleteLine()
    {       
        //Bir satırın komple silinip silinmeyeceğine belirtir. 
        for (int y = 0; y < _GridExe.GetLength(1); y++)
        {   
            bool isCompleteLine = true;
            for (int x = 0; x < _GridExe.GetLength(0); x++)
            {
                if(!_GridExe[x, y])
                {  
                    isCompleteLine = false;
                    break;                 
                }
            }
            if(isCompleteLine)
            {
                #region Obje Silme
                for (int x = 0; x < _GridExe.GetLength(0); x++)
                {
                    if(_dogrulukMan.ContainsKey(new Vector2(x,y)))
                    {
                        Destroy(_dogrulukMan[(new Vector2(x,y))].gameObject);
                        _dogrulukMan.Remove(new Vector2(x,y));

                        _GridExe[x, y] = false;  
                    }
                     
                }
                #endregion
                #region Kalan Obleri 1br aşağıya indirme
                for (int y2 = y; y2 < _GridExe.GetLength(1); y2++)
                {
                    for (int x2 = 0; x2 < _GridExe.GetLength(0); x2++)
                    {
                        int degerX = Mathf.RoundToInt(x2);
                        int degerY = Mathf.RoundToInt(y2);
                        if(_GridExe[degerX, degerY])
                        {
                            if(_dogrulukMan.ContainsKey(new Vector2(degerX, degerY)))
                            {
                            var obj = _dogrulukMan[new Vector2(degerX, degerY)];
                            var pos = obj.transform.position;
                            pos.y -= 1;
                            obj.transform.position = pos;
                            _dogrulukMan.Remove(new Vector2(degerX, degerY));
                            _GridExe[degerX, degerY] = false;
                            _dogrulukMan.Add(new Vector2(degerX, degerY-1), obj);
                            _GridExe[degerX, degerY-1] = true;
                            }
                            
                        }
                    }
                }
                ScoreAndExtras.OnLineDestroyed();                
                #endregion
            }
            
        }
    }
}
