﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Service;
using InGame;

namespace Manager
{
    public class BlockManager : Singleton<BlockManager>
    {
        enum Direction { None, MoveToX, MoveToY }
        Direction currentDirection = Direction.None;
        Vector2 startTouch;
        Vector2 endTouch;
        Vector2 moveTouch;
        Block[] selectBlocks = new Block[7];
        int blockNumber = 49;
        float dragDistance;

        [SerializeField] Transform field;
        [SerializeField] Block[] blocks;

        public List<Block> animalBlock = new List<Block>();
        public Queue<GameObject> animalObject = new Queue<GameObject>();

        public Block selectBlock { get; private set; }
        public LayerMask blockLayerMask = 1 << 8;
        public float touchSenstive;
        public float moveDelay = 0.001f;
        public int catNumber = -1;
        public int dogNumber = -1;
        public int bearNumber = -1;
        public bool clickAble = true;

        void Update()
        {
            if(LevelManager.Instance.currentState.Equals(LevelManager.LevelState.Ready)&& clickAble)
                DragBlock();
        }
        void DragBlock()
        {
            // Drag.Began
            if (Input.GetMouseButtonDown(0))
            {
                startTouch = Input.mousePosition; // Input.GetTouch(0).position;
                SelectBlock();
            }

            // Drag.Ended
            if (Input.GetMouseButtonUp(0))
            {
                if (selectBlock == null)
                    return;

                endTouch = Input.mousePosition; // Input.GetTouch(0).position;
                if (currentDirection.Equals(Direction.None))
                {
                    if ((endTouch.x - startTouch.x > touchSenstive || endTouch.x - startTouch.x <= -touchSenstive))
                    {
                        if (LevelManager.Instance.walk >= LevelManager.Instance.priceWalk)
                        {
                            LevelManager.Instance.DecreaseWalk();
                            UIManager.Instance.AnimateWalkUI();
                        }
                        else if (LevelManager.Instance.resource >= LevelManager.Instance.priceWalk)
                        {
                            LevelManager.Instance.DecreaseResource(LevelManager.Instance.priceWalk);
                            UIManager.Instance.AnimateResourceUI();
                        }
                        else
                            return;

                        dragDistance = endTouch.x - startTouch.x;
                        SetSelectBlocks(Direction.MoveToX);
                        currentDirection = Direction.MoveToX;
                        dragDistance = Normalize(dragDistance);
                        ChangePositionX(dragDistance);
                        StartCoroutine(CMoveHorizontal(dragDistance));
                    }
                    else if ((endTouch.y - startTouch.y > touchSenstive || endTouch.y - startTouch.y < -touchSenstive))
                    {
                        if (LevelManager.Instance.walk >= LevelManager.Instance.priceWalk)
                        {
                            LevelManager.Instance.DecreaseWalk();
                            UIManager.Instance.AnimateWalkUI();
                        }
                        else if (LevelManager.Instance.resource >= LevelManager.Instance.priceWalk)
                        {
                            LevelManager.Instance.DecreaseResource(LevelManager.Instance.priceWalk);
                            UIManager.Instance.AnimateResourceUI();
                        }
                        else
                            return;

                        dragDistance = endTouch.y - startTouch.y;
                        SetSelectBlocks(Direction.MoveToY);
                        currentDirection = Direction.MoveToY;
                        dragDistance = Normalize(dragDistance);
                        ChangePositionY(dragDistance);
                        StartCoroutine(CMoveVertical(dragDistance));
                    }
                    else
                    {
                        selectBlock.ClickBlock();
                        return;
                    }
                }
                selectBlock = null;
            }
        }

        void SelectBlock()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.collider != null)
                    selectBlock = hit.transform.gameObject.GetComponent<Block>();
            }
        }

        void SetSelectBlocks(Direction direction)
        {
            int blockIndex = 0;
            for (int i = 0; i < blockNumber; i++)
            {
                if (direction.Equals(Direction.MoveToX) && selectBlock.positionY.Equals(blocks[i].positionY)
                    || direction.Equals(Direction.MoveToY) && selectBlock.positionX.Equals(blocks[i].positionX))
                {
                    selectBlocks[blockIndex] = blocks[i];
                    selectBlocks[blockIndex].transform.SetParent(transform);
                    blockIndex++;
                }
            }
        }

        int Normalize(float value)
        {
            if (value >= 0)
                return 1;
            else
                return -1;
        }

        void ChangePositionX(float direction)
        {
            if (direction < 0)
            {
                for (int i = 0; i < selectBlocks.Length; i++)
                {
                    selectBlocks[i].positionX--;
                    if (selectBlocks[i].positionX.Equals(-1))
                    {
                        selectBlocks[i].positionX = selectBlocks.Length - 1;
                        selectBlocks[i].transform.position = new Vector3(selectBlocks.Length, 0, selectBlocks[i].positionY);
                    }
                }
            }
            else
            {
                for (int i = 0; i < selectBlocks.Length; i++)
                {
                    selectBlocks[i].positionX++;
                    if (selectBlocks[i].positionX.Equals(selectBlocks.Length))
                    {
                        selectBlocks[i].positionX = 0;
                        selectBlocks[i].transform.position = new Vector3(-1, 0, selectBlocks[i].positionY);
                    }
                }
            }
        }

        void ChangePositionY(float direction)
        {
            if (direction < 0)
            {
                for (int i = 0; i < selectBlocks.Length; i++)
                {
                    selectBlocks[i].positionY--;
                    if (selectBlocks[i].positionY.Equals(-1))
                    {
                        selectBlocks[i].positionY = selectBlocks.Length - 1;
                        selectBlocks[i].transform.position = new Vector3(selectBlocks[i].positionX, 0, selectBlocks.Length);
                    }
                }
            }
            else
            {
                for (int i = 0; i < selectBlocks.Length; i++)
                {
                    selectBlocks[i].positionY++;
                    if (selectBlocks[i].positionY.Equals(selectBlocks.Length))
                    {
                        selectBlocks[i].positionY = 0;
                        selectBlocks[i].transform.position = new Vector3(selectBlocks[i].positionX, 0, -1);
                    }
                }
            }
        }

        public void AnimalSpawn()
        {
            for (int i = 0; i < animalBlock.Count; i++)
            {
                animalObject.Enqueue(Instantiate(AnimalInformation.Instance.level[animalBlock[i].animalLevel].animalObject[animalBlock[i].animalIndex]
                        , new Vector3(animalBlock[i].positionX, 0, animalBlock[i].positionY), Quaternion.identity));
            }
        }

        IEnumerator CMoveHorizontal(float direction)
        {
            float distance = 10f * direction;
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(moveDelay);
                transform.Translate(1 / distance, 0, 0);
            }
            for (int i = 0; i < selectBlocks.Length; i++)
                selectBlocks[i].transform.SetParent(field);
            currentDirection = Direction.None;
            EventManager.Instance.onMoveInvoke();
        }

        IEnumerator CMoveVertical(float direction)
        {
            float distance = 10f * direction;
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(moveDelay);
                transform.Translate(0, 0, 1 / distance);
            }
            for (int i = 0; i < selectBlocks.Length; i++)
                selectBlocks[i].transform.SetParent(field);
            currentDirection = Direction.None;
            EventManager.Instance.onMoveInvoke();
        }
    }
}