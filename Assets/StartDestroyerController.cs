using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDestroyerController : MonoBehaviour
{
    public int scaleFactor = 2;
    private Vector3 _maxScale;

    // Start is called before the first frame update
    void Start()
    {
        _maxScale = new Vector3(10000, 10000, 10000);
        StartCoroutine(waitAndScaleUp());
    }

    private IEnumerator waitAndScaleUp ()
    {
        Debug.Log("Waiting And Scaling");
        yield return new WaitForSeconds(5);

        while (transform.localScale.x < _maxScale.x)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(scaleFactor, scaleFactor, scaleFactor));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
