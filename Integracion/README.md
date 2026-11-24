# Integración – ODS 12: Planta de reciclaje

Flujo implementado:

```
Interacción VR/FPS (Unity)            n8n (localhost:5678)                        Respuesta
─────────────────────────            ────────────────────                        ─────────
Recoger residuo ─ GET /webhook/chatbot ──► Chatbot residuos (base de conocimiento) ──► explicación en PanelMensajes
Depositar en contenedor ─ POST /webhook/reciclaje ──► Leer CSV → Extraer CSV → Procesar evento
                                                     (contadores + tasa de reaprovechamiento) ──► mensaje + PanelDashboard
Navegador ─ GET /webhook/dashboard ──► Leer CSV → Construir dashboard HTML ──► dashboard web (se refresca cada 3 s)
```

- **Evento**: JSON `{evento, residuo, tipo, contenedor, correcto, timestamp, sesion}` enviado por `RecyclingEventClient.cs`.
- **Procesamiento**: el workflow guarda los contadores en *static data* de n8n y calcula la tasa de reaprovechamiento con los datos abiertos de `Data/residuos_solidos_sjm_2026.csv`.
- **Respuesta visible**: paneles 3D en la escena (mensaje, explicación del chatbot y dashboard), el dashboard web y el log local `eventos_reciclaje.log` en `Application.persistentDataPath`.
- Sin credenciales privadas: todo corre en local y usa datos abiertos (ODC-BY).

## Ejecutar

Requisitos: Node.js 20 o 22 y n8n (`npm install -g n8n`).

```powershell
cd Integracion
.\iniciar_n8n.ps1
```

El script importa `n8n/planta_reciclaje_workflow.json`, lo activa y arranca n8n con las variables necesarias:

| Variable | Uso |
|---|---|
| `RECICLAJE_CSV` | Ruta absoluta al CSV de `Data/` |
| `N8N_RESTRICT_FILE_ACCESS_TO` | Permite al nodo *Read/Write Files* leer la carpeta `Data/` |
| `N8N_BLOCK_ENV_ACCESS_IN_NODE=false` | Permite usar `$env.RECICLAJE_CSV` en el workflow |

Después, en Unity, presiona Play en la escena `PlantaReciclaje` (Objeto3D) o en el Meta XR Simulator (ObjetoVR):

1. Acércate a la banda y recoge un residuo (automático por proximidad, o con la tecla **E**). El chatbot de n8n explica a qué contenedor va.
2. Llévalo al contenedor y acércate a él. El evento se envía a n8n y el panel muestra la respuesta y el dashboard.
3. Abre http://localhost:5678/webhook/dashboard para ver el dashboard web.

Prueba sin Unity:

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5678/webhook/reciclaje -ContentType application/json `
  -Body '{"evento":"clasificacion","residuo":"Botella PET","tipo":"Plastico","contenedor":"Plastico","correcto":true}'
Invoke-RestMethod "http://localhost:5678/webhook/chatbot?tipo=Vidrio&residuo=Frasco"
```

Si n8n no está disponible, Unity sigue funcionando en modo local y registra los eventos en el log.
