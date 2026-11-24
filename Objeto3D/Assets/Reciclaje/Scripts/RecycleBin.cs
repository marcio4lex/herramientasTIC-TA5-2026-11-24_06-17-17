using UnityEngine;

// Contenedor de clasificación (colores según NTP 900.058:2019).
public class RecycleBin : MonoBehaviour
{
    public WasteType accepts;

    public void Deposit(WasteItem item)
    {
        bool correct = item.type == accepts;
        RecyclingEventClient.Instance?.SendClassification(item, this, correct);
        item.ResetItem();
    }
}
