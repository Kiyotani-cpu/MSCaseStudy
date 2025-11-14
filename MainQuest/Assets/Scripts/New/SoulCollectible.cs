using UnityEngine;
using System.Collections;

public class SoulCollectible : MonoBehaviour
{
    [Header("Soul Settings")]
    public float floatHeight = 1f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 90f;
    public float collectionSpeed = 3f;
    public float collectionRange = 1.5f;

    [Header("Visual Effects")]
    public Light soulLight;
    public GameObject cardIndicator;
    public GameObject xpIndicator;
    
    private Card cardDrop;
    private int xpAmount;
    private DropType dropType = DropType.XP;
    private Renderer soulRenderer;

    private enum DropType { XP, Card }

    void Start()
    {
        soulRenderer = GetComponent<Renderer>();
        StartCoroutine(FloatAnimation());
        UpdateVisuals();
    }

    public void SetCardDrop(Card card)
    {
        cardDrop = card;
        dropType = DropType.Card;
        UpdateVisuals();
    }

    public void SetXPDrop(int xp)
    {
        xpAmount = xp;
        dropType = DropType.XP;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (cardIndicator != null) 
            cardIndicator.SetActive(dropType == DropType.Card);
        if (xpIndicator != null) 
            xpIndicator.SetActive(dropType == DropType.XP);

        if (soulLight != null)
        {
            soulLight.color = dropType == DropType.Card ? Color.cyan : Color.green;
        }
    }

    void Update()
    {
        if (collectionRange <= 0) return;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= collectionRange)
        {
            StartCoroutine(CollectSoul(player.transform));
        }
    }

    IEnumerator FloatAnimation()
    {
        float time = 0f;
        Vector3 basePosition = transform.position;

        while (true)
        {
            time += Time.deltaTime * floatSpeed;
            float yOffset = Mathf.Sin(time) * floatHeight;
            transform.position = basePosition + new Vector3(0, yOffset, 0);
            yield return null;
        }
    }

    IEnumerator CollectSoul(Transform player)
    {
        float t = 0f;
        Vector3 startPosition = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * collectionSpeed;
            transform.position = Vector3.Lerp(startPosition, player.position, t);
            yield return null;
        }

        ApplySoulToPlayer();
        Destroy(gameObject);
    }

    void ApplySoulToPlayer()
    {
        if (dropType == DropType.Card)
        {
            PlayerCardManager cardManager = FindObjectOfType<PlayerCardManager>();
            if (cardManager != null && cardDrop != null)
            {
                cardManager.AddCard(cardDrop);
                Debug.Log($"Collected card: {cardDrop.cardName}");
            }
        }
        else
        {
            PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddXP(xpAmount);
                Debug.Log($"Gained {xpAmount} XP!");
            }
        }
    }
}