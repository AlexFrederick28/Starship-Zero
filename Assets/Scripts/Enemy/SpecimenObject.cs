using System.Collections;
using UnityEngine;

public class SpecimenObject : MonoBehaviour, ICollectable
{
    public InventoryItemPackage specimenType;
    public SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        PlayerBase.OnPressingRetryOrRespawn += AddSpecimenBackToSpawnPool;
    }

    private void OnDisable()
    {
        PlayerBase.OnPressingRetryOrRespawn -= AddSpecimenBackToSpawnPool;
    }

    public void AddSpecimenBackToSpawnPool()
    {
        Spawning.instance.specimenPool.AddToPool(this);
    }

    public IEnumerator Collect()
    {
        while (gameObject.activeSelf == true)
        {
            // once the object has been collected and turn off, the coroutine will stop
            Vector3 targetPosition = (GameState.instance.playerTransform.position - transform.position).normalized;
            transform.position += targetPosition * GameState.instance.playerInventory.collectionSpeed * Time.deltaTime;

            yield return null;
        }
    }
}
