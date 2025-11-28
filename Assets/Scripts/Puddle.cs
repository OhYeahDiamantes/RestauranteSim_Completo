using UnityEngine;
using System.Collections;

public class Puddle : MonoBehaviour, IInteractable
{
    //Sprites
    public Sprite normalSprite;      //Estado 0
    public Sprite mediumDirtySprite; //Estado 1
    public Sprite dirtySprite;       //Estado 2

    [Header("Configuración de Puntos")]
    public int baseCleanReward = 20;

    [Header("Configuración de Suciedad Automática")]
    public float minTimeToIncreaseDirt = 10f;
    public float maxTimeToIncreaseDirt = 20f;

    private SpriteRenderer sr;
    private int dirtyState = 0; //0: Normal, 1: Medio, 2: Sucio

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            enabled = false;
            return;
        }
        UpdateSprite();
        StartCoroutine(DirtinessRoutine());
    }

    IEnumerator DirtinessRoutine()
    {
        while (dirtyState < 2)
        {
            float waitTime = Random.Range(minTimeToIncreaseDirt, maxTimeToIncreaseDirt);
            yield return new WaitForSeconds(waitTime);

            if (dirtyState < 2)
            {
                IncreaseDirtiness();
            }
        }
    }

    public void Interact(GameObject interactor)
    {
        if (!enabled) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPuddleSound();
        }

        int pointsToGive = 0;

        if (dirtyState == 0) //Normal (Limpio)
        {
            pointsToGive = baseCleanReward;
            Debug.Log($"Limpiando charco normal: +{pointsToGive} dinero.");
            GameManager.Instance.AddMoney(pointsToGive);
            PerformDestruction();
        }
        else if (dirtyState == 1) //Medio Sucio
        {
            pointsToGive = baseCleanReward / 2;
            Debug.Log($"Limpiando charco medio sucio: +{pointsToGive} dinero.");
            GameManager.Instance.AddMoney(pointsToGive);
            dirtyState--;
            UpdateSprite();
        }
        else if (dirtyState == 2) //Muy Sucio
        {
            pointsToGive = 1;
            Debug.Log($"Limpiando charco muy sucio: +{pointsToGive} dinero.");
            GameManager.Instance.AddMoney(pointsToGive);
            dirtyState--;
            UpdateSprite();
        }
    }

    void PerformDestruction()
    {
        GameManager.Instance.RecordPuddleCleaned();
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void IncreaseDirtiness()
    {
        if (dirtyState < 2)
        {
            dirtyState++;
            UpdateSprite();
        }
        else
        {
            StopAllCoroutines();
        }
    }

    void UpdateSprite()
    {
        if (sr == null) return;
        switch (dirtyState)
        {
            case 0: sr.sprite = normalSprite; break;
            case 1: sr.sprite = mediumDirtySprite; break;
            case 2: sr.sprite = dirtySprite; break;
        }
    }
}