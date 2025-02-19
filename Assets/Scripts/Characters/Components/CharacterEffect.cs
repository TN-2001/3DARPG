using System.Collections.Generic;
using UnityEngine;

public class CharacterEffect : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleList = new(); // パーティクルリスト

    public void PlayParticle(int number)
    {
        particleList[number].Play();
    }

    public void PlayInstantiateParticle(int number)
    {
        GameObject obj = Instantiate(particleList[number].gameObject, particleList[number].transform.position, Quaternion.identity);
        obj.GetComponent<ParticleSystem>().Play();
        Destroy(obj, 30f);
    }

    public void PlayInstantiateParticle(int number, Vector3 position)
    {
        GameObject obj = Instantiate(particleList[number].gameObject, position, Quaternion.identity);
        obj.GetComponent<ParticleSystem>().Play();
        Destroy(obj, 30f);
    }

    public void StopParticle(int number)
    {
        particleList[number].Stop();
    }
}
