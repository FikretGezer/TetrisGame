using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    [SerializeField] private GameObject _prefabObject;
    [SerializeField] private float _sec = 1f;
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private LayerMask _maskBorder;

    private Vector3 _lastPos;   
    private Vector3 _beginningPos; 
    public static GameObject _spawnedObject;
    private Vector3 posMiddle, posLeft, posRight;

    private float minY = -9f, maxY = 9f, minX = -4f, maxX = 4f;
    private float _timer;
    private bool doesHitAnObject;
    private bool keepSpawning;
    public bool canMoveLeft, canMoveRight, canMoveDown;
    private float _rotationAmount = -90f;

    private void Start()
    {
        _beginningPos = new Vector3(0f, maxY);
        _lastPos = _beginningPos;
        _spawnedObject = null;
        keepSpawning = canMoveLeft = canMoveRight = canMoveDown = true;
    }
    void Update()
    {
        if(_spawnedObject == null && keepSpawning)
        {
            canMoveLeft = canMoveRight = canMoveDown = true;
            _lastPos = _beginningPos;
            _spawnedObject = Instantiate(_prefabObject, _lastPos, Quaternion.identity);
            _spawnedObject.layer = 0;
        }
        else if((_spawnedObject != null /*&& _spawnedObject.transform.position.y <= minY)*/ && doesHitAnObject))
        {
            doesHitAnObject = false;
            _spawnedObject.layer = 6;
            _rotationAmount = -90f;
            _spawnedObject = null;
        }

        if(canMoveLeft)
        {
            Move(KeyCode.A, new Vector3(-1, 0));
        }
        if(canMoveRight)
        {
            Move(KeyCode.D, new Vector3(1, 0));
        }
        if(canMoveDown)
            Move(KeyCode.S, new Vector3(0, -1));

        if(Input.GetKeyDown(KeyCode.F) && _spawnedObject != null)
        {
            _spawnedObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,_rotationAmount));
            _rotationAmount += -90f;
        }
        
        if(_spawnedObject != null)
        {
            //_spawnedObject.transform.Translate (new Vector3(1,0));// = _lastPos;
            //CheckControl();
            CheckBorder();
           // CheckControlReworked();
        }
    }
    Vector3 _lowestItemPos, _lowestItemOntheLeftPos, _lowestItemOntheRightPos;
    GameObject _lowestItem;
    private void CheckControlReworked()
    {
        _lowestItemPos = _lowestItemOntheLeftPos = _lowestItemOntheRightPos = _spawnedObject.transform.GetChild(0).transform.GetChild(0).transform.position;
        foreach (Transform item in _spawnedObject.transform.GetChild(0).transform)
        {
            if(item.transform.position.y < _lowestItemPos.y)
            {
                _lowestItemPos = item.transform.position;
                _lowestItem = item.gameObject;
            }
            if(item.transform.position.x > _lowestItemOntheRightPos.x)
            {
                _lowestItemOntheRightPos = item.transform.position;
            }
            if(item.transform.position.x < _lowestItemOntheLeftPos.x)
            {
                _lowestItemOntheLeftPos = item.transform.position;
            }


            if(Physics.CheckSphere(item.position, radius, _mask))
            {
                if(item.transform.position == _lowestItemPos)
                {                                  
                    doesHitAnObject = true;  
                }
                if(item.transform.position != _lowestItemOntheLeftPos || item.transform.position != _lowestItemOntheRightPos)
                {
                    doesHitAnObject = true; 
                }
                if(_spawnedObject.transform.position.y >= 9f)
                {
                    keepSpawning = false;
                }
            }
        }
        
    }
    private void CheckBorder()
    {
        foreach (Transform item in _spawnedObject.transform.GetChild(0).transform)
        {
            if(Physics.CheckSphere(item.position, radius, _maskBorder))
            {
                if(item.position.x > 1)
                {
                    canMoveRight = false;
                    canMoveLeft = true;
                }
                else if(item.position.x < -1)
                {
                    canMoveLeft = false;
                    canMoveRight = true;
                }
                else
                {
                    canMoveLeft = canMoveRight = true;
                }
                // if(item.position.y < 0 && item == _lowestItem)
                // {
                //     canMoveDown = false;
                //     doesHitAnObject = true;
                // }
            }
        }
    }
    private void Move(KeyCode k, Vector3 v)
    {
        if(Input.GetKeyDown(k))
        {
            _spawnedObject.transform.Translate (v);
        }
    }
    private void MoveDownObjects(float _sec)
    {
        if(_lastPos.y > minY)
        {
            _timer += Time.deltaTime;

            if(_timer > _sec)
            {
                _timer = 0f;
                _lastPos.y += -1f;
            }
        }
        
    }
    void OnDrawGizmos()
    {
        if(_spawnedObject != null)
        {
            foreach (Transform item in _spawnedObject.transform.GetChild(0).transform)  
            {
                if(Physics.CheckSphere(item.position, radius, _maskBorder))
                {
                    Gizmos.color = Color.green;
                }
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(item.position, radius);
            }
        }
        
    }
}
