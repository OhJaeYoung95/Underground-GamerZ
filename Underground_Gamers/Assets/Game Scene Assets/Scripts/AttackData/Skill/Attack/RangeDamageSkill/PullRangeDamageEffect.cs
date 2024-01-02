using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullRangeDamageEffect : RangeDamageEffect
{
    private int count = 0;

    private void OnDisable()
    {
        count = 0;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        TeamIdentifier identity = other.GetComponent<TeamIdentifier>();
        CharacterStatus aStatus = controller.GetComponent<CharacterStatus>();
        CharacterStatus dStatus = other.GetComponent<CharacterStatus>();
        AIController dController = other.GetComponent<AIController>();
        if (identity == null)
            return;
        if (other.gameObject.layer == controller.gameObject.layer)
            return;

        if (hitEffectPrefab != null)
        {
            DurationEffect hitEffect = Instantiate(hitEffectPrefab, other.transform.position, hitEffectPrefab.transform.rotation);
            hitEffect.SetOffsetNScale(offsetHitEffect, scaleHitEffect);
            Destroy(hitEffect, durationHitEffect);
        }

        var attackables = other.GetComponentsInChildren<IAttackable>();

        float damage = attack.Damage;
        damage = Utils.GetRandomDamageByAccuracy(damage, aStatus);
        attack.Damage = Mathf.RoundToInt(damage);
        if(count < timing.Length - 1)
        {
            dController.PullByTargetPos(transform.position, timing[count+1] - timing[count]);
            count++;
        }
        else if(count == timing.Length)
        {
            dController.PullByTargetPos(transform.position, timing[1] - timing[0]);
        }
        foreach (var attackable in attackables)
        {
            attackable.OnAttack(controller.gameObject, attack);
        }
    }

}
