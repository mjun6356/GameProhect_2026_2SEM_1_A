using UnityEngine;
using TMPro;
using System.Collections;

public class AchievementManager : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField]private GameObject achiementPanal;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private float displayTime = 3f;

    private int totalCollected;
    private bool collectorUnlocked;
    private bool healUnlocked;
    private bool dropUnlocked;
    private bool QeustUnlocked;

    private bool firstItemUnlocked;
    private Coroutine popupRoutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (achiementPanal != null) achiementPanal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Unlock(string title, string description)
    {
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
            achiementPanal.SetActive(false);
        }

        popupRoutine = StartCoroutine( ShowPopup(title, description));
    }

    private IEnumerator ShowPopup(string title, string description)
    {
        if (achiementPanal == null || titleText == null || descriptionText== null)
        {
            Debug.LogWarning("AchievementManager의 UI가 연결이 되어 있지 않습니다.");
            yield break;
        }

        achiementPanal.SetActive(true);
        titleText.text = "업적 달성!\n"+title;
        descriptionText.text = description;

        yield return new WaitForSeconds(displayTime);

        achiementPanal
            .SetActive(false);

        popupRoutine = null;


    }

    public void ItemCollected(int amount)
    {
        totalCollected += amount;

        if (firstItemUnlocked == false)
        {
            firstItemUnlocked = true;
            Unlock("첫 수집", "처음으로 아이템을 획득하였습니다. ");
        }

        if(collectorUnlocked == false && totalCollected >= 5)
        {
            collectorUnlocked = true;
            Unlock("초보 수집가", "아이템을 총 5개 획득했습니다. ");
        } 
    }

    public void ItemUsed(string itemName)
    {
        if (healUnlocked) return;

        if(itemName == "물약" || itemName == "회복약" || itemName == "구급키트")
        {
            healUnlocked = true;
            Unlock("건강이 최고", "회복 아이템 사용 했습니다.");
        }
    }

    public void ItemDropped()
    {
        if (dropUnlocked) return;

        dropUnlocked = true;

        Unlock("과감한 정리", "아이템을 처음 버리셨습니다. ");
    }

    public void QuestCompleted()
    {
        if (QeustUnlocked) return;

        QeustUnlocked = true;

        Unlock("퀘스트 해결사", "퀘스트를 처음 완료 했습니다.");
    }







}
