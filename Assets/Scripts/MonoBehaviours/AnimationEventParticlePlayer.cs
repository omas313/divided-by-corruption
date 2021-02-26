using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventParticlePlayer : MonoBehaviour
{
    [SerializeField] ParticleSystem _particles;

   public void PlayParticles()
   {
       _particles.Play();
   }
}
