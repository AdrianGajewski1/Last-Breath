﻿using LB.GameMechanics;
using UnityEngine;

namespace LB.Player
{
    public class PlayerStats : MonoBehaviour
    {
        public class PlayerLevel
        {
            public PlayerLevel(PlayerStatsSaveModel model)
            {
                level = model.CurrentPlayerLevel;
                currentEXP = model.CurrentPlayerExperience;
                expToReach = 20;
            }
            
            public int level;
            public float expToReach;
            public float currentEXP;
            public bool LevelFinished()
            {
                return expToReach <= currentEXP;
            }
        }
        public static PlayerStats Singleton;

        private float m_Score;
        public float Score { get => m_Score; set => m_Score = value; }

        private int m_KilledZombies;
        public int KilledZombies { get => m_KilledZombies; set => m_KilledZombies = value; }

        private float m_ExperiencePoint;
        public float ExperiencePoint { get => m_ExperiencePoint; }


        private PlayerLevel m_CurrentPlayerLevel;
        public PlayerLevel CurrentPlayerLevel { get => m_CurrentPlayerLevel; }

        public void SetPlayerCurrentLevel(int v) => m_CurrentPlayerLevel.level = v; 
        public void SetPlayerEXP(float v) => m_ExperiencePoint = v;

        private void Awake()
        {

            m_CurrentPlayerLevel = new PlayerLevel(SaveSystem.LoadPlayerStats());
            Singleton = this;
        }

        public void AddEXP(float amount)
        {
            m_ExperiencePoint += amount;
        }

        private void Update()
        {
            m_CurrentPlayerLevel.currentEXP = m_ExperiencePoint;
            IncreasePlayerLevel();
        }

        void IncreasePlayerLevel()
        {
          if(m_CurrentPlayerLevel.LevelFinished())
            {

                m_CurrentPlayerLevel.level += 1;
                m_CurrentPlayerLevel.expToReach *= 2;
            }

        }

        public void AddScore(float amount)
        {
            m_Score += amount;
        }

        public void AddKilledZombies(int amount)
        {
            m_KilledZombies += amount;
        }

        public int GetCurrentPlayerLevel() => m_CurrentPlayerLevel.level;
    }

}
