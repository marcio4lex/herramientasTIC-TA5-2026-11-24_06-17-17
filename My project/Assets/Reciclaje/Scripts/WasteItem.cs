using UnityEngine;

public enum WasteType { Plastico, Papel, Vidrio, Metal, Organico, NoAprovechable, Peligroso }

// Residuo recogible: el jugador lo toma al acercarse (o con E) y lo lleva a un contenedor.
public class WasteItem : MonoBehaviour
{
    public WasteType type;
    public string itemName = "Botella PET";

    Vector3 startPos;
    Quaternion startRot;

    public bool IsCarried { get; private set; }

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void PickUp(Transform holder)
    {
        IsCarried = true;
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        foreach (var c in GetComponents<Collider>()) c.enabled = false;
        transform.SetParent(holder, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Vuelve a su posición inicial para que el recorrido pueda repetirse.
    public void ResetItem()
    {
        IsCarried = false;
        transform.SetParent(null);
        transform.SetPositionAndRotation(startPos, startRot);
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;
        foreach (var c in GetComponents<Collider>()) c.enabled = true;
    }
}
