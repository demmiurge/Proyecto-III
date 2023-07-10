using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    [Header("Particle effects list")]
    [SerializeField] private GameObject generationOfALargeBubble;
    [SerializeField] private GameObject destructionOfALargeBubble;
    [SerializeField] private GameObject impactAgainstAnEnemy;
    [SerializeField] private GameObject impactAgainstAnInvalidSurface;
    [SerializeField] private GameObject enemyHurt;
    [SerializeField] private GameObject enemyDie;
    [SerializeField] private GameObject enemyShoot;

    public void GenerateParticlesForTheBigBubble(Vector3 desiredPosition, Vector3 surfaceNormal)
    {
        Instantiate(generationOfALargeBubble, desiredPosition, Quaternion.LookRotation(surfaceNormal));
    }

    public void GenerateDestructionParticlesForTheLargeBubble(Vector3 desiredPosition)
    {
        Instantiate(destructionOfALargeBubble, desiredPosition, Quaternion.identity);
    }

    public void GenerateParticlesForHitAnEnemy(Vector3 desiredPosition)
    {
        Instantiate(impactAgainstAnEnemy, desiredPosition, Quaternion.identity);
    }

    public void GenerateParticlesForInvalidSurface(Vector3 desiredPosition, Vector3 surfaceNormal)
    {
        Instantiate(impactAgainstAnInvalidSurface, desiredPosition, Quaternion.LookRotation(surfaceNormal));
    }

    public void GenerateParticlesEnemyHurt(Vector3 desiredPosition)
    {
        Instantiate(enemyHurt, desiredPosition, Quaternion.identity);
    }

    public void GenerateParticlesEnemyDie(Vector3 desiredPosition)
    {
        Instantiate(enemyDie, desiredPosition, Quaternion.identity);
    }

    public void GenerateParticlesEnemyShoot(Vector3 desiredPosition)
    {
        Instantiate(enemyShoot, desiredPosition, Quaternion.identity);
    }
}
