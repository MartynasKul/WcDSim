using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepFryer : MonoBehaviour
{
    public GameObject friesPrefab;
    public Transform basketTransform;
    public Transform SpawnPoint;
    public float cookingTime = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PotatoRaw"))
        {
            Vector3 position = other.transform.position;
            Quaternion rotation = other.transform.rotation;
            Destroy(other.gameObject);
            Instantiate(friesPrefab, SpawnPoint.position, SpawnPoint.rotation);
            //StartCoroutine(FryPotato(other.gameObject));
        }
    }

    private IEnumerator FryPotato(GameObject rawPotato)
    {
        Debug.Log("Frying potato");

        yield return new WaitForSeconds(cookingTime);

        Vector3 position = rawPotato.transform.position;
        Quaternion rotation = rawPotato.transform.rotation;

        Instantiate(friesPrefab, position, rotation);
        Destroy(rawPotato);

        Debug.Log("Potato turned into fries");
    }
}