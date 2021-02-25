﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Service;
using InGame;

namespace Manager
{
    public class StageManager : Singleton<StageManager>
    {
        int currentStage;

        [SerializeField] float warpDelay = 1f;
        [SerializeField] int[] warpIndex;
        [SerializeField] int infinityValue = 50;
        public Stage[] stages;
        public Transform[] spawnPositionsA;
        public Transform[] spawnPositionsB;
        public float healthRate = 1f;
        public Transform[] warps;
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (ModeManager.Instance.modeLevel == 0)
                healthRate = 0.8f;
            if (ModeManager.Instance.modeLevel == 2)
                healthRate = 1.2f;

            SetWarpTrigger();
            ReviseStage();
            EventManager.Instance.onClearLevel += SetRandomPosition;
            EventManager.Instance.onClearLevel += SetWarpTrigger;
            EventManager.Instance.onStartLevel += StartStageTrigger;
            EventManager.Instance.onClearLevel += ReviseStage;

            if (ModeManager.Instance.modeLevel == 3)
            {
                SetRandomPosition();
                EventManager.Instance.onClearLevel -= ReviseStage;
                EventManager.Instance.onClearLevel += ReviseStage_Infinity;
            }
        }

        public void SetRandomPosition()
        {
            for (int i = 0; i < warps.Length; i++)
            {
                int randomIndex = Random.Range(i, warpIndex.Length);
                int temp = warpIndex[randomIndex];
                warpIndex[randomIndex] = warpIndex[i];
                warpIndex[i] = temp;
            }

            for (int i = 0; i < warps.Length; i++)
            {
                if (i % 2 == 0)
                {
                    warps[i].SetParent(spawnPositionsA[warpIndex[i]]);
                    warps[i].position = spawnPositionsA[warpIndex[i]].position;
                    warps[i].localRotation = Quaternion.Euler(0, 0, 90f);
                }
                else
                {
                    warps[i].SetParent(spawnPositionsB[warpIndex[i]]);
                    warps[i].position = spawnPositionsB[warpIndex[i]].position;
                    warps[i].localRotation = Quaternion.Euler(0, 0, 90f);
                }
            }
        }


        public void SetWarpTrigger()
        {
            StopAllCoroutines();
            StartCoroutine(CSetWarpTrigger());
        }
        void StartStageTrigger()
        {
            stages[currentStage].StartStage();
        }
        void ReviseStage() => currentStage = LevelManager.Instance.level;

        void ReviseStage_Infinity()
        {
            currentStage = 0;
            stages[currentStage].health += infinityValue+ LevelManager.Instance.level*3;
        }
        IEnumerator CSetWarpTrigger()
        {
            for (int i = 0; i < warps.Length; i++)
                warps[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(warpDelay * 2);
            for (int i = 0; i < warps.Length; i++)
            {
                yield return new WaitForSeconds(warpDelay);
                warps[i].gameObject.SetActive(true);
            }
            EventManager.Instance.onWarpSettingInvoke();
            LevelManager.Instance.currentState = LevelManager.LevelState.Ready;
        }
    }
}
