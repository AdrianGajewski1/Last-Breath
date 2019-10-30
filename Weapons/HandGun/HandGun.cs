﻿using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(AudioSource))]
public class HandGun : Weapon
{
    float timeToFireAllowed;
    
    private void Awake()
    {
        camera = Camera.main;
        AudioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<Animator>();
        CurrentWeaponPosition = this.gameObject.transform.parent.position;
        OnShot += HandleShooting;
    }

    private void HandleShooting(RaycastHit hit)
    {
        var zombie = hit.transform.GetComponentInParent<ZombieHealth>();

        if (zombie != null)
        {
            if (hit.transform.CompareTag("ZombieHead"))
                zombie.GiveDamage(Damage * 2);
            else
                zombie.GiveDamage(Damage);

            if (zombie.transform.CompareTag("Zombie"))
                Instantiate(vfx[0], hit.point, Quaternion.identity);
        }
        else
        {
            if(hit.point != null)
                Instantiate(vfx[3], hit.point, Quaternion.identity);

        }
    }

    void DestroyParticles()
    {
        Destroy(GameObject.Find("Blood(Clone)"),1);
        Destroy(GameObject.Find("BulletImpact(Clone)"),1);
    }
    public override void Aim()
    {
        animator.SetBool("IsAiming", InputController.RightMouse);
    }

    void Update()
    {
        if(CheckIfCanFire(ref timeToFireAllowed, rateOfFire))
        {
            vfx[1].Play();
            vfx[2].Play();
            Shot();     
        }
        
        animator.SetBool("IsShooting", canFIre);
        animator.SetBool("IsRunning", !FirstPersonController.IsWalking);
        Aim();
        DestroyParticles();
    }
}
