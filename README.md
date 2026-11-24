# TA5 – Planta de reciclaje (ODS 12)

Escenario 3D FPS, simulador VR e integración con n8n para la propuesta **ODS 12: Reciclaje – Planta de reciclaje**.

| Carpeta | Contenido |
|---|---|
| `My project/` | Proyecto Unity 3D **Objeto3D** (Parte 1). Se renombrará a `Objeto3D/`. Escena `Assets/Scenes/PlantaReciclaje.unity` |
| `ObjetoVR/` | Proyecto Unity VR (Parte 2): OpenXR + Meta XR Core SDK, Building Block *Camera Rig* + *OVR Player Controller* |
| `Integracion/` | Workflow n8n (eventos de clasificación, chatbot de residuos y dashboard) y script de arranque |
| `Data/` | CSV de datos abiertos de residuos municipales y su README (fuente, fecha y licencia) |

Unity 6000.6.10f1 (Built-in Render Pipeline), Active Input Handling = Both.

## Escena

- Terreno con colinas (Standard Assets TerrainAssets), patio de concreto y banda de clasificación con 14 residuos.
- 7 contenedores con los colores de la NTP 900.058:2019: plástico (blanco), papel y cartón (azul), vidrio (verde), metales (amarillo), orgánicos (marrón), no aprovechables (negro) y peligrosos (rojo).
- `House.fbx` duplicado como recepción, nave de clasificación y almacén; `Car.fbx` duplicado como vehículos recolectores. Se desactivaron la cámara, las luces y el plano que traen los FBX.
- 13 materiales (colores) y prefabs (`Assets/Reciclaje/Materials`, `Assets/Reciclaje/Prefabs`). La escena se regenera con el menú **Reciclaje → Construir escena PlantaReciclaje**.
- Scripts de Standard Assets corregidos: `GUIText` → `Text` y `GUITexture` → `Image`, con `using UnityEngine.UI`.

## Interacción

Acércate a un residuo para recogerlo (o pulsa **E**) y llévalo a un contenedor. Cada acción llama a n8n: el chatbot explica el residuo y el evento de clasificación actualiza el dashboard. La respuesta aparece en los paneles de la escena. Ver `Integracion/README.md`.

## Meta XR Simulator (ObjetoVR)

1. Abre `ObjetoVR` en Unity y la escena `PlantaReciclaje`.
2. En la barra de herramientas, menú **Meta XR Simulator**, instala el simulador. Es un instalador de Windows y requiere permisos de administrador.
3. Ve a **Window → Meta → Meta XR Simulator → Activate** y presiona Play. Navega con W, A, S, D, clic derecho y la rueda del mouse.
