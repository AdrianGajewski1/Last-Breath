﻿using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int Damage;
    public int ClipSize;
    public int CurrentAmmoInClip;

    public float rateOfFire;
    public float range;

    public Crosshair Crosshair;

    protected Camera camera;
    protected bool canFIre;

    [HideInInspector]public AudioSource AudioSource;
    [SerializeField] AudioClip gunShotSound;

    public event System.Action<RaycastHit> OnShot;

    public virtual void Shot() 
    {
        RaycastHit hit;

        var direction = camera.transform.forward;

        AudioSource.PlayOneShot(gunShotSound);

        if (Physics.Raycast(gameObject.transform.position, direction, out hit, range))
        {
            OnShot?.Invoke(hit);
        }
    }
}
