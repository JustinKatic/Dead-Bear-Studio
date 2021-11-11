using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBackAbility : MonoBehaviour
{
    private int IdOfPlayerWhoSpawnedKnockback;
    private float knockBackForce;
    private float knockBackRange;
    private float knockBackUpPos;

    Vector3 startSize;
    Vector3 endSize;
    float lerpTime = 0;

    private void Start()
    {
        startSize = transform.localScale;
        endSize = new Vector3(knockBackRange, knockBackRange, knockBackRange);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(startSize, endSize, lerpTime);
        lerpTime += Time.deltaTime;
        if (transform.localScale.magnitude >= endSize.magnitude - 1)
            Destroy(gameObject);
    }

    public void Init(int IdOfPlayerWhoSpawnedKnockback, float knockBackForce, float knockBackRange, float knockBackUpPos)
    {
        this.IdOfPlayerWhoSpawnedKnockback = IdOfPlayerWhoSpawnedKnockback;
        this.knockBackForce = knockBackForce;
        this.knockBackRange = knockBackRange;
        this.knockBackUpPos = knockBackUpPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerParent"))
        {
            PlayerHealthManager playerHealth = other.GetComponentInParent<PlayerHealthManager>();
            if (playerHealth.PlayerId != IdOfPlayerWhoSpawnedKnockback)
            {
                Vector3 KnockBackPos;
                KnockBackPos = other.transform.position + ((other.transform.position - transform.position).normalized * 5);
                KnockBackPos.y = transform.position.y + knockBackUpPos;
                other.GetComponent<PlayerController>().KnockBack(KnockBackPos, IdOfPlayerWhoSpawnedKnockback, knockBackForce);
            }
        }
    }

}
