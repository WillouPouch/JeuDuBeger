﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPC
{
    public class HandleShooting : MonoBehaviour
    {

        StateManager states;
        public Animator weaponAnim;
        public float fireRate;
        float timer;
        public Transform bulletSpawnPoint;
        public GameObject smokeParticle;
        public ParticleSystem[] muzzle;
        public GameObject casingPrefab;
        public Transform caseSpawn;

        public int curBullets = 3;
        public bool infiniteBullets = true;


        bool shoot;
        bool dontShoot;
        bool emptyGun;


        // Use this for initialization
        void Start()
        {
            states = GetComponent<StateManager>();
            timer = 1;
        }

        // Update is called once per frame
        void Update()
        {
            shoot = states.shoot;
            if (shoot && states.alive)
            {
                if (timer <= 0)
                {

                    states.canShoot = true;
                    if (curBullets > 0 || infiniteBullets)
                    {
                        emptyGun = false;

                        StartCoroutine(NoAimShoot());
                        weaponAnim.SetBool("Shoot", false);
                        states.anim.SetTrigger("Fire");
                        curBullets--;
                    }
                    else
                    {
                        if (emptyGun)
                        {
                            //states.handleAnim.StartReload();
                            curBullets = 3;
                        }
                        else
                        {
                            //states.audioManager.PlayEffect("empty_gun");
                            emptyGun = true;
                        }
                    }
                    timer = fireRate;
                }
                else
                {
                    weaponAnim.SetBool("Shoot", true);
                    timer -= Time.deltaTime;
                    states.canShoot = false;
                }
            }
            else
            {
                timer -= Time.deltaTime;
                //weaponAnim.SetBool("Shoot", false);
            }
        }


        IEnumerator NoAimShoot()
        {
            if (!states.aiming)
                states.shooting = true;
            yield return new WaitForSeconds(0.3f);
            //states.audioManager.PlayGunSound();
            states.shooting = false;
            //GameObject go = Instantiate(casingPrefab, caseSpawn.position, caseSpawn.rotation) as GameObject;
            //Rigidbody rb = go.GetComponent<Rigidbody>();
            //rb.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
            //rb.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);

            for (int i = 0; i < muzzle.Length; i++)
            {
                muzzle[i].Emit(1);
            }
            RaycastShoot();
        }

        void RaycastShoot()
        {
            Vector3 direction = states.lookHitPosition - bulletSpawnPoint.position;
            RaycastHit hit;
            if (Physics.Raycast(bulletSpawnPoint.position, direction, out hit, 100, states.layerMask))
            {
                GameObject go = Instantiate(smokeParticle, hit.point, Quaternion.identity) as GameObject;
                go.transform.LookAt(bulletSpawnPoint.position);

                if (hit.transform.GetComponent<WolfHealth>())
                {
                    hit.transform.GetComponent<WolfHealth>().takeDamage(20);
                }
            }
        }
    }
}