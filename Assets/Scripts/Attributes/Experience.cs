using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] public float experiencePoints = 0;
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }


        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}
