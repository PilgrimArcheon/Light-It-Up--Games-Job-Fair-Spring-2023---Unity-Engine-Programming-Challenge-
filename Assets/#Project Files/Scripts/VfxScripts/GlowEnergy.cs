using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowEnergy : MonoBehaviour
{
    public Animator anim;
    public bool glown;    
    public float delay;
    public bool canGlow;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateEnergyGlow()
    {
        if(!canGlow)
        {
            canGlow = true;
            if(glown)
            {
                Invoke("Remove", delay);
            }
            else
            {
                Invoke("Glow", delay);
            }
        }

        anim.SetBool("canGlow", canGlow);
    }

    void Glow()
    {
        anim.SetTrigger("glow");
        glown = true;
        canGlow = false;
    }

    void Remove()
    {
        anim.SetTrigger("remove");
        glown = false;
    }
}
