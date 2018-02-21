﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script à placer sur le Particle system enfant du loup aquatique

public class WolfWaterAttack : MonoBehaviour {

    private ParticleSystem waterJet;
    private int damage = 1;
    private Animator anim;


    // Use this for initialization
    void Start () {
        waterJet = gameObject.GetComponentInChildren<ParticleSystem>();
        anim = GetComponentInParent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        
        //temp pour l'attaque
        waterJet.Stop();
        if (Input.GetKey(KeyCode.M))
        {
            anim.SetTrigger("Attack");
            waterJet.Play();
        } 

    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.takeDamage(damage);
        }
    }
}