using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;
using System;

public class NPCKaira : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TMP_InputField playerInput;
    public Button sendButton;
    private string apiKey = "AIzaSyA0dMB4x2MgtJ4-CrUe5pU3pmDqPenqyz8";
    private string conversationHistory = "";
    private bool isTalking = false;
    private bool hasGivenFullInfo = false;
    private string playerMood = "neutral"; // neutral, friendly, annoyed
    private bool isPlayerInRange = false;
    private bool askedName = false;
    private string playerName = "";
    private int moodLevel = 0; // -2 (annoyed) to +2 (friendly)
    private List<string> suggestedItems = new List<string>(); // Отслеживание советов
    private int frustrationCount = 0; // Счётчик раздражения
    private int retryCount = 0; // Счётчик попыток перезапуска
    private const int MAX_RETRIES = 2; // Максимум попыток
    private bool nameRepeated = false; // Отслеживание повторного вопроса об имени
    private int apologyCount = 0; // Счётчик извинений

    // Списки для разнообразия
    private string[] randomGreetings = { "О, новый друг!", "Привет, странник!", "Рада тебя видеть!" };
    private string[] randomTips = { "Возьми фонарик, Туман коварен!", "С собой воду — путь длинный!", "Свет спасёт от Тумана!" };
    private string[] annoyedResponses = { "Хватит хамить, давай к делу!", "Терпение не бесконечное, говори нормально!", "С меня хватит, вернёмся к Вардену!" };
    private string[] calmResponses = { "Успокойся, я помогу!", "Давай без криков, что нужно?", "Всё нормально, объясни!" };

    // Список грубых слов и неимён
    private string[] rudeWords = {
        "дурак", "идиот", "кретин", "дебил", "придурок", "мудак", "идиотина", "урод", "скотина", "сволочь",
        "подонок", "гад", "тварь", "ублюдок", "засранец", "болван", "осёл", "козёл", "баран", "хам",
        "хуй", "пиздец", "ебать", "хуйня", "дерьмо", "говно", "жопа", "пидор", "сучка", "блядь",
        "еблан", "хер", "пизда", "задница", "очко", "хрень", "хуйло", "пиздеж",
        "черт", "чёрт возьми", "дьявол", "проклятье", "сука", "чертова кукла", "на хрен", "к черту",
        "пошло всё", "аааа", "бери всё", "чёрт подери",
        "лох", "бот", "нуб", "пидрила", "чмо", "шлак", "отстой", "дно", "лузер", "клиника",
        "уёбок", "гнида", "шваль", "быдло", "мразь",
        "иди на хуй", "иди в жопу", "отвали", "пошел вон", "заткнись", "проваливай", "отъебись",
        "не лезь", "уебывай", "вали отсюда", "пиздуй", "сгинь", "убирайся", "закрой варежку", "не суйся",
        "блин", "чертыхаться", "чёрт бы побрал", "зараза", "дьявол тебя забери", "к чёртовой матери",
        "чёрт знает что", "ерунда", "фигня", "бардак"
    };
    private string[] nonNames = { "утюг", "стол", "дверь", "окно", "машина", "кот", "собака", "стул", "лампа", "книга" }; // Примеры неимён

    void Start()
    {
        dialogueText.text = "";
        playerInput.gameObject.SetActive(false);
        sendButton.onClick.AddListener(SendMessage);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isTalking && Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInput.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInRange && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        string greeting = randomGreetings[UnityEngine.Random.Range(0, randomGreetings.Length)];
        conversationHistory = $"Кайра: {greeting} Я Кайра, сбежала из 'Эксперимент-0'. Кристаллы тают из-за саботажа элиты. Варден в Обители Искр, Туман — древняя сила, пробуждённая экспериментами. Как тебя зовут?\n";
        UpdateDialogueText();
        playerInput.gameObject.SetActive(true);
    }

    public void SendMessage()
    {
        string playerMessage = playerInput.text.Trim();
        if (string.IsNullOrEmpty(playerMessage)) return;

        conversationHistory += $"You: {playerMessage}\n";
        UpdateDialogueText("Кайра: Подожди, обдумываю…");

        // Проверка на грубые слова или неимя
        bool isRude = false;
        bool isNonName = false;
        foreach (string rudeWord in rudeWords)
        {
            if (playerMessage.ToLower().Contains(rudeWord))
            {
                isRude = true;
                break;
            }
        }
        foreach (string nonName in nonNames)
        {
            if (playerMessage.ToLower() == nonName)
            {
                isNonName = true;
                break;
            }
        }

        if (isRude || isNonName)
        {
            if (!askedName && string.IsNullOrEmpty(playerName))
            {
                string response = isRude ? "Кайра: Очень смешно, а теперь скажи своё имя без хамства!" : "Кайра: Очень смешно, а теперь скажи своё имя!";
                UpdateMoodAndInfo(playerMessage, response);
            }
            else
            {
                string response = "Кайра: " + (string.IsNullOrEmpty(playerName) ? "Прости, не сердись!" : $"{playerName}, хватит хамить!") + 
                                 " Давай к делу. Ищи Вардена в Обители!";
                UpdateMoodAndInfo(playerMessage, response);
                frustrationCount++;
                if (frustrationCount > 2) moodLevel = Math.Max(moodLevel - 1, -2);
            }
        }
        else if (!askedName && !string.IsNullOrEmpty(playerMessage))
        {
            playerName = playerMessage;
            string response = $"Кайра: Приятно познакомиться, {playerName}! Давай разберёмся с Туманом и Варденом.";
            UpdateMoodAndInfo(playerMessage, response);
            askedName = true;
        }
        else
        {
            playerInput.text = "";
            retryCount = 0;
            StartCoroutine(SendToGemini(playerMessage));
        }
    }

    IEnumerator SendToGemini(string message)
    {
        string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + apiKey;
        string context = "Ты — Кайра, бывшая сотрудница 'Эксперимент-0' в игре 'Limit of the Mist'. Ты саркастична, решительна, но готова помочь. Знаешь о саботаже элиты, истощении кристаллов, экспериментах корпорации, Тумане как древней силе, пробуждённой ими, и Вардене в Обители Искр. Не знаешь о древней цивилизации, сознании Тумана или тайной организации. Отвечай коротко (2-200 символов), дружелюбно, с лёгким сарказмом. Строго следуй сюжету: саботаж элиты, тающие кристаллы, Туман, Варден, Обитель Искр. Ты не знаешь информацию из реальной жизни в игре, не используй её. Если игрок хамит, показывай раздражение после 2 раз. Не упоминай сестру, если игрок не рассказывал. Давай конкретные подсказки (например, 'Ищи Вардена', 'Возьми свет'). Ограничивай извинения 2 раза, затем предлагай шаг.";
        string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"" + context + "\\n" + conversationHistory + message + "\"}]}], \"generationConfig\": {\"maxOutputTokens\": 75, \"temperature\": 0.7}}";
        Debug.Log("Request: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Parsed Response: " + response);
                string kairaResponse = ParseResponse(response);
                if (!kairaResponse.StartsWith("Кайра:")) kairaResponse = "Кайра: " + kairaResponse;
                Debug.Log("Kaira Response: " + kairaResponse);
                UpdateMoodAndInfo(message, kairaResponse);
                retryCount = 0;
            }
            else
            {
                Debug.LogError($"Error: {request.error} (Code: {request.responseCode}), Attempt: {retryCount + 1}");
                if (retryCount < MAX_RETRIES)
                {
                    retryCount++;
                    yield return new WaitForSeconds(1f);
                    yield return SendToGemini(message);
                }
                else
                {
                    string fallbackResponse = "Кайра: Ой, связь пропала! Ищи Вардена в Обители!";
                    UpdateMoodAndInfo(message, fallbackResponse);
                    retryCount = 0;
                }
            }
        }
    }

    string ParseResponse(string jsonResponse)
    {
        try
        {
            var responseObject = JsonConvert.DeserializeObject<GeminiResponse>(jsonResponse);
            if (responseObject != null && responseObject.candidates != null && responseObject.candidates.Count > 0 &&
                responseObject.candidates[0].content != null && responseObject.candidates[0].content.parts != null &&
                responseObject.candidates[0].content.parts.Count > 0)
            {
                return responseObject.candidates[0].content.parts[0].text.Trim();
            }
            else
            {
                Debug.LogWarning("Unexpected response structure: " + jsonResponse);
                return "Кайра: Ох, что-то пошло не так! Попробуй ещё.";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message + "\nResponse: " + jsonResponse);
            return "Кайра: Ох, запуталась! Давай заново.";
        }
    }

    void UpdateMoodAndInfo(string playerMessage, string kairaResponse)
    {
        // Обработка имени уже выполнена в SendMessage

        // Реакция на контекст
        if (playerMessage.ToLower().Contains("сестра") || playerMessage.ToLower().Contains("пропала"))
        {
            if (playerMessage.ToLower().Contains("туман"))
            {
                kairaResponse = $"Кайра: {playerName}, жаль слышать! Туман забрал её. Ищи Вардена в Обители!";
            }
            else
            {
                kairaResponse = $"Кайра: {playerName}, сестра пропала? Проверь Обитель Искр, там Варден!";
            }
        }
        else if (playerMessage.ToLower().Contains("не понимаю"))
        {
            kairaResponse = $"Кайра: {playerName}, запутанно, да? Саботаж элиты виноват. Ищи Вардена!";
        }
        else if (playerMessage.ToLower().Contains("помоги"))
        {
            kairaResponse = $"Кайра: {playerName}, помогу! Иди к Обители, там Варден!";
        }
        // Если нет условий, используем ответ API как есть
        else if (!string.IsNullOrEmpty(kairaResponse) && !kairaResponse.StartsWith("Кайра: Ох,"))
        {
            // Оставляем как есть
        }
        else
        {
            kairaResponse = $"Кайра: {playerName}, ну ладно! Расскажи про Туман или саботаж.";
        }

        if (kairaResponse.Length > 200) kairaResponse = kairaResponse.Substring(0, 200).Trim() + "…";
        conversationHistory += kairaResponse + "\n";
        UpdateDialogueText();
        if (hasGivenFullInfo && !playerMessage.ToLower().Contains("груб")) Invoke("CheckReminder", 2f);

        Debug.Log($"Mood: {playerMood}, Level: {moodLevel}, Frustration: {frustrationCount}, Retry: {retryCount}, Name: {playerName}");
    }

    void UpdateDialogueText(string prefix = "")
    {
        dialogueText.text = prefix + "\n" + conversationHistory;
    }

    void CheckReminder()
    {
        if (hasGivenFullInfo && !dialogueText.text.Contains("напоминаю"))
            UpdateDialogueText("Кайра: Напомню, ищи Вардена в Обители!");
    }

    private class GeminiResponse { public List<Candidate> candidates { get; set; } }
    private class Candidate { public Content content { get; set; } }
    private class Content { public List<Part> parts { get; set; } }
    private class Part { public string text { get; set; } }
}