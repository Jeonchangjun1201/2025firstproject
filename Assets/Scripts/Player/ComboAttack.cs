using UnityEngine;

[System.Serializable]
public class AttackData
{
    public Vector2 offset;
    public Vector2 size;
    public float damage;
}

public class ComboAttack : MonoBehaviour
{
    public AttackData[] comboAttacks;
    public LayerMask enemyLayer;

    [Header("Sword Swing Sounds")]
    public AudioClip[] swordSwingSounds;
    public AudioSource swordAudioSource; // Inspector에서 할당

    void Start()
    {
        if (swordAudioSource == null)
        {
            swordAudioSource = gameObject.AddComponent<AudioSource>();
            swordAudioSource.playOnAwake = false;
        }
    }

    // 애니메이션 이벤트에서 호출 (파라미터로 콤보 인덱스 전달)
    public void DealComboDamage(int comboIndex)
    {
        if (comboIndex < 0 || comboIndex >= comboAttacks.Length) return;

        // 칼 휘두르는 소리 재생
        PlaySwordSound(comboIndex);

        AttackData data = comboAttacks[comboIndex];
        if (data.damage <= 0) return;

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 center = (Vector2)transform.position
                         + new Vector2(data.offset.x * direction, data.offset.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, data.size, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(data.damage);

            var boss = hit.GetComponent<BossHealth>();
            if (boss != null) boss.TakeDamage(data.damage);

            var iceBoss = hit.GetComponent<IceBossHealth>();
            if (iceBoss != null) iceBoss.TakeDamage(data.damage);

            var rockBoss = hit.GetComponent<RockBossHealth>();
            if (rockBoss != null) rockBoss.TakeDamage(data.damage);

            var windBoss = hit.GetComponent<WIndBossHealth>();
            if (windBoss != null) windBoss.TakeDamage(data.damage);
        }
    }

    private void PlaySwordSound(int comboIndex)
    {
        if (swordSwingSounds == null || swordSwingSounds.Length == 0 || swordAudioSource == null) return;
        int index = comboIndex % swordSwingSounds.Length;
        AudioClip clip = swordSwingSounds[index];
        if (clip != null)
        {
            swordAudioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (comboAttacks == null) return;
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        for (int i = 0; i < comboAttacks.Length; i++)
        {
            AttackData data = comboAttacks[i];
            Vector2 center = (Vector2)transform.position
                             + new Vector2(data.offset.x * direction, data.offset.y);
            Gizmos.color = Color.Lerp(Color.red, Color.green, i / (float)(comboAttacks.Length - 1));
            Gizmos.DrawWireCube(center, data.size);
        }
    }
}
