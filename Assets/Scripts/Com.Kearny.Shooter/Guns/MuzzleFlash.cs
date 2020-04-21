using UnityEngine;

namespace Com.Kearny.Shooter.Guns
{
    public class MuzzleFlash : MonoBehaviour
    {
        public GameObject flashHolder;

        public float flashTime;
        private void Start()
        {
            Deactivate();
        }

        public void Activate()
        {
            flashHolder.SetActive(true);
        
            Invoke(nameof(Deactivate), flashTime);
        }

        public void Deactivate()
        {
            flashHolder.SetActive(false);
        }
    }
}
