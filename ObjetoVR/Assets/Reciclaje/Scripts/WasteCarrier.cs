using UnityEngine;

// Va en el jugador (FPSController u OVRPlayerController). Recoge el residuo más cercano
// y lo deposita en el contenedor más cercano. Funciona por proximidad para el Meta XR Simulator
// y también con la tecla E en modo FPS.
public class WasteCarrier : MonoBehaviour
{
    public float pickupRadius = 1.8f;
    public float binRadius = 2.2f;
    public bool autoPickup = true;
    public KeyCode actionKey = KeyCode.E;

    Transform holder;
    WasteItem carried;
    float cooldown;

    void Start()
    {
        var cam = GetComponentInChildren<Camera>();
        holder = new GameObject("WasteHolder").transform;
        holder.SetParent(cam ? cam.transform : transform, false);
        holder.localPosition = new Vector3(0.35f, -0.35f, 0.9f);
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        bool pressed = Input.GetKeyDown(actionKey);
        if (cooldown > 0f && !pressed) return;

        if (carried == null)
        {
            var item = Nearest<WasteItem>(pickupRadius, i => !i.IsCarried);
            if (item && (autoPickup || pressed))
            {
                carried = item;
                item.PickUp(holder);
                RecyclingEventClient.Instance?.AskChatbot(item);
                cooldown = 0.5f;
            }
        }
        else
        {
            var bin = Nearest<RecycleBin>(binRadius, b => true);
            if (bin && (autoPickup || pressed))
            {
                bin.Deposit(carried);
                carried = null;
                cooldown = 1.5f;
            }
        }
    }

    T Nearest<T>(float radius, System.Func<T, bool> ok) where T : Component
    {
        T best = null;
        float bestD = radius;
        foreach (var c in FindObjectsByType<T>(FindObjectsSortMode.None))
        {
            if (!ok(c)) continue;
            var a = c.transform.position; var b = transform.position;
            a.y = b.y = 0f;
            float d = Vector3.Distance(a, b);
            if (d < bestD) { bestD = d; best = c; }
        }
        return best;
    }
}
