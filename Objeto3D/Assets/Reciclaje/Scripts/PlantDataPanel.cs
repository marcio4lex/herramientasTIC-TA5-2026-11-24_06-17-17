using System.Globalization;
using System.IO;
using UnityEngine;

// Muestra en la planta los datos abiertos de residuos (Data/residuos_solidos_sjm_2026.csv).
public class PlantDataPanel : MonoBehaviour
{
    public string csvFile = "residuos_solidos_sjm_2026.csv";
    public TextMesh text;

    void Start()
    {
        var path = Path.Combine(Application.streamingAssetsPath, csvFile);
        if (!File.Exists(path)) { text.text = "CSV no encontrado"; return; }

        double noAprov = 0, aprov = 0;
        var sb = new System.Text.StringBuilder("RESIDUOS MUNICIPALES - SJM 2026\n(datosabiertos.gob.pe, ODC-BY)\n");
        foreach (var line in File.ReadAllLines(path))
        {
            var f = line.Split(';');
            if (f.Length < 7 || f[2] == "TOTAL" || !double.TryParse(f[5], NumberStyles.Float, CultureInfo.InvariantCulture, out var n)) continue;
            double a = double.Parse(f[6], CultureInfo.InvariantCulture);
            noAprov += n; aprov += a;
            sb.AppendLine($"{f[2],-8} no aprov: {n,9:0.0} t  aprov: {a,5:0.0} t");
        }
        sb.Append($"Tasa de reaprovechamiento: {100 * aprov / (aprov + noAprov):0.00}%");
        text.text = sb.ToString();
    }
}
