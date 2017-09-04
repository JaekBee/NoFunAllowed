using UnityEngine;

public class Water : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Gross"))
        {

            other.gameObject.SetActive(false);

        }

    }

}
