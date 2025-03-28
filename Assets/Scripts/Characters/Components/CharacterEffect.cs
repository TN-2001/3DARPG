using System.Collections.Generic;
using UnityEngine;

public class CharacterEffect : MonoBehaviour {
    // パーティクル
    [SerializeField] private ParticleSystem hitParticle = null; // ダメージパーティクル
    [SerializeField] private List<ParticleSystem> particleList = new(); // パーティクルリスト


    public void PlayParticle(int number) {
        particleList[number].Play();
    }

    public void StopParticle(int number) {
        particleList[number].Stop();
    }

    public void PlayInstantiateParticle(int number) {
        GameObject obj = Instantiate(particleList[number].gameObject, particleList[number].transform.position, Quaternion.identity);
        obj.GetComponent<ParticleSystem>().Play();
        Destroy(obj, 30f);
    }

    public void PlayInstantiateParticle_Hit(Vector3 position) {
        GameObject obj = Instantiate(hitParticle.gameObject, position, Quaternion.identity);
        obj.GetComponent<ParticleSystem>().Play();
        Destroy(obj, 30f);
    }
}
