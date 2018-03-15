﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script;
using Assets.Script.Managers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Enclosures
{


    public class EnclosureScript : MonoBehaviour
    {

        public int GoldReward = 1; // depend of the distance of the enclos
        public GameObject SmokeEffect;
        public delegate void OnDead();
        public OnDead OnTriggerDead;
        public GameObject SheepPrefab;
        public float Distance;
        private bool _isDisplayingPanel = false;
        public GameObject PinkSuperSheepPrefab;
        public Material pinkFence;


    private GameManager _gameManager;

        private int _sheepNumber;
    private int _nbSuperSheep;
    public int SheepNumber
        {
            get { return _sheepNumber; }
            set
            {
                EnclosureManager.SheepNumberInTheWorld = EnclosureManager.SheepNumberInTheWorld - _sheepNumber + value;
                _sheepNumber = value;
            }
        }

        private float _health;
        public float Health
        {
            get { return _health; }
            set
            {

                if (value < 0)
                {
                    value = 0;
                }
                if (value > 100)
                {
                    return;
                }
                if (_health > value && (_health % 10) - (_health - value) <= 0 && _health != 0)
                {
                    int sheepToRemove = SheepNumber - (int) Math.Truncate((double) value / 10);
                    for (int i = 0; i < sheepToRemove; i++)
                        KillSheep();
                }
                if (_health < value && (_health % 10) + (value - _health) >= 10 && value != 0)
                {
                    int sheepToAdd = (int) Math.Truncate((double) value / 10) - SheepNumber;
                    for (var i = 0; i < sheepToAdd; i++)
                    {
                        AddSheep();
                    }
                }
                _health = value;
            }
        }


        private void Awake()
        {
        }
        private void Start()
        {
            Distance = Vector3.Distance(EnclosureManager.HousePosition, this.transform.position);
            _gameManager = GameManager.instance;
        }


    void Update()
        {
            ShowPanel();
        }

        private void ShowPanel()
        {
            Vector3 distPlayertoEnclos = TerrainTest.PlayerGameObject.transform.position - transform.position;

            if (distPlayertoEnclos.magnitude < 25 && _gameManager.IsTheSunAwakeAndTheBirdAreSinging)
            {
                if (!EnclosureManager.EnclosurePannel.activeSelf)
                {
                    EnclosureManager.EnclosurePannel.transform.GetChild(1).GetComponent<Text>().text =
                        SheepNumber.ToString();
                    EnclosureManager.EnclosurePannel.SetActive(true);
                    _isDisplayingPanel = true;
                }
                HandleInputs();
            }
            else
            {
                if (EnclosureManager.EnclosurePannel.activeSelf && _isDisplayingPanel)
                {
                    _isDisplayingPanel = false;
                    EnclosureManager.EnclosurePannel.SetActive(false);
                }
            }
        }
        private void HandleInputs()
        {

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Health += 10;
                EnclosureManager.EnclosurePannel.transform.GetChild(1).GetComponent<Text>().text =
                    SheepNumber.ToString();
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (Health != 0) _gameManager.TakeSheep();
                Health -= 10;
                EnclosureManager.EnclosurePannel.transform.GetChild(1).GetComponent<Text>().text =
                    SheepNumber.ToString();
            }
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                if (_nbSuperSheep < 1 && _gameManager.TotalSuperSheeps == 1)
                {
                    AddPinkSuperSheep();
                    _nbSuperSheep++;
                    _health += 50;
                    _gameManager.PlaceSuperSheep();
                }
            }

    }

    public void DamageEnclos(float degats)
        {
            Health -= degats;
            if (Health == 0)
            {
                if (_nbSuperSheep < 1 && _gameManager.TotalSuperSheeps == 1)
                {
                    AddPinkSuperSheep();
                    _nbSuperSheep++;
                    _health += 50; 
                    _gameManager.PlaceSuperSheep();
                }
            }
        }


        public void AddSheep()
        {
            if (_gameManager.TotalSheeps <= 0)
                return;
            if (SheepNumber < 10)
            {
                SheepNumber++;
            var sheep = Instantiate(SheepPrefab, this.transform); //crée un clone mouton
            sheep.transform.position = this.transform.position; //le place dans l'enclos
            sheep.transform.Rotate(0, Random.Range(0, 360), 0); //l'oriente d'une façon aléatoire

        }
        _gameManager.PlaceSheep();
        }

        public void AddPinkSuperSheep()
        {
            var sheep = Instantiate(PinkSuperSheepPrefab, this.transform); //crée un clone mouton
            sheep.transform.position = this.transform.position; //le place dans l'enclos
            sheep.transform.Rotate(0, Random.Range(0, 360), 0);

            foreach (Transform child in transform)
            {
                if (child.tag == "Fences")
                {
                    Renderer r = child.GetComponent<Renderer>();
                    r.material = pinkFence;
                }

            }
        }

    public void KillSheep()
        {
            if (SheepNumber > 0)
            {
                var sheepClone = transform.GetChild(transform.childCount - SheepNumber).gameObject;
                GameObject boom = CFX_SpawnSystem.GetNextObject(SmokeEffect);
                boom.transform.position = sheepClone.transform.position;
                Destroy(sheepClone);
                SheepNumber--;
            }
        }
        public void KillAllSheep()
        {
            Health = 0;
        }
        public void DamageEnclos(int degats)
        {
            Health -= degats;
            if (Health == 0)
            {
                Health = 0; //santé min
                Debug.Log("Enclos Mort");
                OnTriggerDead.Invoke();
                OnTriggerDead = null; //On reset le delegate
            }

        }

        public void AddSubscriber(OnDead function)
        {
            OnTriggerDead += function;
        }
        public void RemoveSubscriber(OnDead function)
        {
            if (function != null)
            {
                OnTriggerDead -= function;
            }
        }

    }
}

