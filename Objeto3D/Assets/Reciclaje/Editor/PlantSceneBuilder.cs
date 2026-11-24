using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Construye la escena "PlantaReciclaje" (ODS 12): terreno, materiales, prefabs, casas, autos,
// contenedores de clasificación, residuos, paneles y FPSController.
public static class PlantSceneBuilder
{
    const string Root = "Assets/Reciclaje";
    const string ScenePath = "Assets/Scenes/PlantaReciclaje.unity";
    static Font font;

    static readonly (WasteType type, string label, Color color)[] Bins =
    {
        (WasteType.Plastico, "PLÁSTICO", new Color(0.95f, 0.95f, 0.95f)),
        (WasteType.Papel, "PAPEL Y
CARTÓN", new Color(0.10f, 0.35f, 0.85f)),
        (WasteType.Vidrio, "VIDRIO", new Color(0.10f, 0.60f, 0.20f)),
        (WasteType.Metal, "METALES", new Color(0.95f, 0.80f, 0.10f)),
        (WasteType.Organico, "ORGÁNICOS", new Color(0.45f, 0.28f, 0.12f)),
        (WasteType.NoAprovechable, "NO
APROVECHABLES", new Color(0.08f, 0.08f, 0.08f)),
        (WasteType.Peligroso, "PELIGROSOS", new Color(0.85f, 0.10f, 0.10f)),
    };

    static readonly (WasteType type, string name, PrimitiveType shape, Vector3 scale)[] Items =
    {
        (WasteType.Plastico, "Botella PET", PrimitiveType.Capsule, new Vector3(0.15f, 0.15f, 0.15f)),
        (WasteType.Plastico, "Envase de yogur", PrimitiveType.Cylinder, new Vector3(0.18f, 0.1f, 0.18f)),
        (WasteType.Papel, "Caja de cartón", PrimitiveType.Cube, new Vector3(0.4f, 0.3f, 0.3f)),
        (WasteType.Papel, "Periódico", PrimitiveType.Cube, new Vector3(0.35f, 0.05f, 0.25f)),
        (WasteType.Vidrio, "Botella de vidrio", PrimitiveType.Capsule, new Vector3(0.14f, 0.2f, 0.14f)),
        (WasteType.Vidrio, "Frasco de vidrio", PrimitiveType.Cylinder, new Vector3(0.16f, 0.12f, 0.16f)),
        (WasteType.Metal, "Lata de aluminio", PrimitiveType.Cylinder, new Vector3(0.12f, 0.1f, 0.12f)),
        (WasteType.Metal, "Lata de conserva", PrimitiveType.Cylinder, new Vector3(0.15f, 0.08f, 0.15f)),
        (WasteType.Organico, "Cáscara de plátano", PrimitiveType.Capsule, new Vector3(0.1f, 0.15f, 0.1f)),
        (WasteType.Organico, "Restos de fruta", PrimitiveType.Sphere, new Vector3(0.2f, 0.2f, 0.2f)),
        (WasteType.NoAprovechable, "Papel higiénico usado", PrimitiveType.Cube, new Vector3(0.2f, 0.2f, 0.2f)),
        (WasteType.NoAprovechable, "Envoltura metalizada", PrimitiveType.Cube, new Vector3(0.25f, 0.04f, 0.15f)),
        (WasteType.Peligroso, "Pila usada", PrimitiveType.Cylinder, new Vector3(0.08f, 0.08f, 0.08f)),
        (WasteType.Peligroso, "Foco fluorescente", PrimitiveType.Capsule, new Vector3(0.08f, 0.3f, 0.08f)),
    };

    [MenuItem("Reciclaje/Construir escena PlantaReciclaje")]
    public static void Build()
    {
        foreach (var d in new[] { "Materials", "Prefabs", "Terrain" })
            if (!AssetDatabase.IsValidFolder($"{Root}/{d}")) AssetDatabase.CreateFolder(Root, d);
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // ---- Materiales (mínimo 9 colores) ----
        var binMats = new Dictionary<WasteType, Material>();
        foreach (var b in Bins) binMats[b.type] = Mat("Contenedor_" + b.type, b.color);
        var concrete = Mat("Concreto_Gris", new Color(0.55f, 0.55f, 0.55f));
        var orange = Mat("Camion_Naranja", new Color(1f, 0.5f, 0.05f));
        var teal = Mat("Techo_Turquesa", new Color(0.1f, 0.55f, 0.55f));
        var cream = Mat("Pared_Crema", new Color(0.93f, 0.88f, 0.75f));
        var dark = Mat("Panel_Oscuro", new Color(0.12f, 0.14f, 0.18f));
        var belt = Mat("Banda_Caucho", new Color(0.2f, 0.2f, 0.22f));

        BuildTerrain();

        // ---- Patio de la planta ----
        var plant = new GameObject("PlantaReciclaje").transform;
        Box("Patio", plant, new Vector3(0, 0.05f, 8), new Vector3(44, 0.1f, 40), concrete);
        Box("BandaClasificacion", plant, new Vector3(0, 0.45f, 0), new Vector3(14, 0.9f, 1.4f), belt);

        // ---- Prefabs de contenedor y residuos ----
        var binPrefab = BinPrefab(concrete);
        var bins = new GameObject("Contenedores").transform;
        bins.SetParent(plant);
        for (int i = 0; i < Bins.Length; i++)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(binPrefab, bins);
            go.name = "Contenedor_" + Bins[i].type;
            go.transform.position = new Vector3(-9 + i * 3f, 0.1f, 8);
            go.GetComponent<RecycleBin>().accepts = Bins[i].type;
            go.transform.Find("Cuerpo").GetComponent<Renderer>().sharedMaterial = binMats[Bins[i].type];
            var label = go.GetComponentInChildren<TextMesh>();
            label.text = Bins[i].label;
            label.color = Bins[i].type == WasteType.NoAprovechable ? Color.white : Color.black;
        }

        var items = new GameObject("Residuos").transform;
        items.SetParent(plant);
        for (int i = 0; i < Items.Length; i++)
        {
            var it = Items[i];
            var prefab = ItemPrefab(it.name, it.type, it.shape, it.scale, binMats[it.type]);
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, items);
            go.transform.position = new Vector3(-6.5f + i * 1f, 1.1f, 0);
        }

        // ---- Edificios (House.fbx) y camiones (Car.fbx), duplicados ----
        var house = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/House.fbx");
        var buildings = new GameObject("Edificios").transform;
        string[] names = { "Recepcion", "NaveClasificacion", "Almacen" };
        for (int i = 0; i < 3; i++)
        {
            var h = Model(house, names[i], buildings, new Vector3(-22 + i * 22, 0.1f, 34), 180, 12f,
                new[] { cream, teal, concrete, dark });
            foreach (var mf in h.GetComponentsInChildren<MeshFilter>()) mf.gameObject.AddComponent<MeshCollider>();
        }

        var car = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Car.fbx");
        var trucks = new GameObject("Vehiculos").transform;
        var carMats = new[] { orange, binMats[WasteType.Vidrio], binMats[WasteType.Papel] };
        for (int i = 0; i < 3; i++)
        {
            var c = Model(car, "Recolector_" + (i + 1), trucks, new Vector3(16, 0.1f, -4 + i * 7), -90, 4.5f,
                new[] { carMats[i], dark, concrete });
            foreach (var wc in c.GetComponentsInChildren<WheelCollider>()) wc.enabled = false;
            c.AddComponent<BoxCollider>();
            FitBox(c);
        }

        // ---- Paneles de información (VR / FPS) ----
        var client = new GameObject("RecyclingEventClient").AddComponent<RecyclingEventClient>();
        client.panelText = Panel("PanelMensajes", new Vector3(-6, 3.2f, 13), new Vector2(9, 3), dark,
            "Recoge un residuo de la banda\ny llévalo a su contenedor.", 0.09f);
        client.statsText = Panel("PanelDashboard", new Vector3(5, 3.2f, 13), new Vector2(9, 3), dark,
            "DASHBOARD n8n\nEsperando eventos...", 0.09f);
        var data = new GameObject("PanelDatosAbiertos").AddComponent<PlantDataPanel>();
        data.text = Panel("PanelDatos", new Vector3(-13, 2.5f, 2), new Vector2(8, 3.6f), dark, "", 0.07f);
        data.text.transform.parent.rotation = Quaternion.Euler(0, -60, 0);
        Panel("Titulo", new Vector3(0, 6f, 13.05f), new Vector2(20, 1.4f), teal,
            "PLANTA DE RECICLAJE - ODS 12", 0.18f);

        // ---- Jugador FPS (se elimina la Main Camera) ----
        Object.DestroyImmediate(GameObject.Find("Main Camera"));
        var fps = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Standard Assets/Characters/FirstPersonCharacter/Prefabs/FPSController.prefab"));
        fps.transform.position = new Vector3(0, 1.2f, -10);
        fps.AddComponent<WasteCarrier>();

        EditorSceneManager.SaveScene(scene, ScenePath);
        var list = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true) };
        EditorBuildSettings.scenes = list.ToArray();
        AssetDatabase.SaveAssets();
        Debug.Log("[Reciclaje] Escena construida: " + ScenePath);
    }

    static Material Mat(string name, Color c)
    {
        var path = $"{Root}/Materials/{name}.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!m) { m = new Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(m, path); }
        m.color = c;
        m.SetFloat("_Glossiness", 0.25f);
        EditorUtility.SetDirty(m);
        return m;
    }

    static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, Material m)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = size;
        go.GetComponent<Renderer>().sharedMaterial = m;
        return go;
    }

    static TextMesh Text(Transform parent, string text, Vector3 localPos, float size, Color color)
    {
        var go = new GameObject("Texto");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        var tm = go.AddComponent<TextMesh>();
        tm.font = font;
        tm.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        tm.fontSize = 64;
        tm.characterSize = size;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = color;
        tm.text = text;
        return tm;
    }

    static TextMesh Panel(string name, Vector3 pos, Vector2 size, Material bg, string text, float charSize)
    {
        var root = new GameObject(name).transform;
        root.position = pos;
        var back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.name = "Fondo";
        back.transform.SetParent(root, false);
        back.transform.localPosition = new Vector3(0, 0, 0.06f);
        back.transform.localScale = new Vector3(size.x, size.y, 0.1f);
        back.GetComponent<Renderer>().sharedMaterial = bg;
        return Text(root, text, Vector3.zero, charSize, Color.white);
    }

    static GameObject BinPrefab(Material lidMat)
    {
        var root = new GameObject("Contenedor");
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Cuerpo";
        body.transform.SetParent(root.transform, false);
        body.transform.localPosition = new Vector3(0, 0.6f, 0);
        body.transform.localScale = new Vector3(1.4f, 1.2f, 1.2f);
        var lid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lid.name = "Tapa";
        lid.transform.SetParent(root.transform, false);
        lid.transform.localPosition = new Vector3(0, 1.25f, 0);
        lid.transform.localScale = new Vector3(1.5f, 0.1f, 1.3f);
        lid.GetComponent<Renderer>().sharedMaterial = lidMat;
        var label = Text(root.transform, "TIPO", new Vector3(0, 0.75f, -0.61f), 0.017f, Color.black);
        label.fontSize = 96;
        root.AddComponent<RecycleBin>();
        var p = PrefabUtility.SaveAsPrefabAsset(root, $"{Root}/Prefabs/Contenedor.prefab");
        Object.DestroyImmediate(root);
        return p;
    }

    static GameObject ItemPrefab(string name, WasteType type, PrimitiveType shape, Vector3 scale, Material m)
    {
        var go = GameObject.CreatePrimitive(shape);
        go.name = name;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = m;
        go.AddComponent<Rigidbody>().mass = 0.3f;
        var w = go.AddComponent<WasteItem>();
        w.type = type;
        w.itemName = name;
        var file = name.Replace(' ', '_').Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u');
        var p = PrefabUtility.SaveAsPrefabAsset(go, $"{Root}/Prefabs/Residuo_{file}.prefab");
        Object.DestroyImmediate(go);
        return p;
    }

    // Instancia un modelo, lo escala a un tamaño objetivo y le aplica materiales por submalla.
    static GameObject Model(GameObject src, string name, Transform parent, Vector3 pos, float yRot, float targetSize, Material[] mats)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(src, parent);
        go.name = name;
        go.transform.rotation = Quaternion.Euler(0, yRot, 0);
        // Componentes innecesarios que trae el FBX: cámara, luces y plano de suelo.
        foreach (var cam in go.GetComponentsInChildren<Camera>()) cam.gameObject.SetActive(false);
        foreach (var l in go.GetComponentsInChildren<Light>()) l.gameObject.SetActive(false);
        foreach (var t in go.GetComponentsInChildren<Transform>())
            if (t.name == "Plane") t.gameObject.SetActive(false);
        var b = Bounds(go);
        float s = targetSize / Mathf.Max(b.size.x, b.size.z);
        go.transform.localScale *= s;
        b = Bounds(go);
        go.transform.position += pos - new Vector3(b.center.x, b.min.y, b.center.z);
        int k = 0;
        foreach (var r in go.GetComponentsInChildren<Renderer>())
        {
            var arr = new Material[r.sharedMaterials.Length];
            for (int i = 0; i < arr.Length; i++) arr[i] = mats[k++ % mats.Length];
            r.sharedMaterials = arr;
        }
        return go;
    }

    static Bounds Bounds(GameObject go)
    {
        var rs = go.GetComponentsInChildren<Renderer>();
        var b = rs[0].bounds;
        foreach (var r in rs) b.Encapsulate(r.bounds);
        return b;
    }

    static void FitBox(GameObject go)
    {
        var b = Bounds(go);
        var bc = go.GetComponent<BoxCollider>();
        bc.center = go.transform.InverseTransformPoint(b.center);
        var ls = go.transform.lossyScale;
        var size = Quaternion.Inverse(go.transform.rotation) * b.size;
        bc.size = new Vector3(Mathf.Abs(size.x / ls.x), Mathf.Abs(size.y / ls.y), Mathf.Abs(size.z / ls.z));
    }

    static void BuildTerrain()
    {
        const int res = 257;
        var td = new TerrainData { heightmapResolution = res, size = new Vector3(240, 40, 240) };
        var h = new float[res, res];
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float dx = x / (float)(res - 1) - 0.5f, dy = y / (float)(res - 1) - 0.5f;
                float edge = Mathf.InverseLerp(0.22f, 0.5f, Mathf.Sqrt(dx * dx + dy * dy));
                float noise = Mathf.PerlinNoise(x * 0.03f, y * 0.03f);
                h[y, x] = edge * edge * (0.35f + 0.65f * noise);
            }
        td.SetHeights(0, 0, h);

        var grass = Layer("Pasto", "Environment/TerrainAssets/SurfaceTextures/GrassHillAlbedo.psd");
        var rock = Layer("Roca", "Environment/TerrainAssets/SurfaceTextures/GrassRockyAlbedo.psd");
        td.terrainLayers = new[] { grass, rock };
        int ar = td.alphamapResolution;
        var a = new float[ar, ar, 2];
        for (int y = 0; y < ar; y++)
            for (int x = 0; x < ar; x++)
            {
                float steep = td.GetSteepness(x / (float)ar, y / (float)ar) / 40f;
                float r = Mathf.Clamp01(steep);
                a[y, x, 0] = 1 - r; a[y, x, 1] = r;
            }
        td.SetAlphamaps(0, 0, a);
        AssetDatabase.CreateAsset(td, $"{Root}/Terrain/TerrenoPlanta.asset");
        var t = Terrain.CreateTerrainGameObject(td);
        t.name = "Terrain";
        t.transform.position = new Vector3(-120, 0, -112);
    }

    static TerrainLayer Layer(string name, string tex)
    {
        var l = new TerrainLayer
        {
            diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Standard Assets/" + tex),
            tileSize = new Vector2(12, 12)
        };
        AssetDatabase.CreateAsset(l, $"{Root}/Terrain/{name}.terrainlayer");
        return l;
    }
}
