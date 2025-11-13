using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enabler : MonoBehaviour
{
    [SerializeField] internal Transform[] objects;

    public float enableDelay = 2f;

    private Transform _transform;

    private void Update()
    {
		foreach (Transform _object in objects)
		{
            if (!_object.gameObject.activeInHierarchy)
			{
                _transform = _object;

                if (!IsInvoking(nameof(EnableObject)))
				{
                    Invoke(nameof(EnableObject), enableDelay);
                }
			}
		}
    }

    public void EnableObject()
    {
        _transform.gameObject.SetActive(true);

        _transform.GetComponent<EnemyHealth>().currentHealth = _transform.GetComponent<EnemyHealth>().maxHealth;
    }
}
