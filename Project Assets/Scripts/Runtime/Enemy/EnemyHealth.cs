using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

	public float dieDelay;

	public bool isDead = false;

    public void TakeDamage(int damage)
	{
		if (currentHealth - damage <= 0f)
		{
			if (!IsInvoking(nameof(DeactivateSelf))) { Invoke(nameof(DeactivateSelf), dieDelay); }
		}
		else
		{
			currentHealth -= damage;
		}
	}

	private void DeactivateSelf()
	{
		gameObject.SetActive(false);
	}
}
