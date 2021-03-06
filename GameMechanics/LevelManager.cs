﻿using LB.UI;
using UnityEngine;

namespace LB.GameMechanics
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Singleton;

        public int CurrentLevel;
        public int ZombiesToSpawnNumber = 3;


        Level currentLevel = new Level();

        public int ZombiesCount;

        void Start()
        {
            Singleton = this;

            if(GameManager.Singleton.GameMode == GameMode.Survival)
            {
                currentLevel.ZombiesToSpawnNumber = ZombiesToSpawnNumber;
                CurrentLevel = currentLevel.LevelNumber;
                ZombiesManager.Singleton.SpawnZombies(currentLevel.ZombiesToSpawnNumber);
            }
            
        }

        private void Update()
        {
            if (GameManager.Singleton.GameMode == GameMode.Survival)
                SurvivalMode();
        }

        void SurvivalMode()
        {
            CurrentLevel = currentLevel.LevelNumber;

            if (currentLevel.LevelFinished == true && ZombiesManager.Singleton.gameOver == false)
            {
                StartCoroutine(UIManager.Singleton.PlayLevelFinishedAnimation());
                currentLevel.ZombiesToSpawnNumber += 2;
                currentLevel.IncreaseLevel();
                ZombiesManager.Singleton.SpawnZombies(currentLevel.ZombiesToSpawnNumber);
            }
        }
    }

}

