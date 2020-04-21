using System.Collections;
using UnityEngine;

namespace Com.Kearny.Shooter.Guns
{
    public class Shell : MonoBehaviour
    {
        public Rigidbody myRigidbody;
        public float forceMin;
        public float forceMax;
        private Renderer _renderer;

        private const float Lifetime = 4;
        private const float FadeTime = 2;

        // Start is called before the first frame update
        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            
            var force = Random.Range(forceMin, forceMax);
            myRigidbody.AddForce(transform.right * force);
            myRigidbody.AddTorque(Random.insideUnitSphere * force);

            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(Lifetime);

            float percent = 0;
            const float fadeSpeed = 1 / FadeTime;
            Material material = _renderer.material;
            Color initialColor = material.color;

            while (percent < 1)
            {
                percent += Time.deltaTime * fadeSpeed;
                material.color = Color.Lerp(initialColor, Color.clear, percent);
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}