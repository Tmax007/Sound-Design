using System.Globalization;
using UnityEngine;

public class BreakableByBomb : MonoBehaviour
{
    public int numberOfDebris = 4;
    public float randomForce = 1.5f;
    public GameObject mesh;
    public GameObject debriPrefab;
    public ParticleSystem debrisParticle;
    public BoxCollider boxCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Break()
    {
        //$Bomb Door Open
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Bomb Door Exploding");
        mesh.SetActive(false);
        Destroy(boxCollider);

        for (int i = 0; i < numberOfDebris; i++)
        {
            GameObject debri = Instantiate(debriPrefab, transform.position, Quaternion.identity);

            debri.transform.position += Vector3.up *  Random.Range(0.3f, 1f);

            debri.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * randomForce, ForceMode.Impulse);
        }

        debrisParticle.Play();
    }
}
