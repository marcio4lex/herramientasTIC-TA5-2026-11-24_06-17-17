using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Envía los eventos de clasificación a n8n y muestra la respuesta en un panel de la escena.
// Flujo: interacción VR -> POST /webhook/reciclaje -> n8n (dashboard + chatbot) -> respuesta en VR.
public class RecyclingEventClient : MonoBehaviour
{
    public static RecyclingEventClient Instance { get; private set; }

    public string n8nBaseUrl = "http://localhost:5678/webhook";
    public TextMesh panelText;
    public TextMesh statsText;

    int correct, wrong;
    string logPath;

    [Serializable] class ClassificationEvent
    {
        public string evento = "clasificacion";
        public string residuo, tipo, contenedor, timestamp, sesion;
        public bool correcto;
    }

    [Serializable] class N8nResponse
    {
        public string mensaje, explicacion;
        public int correctos, incorrectos, total;
        public float tasaPlanta;
    }

    void Awake()
    {
        Instance = this;
        logPath = Path.Combine(Application.persistentDataPath, "eventos_reciclaje.log");
    }

    public void ShowLocal(string msg)
    {
        if (panelText) panelText.text = msg;
    }

    // Chatbot de n8n: explica el residuo que el jugador acaba de recoger.
    public void AskChatbot(WasteItem item)
    {
        ShowLocal($"Llevas: {item.itemName}\nConsultando al chatbot...");
        StartCoroutine(Ask(item));
    }

    IEnumerator Ask(WasteItem item)
    {
        var url = $"{n8nBaseUrl}/chatbot?residuo={UnityWebRequest.EscapeURL(item.itemName)}&tipo={item.type}";
        using var req = UnityWebRequest.Get(url);
        req.timeout = 5;
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            var r = JsonUtility.FromJson<N8nResponse>(req.downloadHandler.text);
            ShowLocal($"Llevas: {item.itemName}\n{Wrap(r.explicacion, 42)}");
        }
        else ShowLocal($"Llevas: {item.itemName}\nBúscale su contenedor.\n(chatbot sin conexión)");
    }

    public void SendClassification(WasteItem item, RecycleBin bin, bool ok)
    {
        if (ok) correct++; else wrong++;
        var ev = new ClassificationEvent
        {
            residuo = item.itemName,
            tipo = item.type.ToString(),
            contenedor = bin.accepts.ToString(),
            correcto = ok,
            timestamp = DateTime.UtcNow.ToString("o"),
            sesion = SystemInfo.deviceName
        };
        ShowLocal((ok ? "¡Correcto!" : "Contenedor incorrecto") + "\nEnviando evento a n8n...");
        StartCoroutine(Post(JsonUtility.ToJson(ev), ev));
    }

    IEnumerator Post(string json, ClassificationEvent ev)
    {
        using var req = new UnityWebRequest(n8nBaseUrl + "/reciclaje", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = 5;
        yield return req.SendWebRequest();

        string shown;
        if (req.result == UnityWebRequest.Result.Success)
        {
            var r = JsonUtility.FromJson<N8nResponse>(req.downloadHandler.text);
            shown = $"{r.mensaje}\n{Wrap(r.explicacion, 42)}";
            if (statsText)
                statsText.text = $"DASHBOARD n8n\nCorrectos: {r.correctos}  Incorrectos: {r.incorrectos}\n" +
                                 $"Total eventos: {r.total}\nReaprovechamiento SJM 2026: {r.tasaPlanta:0.00}%";
        }
        else
        {
            // Sin n8n: respuesta local para que la simulación siga funcionando.
            shown = (ev.correcto ? "¡Correcto! " : "Incorrecto. ") + $"{ev.residuo} va en: {ev.tipo}\n(n8n sin conexión)";
            if (statsText) statsText.text = $"Modo local\nCorrectos: {correct}  Incorrectos: {wrong}";
        }
        ShowLocal(shown);

        var line = $"{ev.timestamp};{ev.residuo};{ev.tipo};{ev.contenedor};{ev.correcto};{req.result}";
        Debug.Log("[Reciclaje] " + line);
        try { File.AppendAllText(logPath, line + Environment.NewLine); } catch { }
    }

    static string Wrap(string s, int width)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var sb = new StringBuilder();
        int col = 0;
        foreach (var word in s.Split(' '))
        {
            if (col + word.Length > width) { sb.Append('\n'); col = 0; }
            sb.Append(word).Append(' ');
            col += word.Length + 1;
        }
        return sb.ToString();
    }
}
