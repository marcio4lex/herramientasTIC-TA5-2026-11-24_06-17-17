# Guía para ejecutar el proyecto en otra computadora

**TA5 – Planta de Reciclaje (ODS 12)**: escenario 3D con recorrido FPS, simulador VR con Meta XR e integración con n8n.

Esta guía explica, paso a paso, cómo descargar el proyecto, abrirlo y ejecutar sus tres partes en una computadora nueva. Si sigues los pasos en orden, no necesitas configurar nada más.

---

## Índice

1. [¿Qué hay en el proyecto?](#1-qué-hay-en-el-proyecto)
2. [Requisitos](#2-requisitos)
3. [Instalar los programas](#3-instalar-los-programas)
4. [Descargar (clonar) el repositorio](#4-descargar-clonar-el-repositorio)
5. [Parte 1: abrir y jugar Objeto3D (FPS)](#5-parte-1-abrir-y-jugar-objeto3d-fps)
6. [Parte 3: iniciar la integración con n8n](#6-parte-3-iniciar-la-integración-con-n8n)
7. [Parte 2: abrir ObjetoVR y usar el Meta XR Simulator](#7-parte-2-abrir-objetovr-y-usar-el-meta-xr-simulator)
8. [Cómo se juega](#8-cómo-se-juega)
9. [Comprobar que todo funciona](#9-comprobar-que-todo-funciona)
10. [Solución de problemas](#10-solución-de-problemas)
11. [Armar el ZIP de entrega](#11-armar-el-zip-de-entrega)

---

## 1. ¿Qué hay en el proyecto?

```
herramientasTIC-TA5/
├── Objeto3D/      → Parte 1: proyecto Unity 3D con recorrido FPS
├── ObjetoVR/      → Parte 2: proyecto Unity VR (Meta XR + OpenXR)
├── Integracion/   → Parte 3: workflow de n8n y script para iniciarlo
├── Data/          → CSV de datos abiertos de residuos + README con fuente y licencia
├── README.md      → Resumen del proyecto
└── INSTALACION.md → Esta guía
```

**En qué consiste la experiencia:** el jugador recorre una planta de reciclaje. Recoge residuos (botellas, latas, cajas, pilas…) de una banda transportadora y los deposita en el contenedor del color correcto. Cada acción se envía a **n8n**, que:

- actúa como **chatbot**: explica en qué contenedor va cada residuo y por qué;
- **registra** si la clasificación fue correcta o incorrecta;
- calcula la **tasa de reciclaje** del distrito con datos abiertos reales;
- muestra todo en un **dashboard web** y en los paneles de la escena.

> **Importante:** `Objeto3D` y `ObjetoVR` son **dos proyectos de Unity distintos**. Cada uno se abre por separado en Unity Hub.

---

## 2. Requisitos

| Requisito | Detalle |
|---|---|
| Sistema operativo | Windows 10 u 11 (64 bits) |
| Memoria RAM | 16 GB recomendados (8 GB como mínimo, abriendo un solo proyecto a la vez) |
| Espacio en disco | ~15 GB libres (Unity + proyectos + simulador) |
| Internet | Necesario la primera vez, para descargar los paquetes de Unity y Meta |
| Cuenta de GitHub | Con acceso al repositorio, porque es **privado** |

### Programas que vas a instalar

| Programa | Para qué sirve | ¿Obligatorio? |
|---|---|---|
| **Git** | Descargar el repositorio | Sí |
| **Git LFS** | Descargar modelos 3D, texturas y audios | **Sí, imprescindible** |
| **Unity Hub** | Administrar las versiones de Unity y los proyectos | Sí |
| **Unity 6000.6.10f1** | Abrir los proyectos (debe ser **esta versión exacta**) | Sí |
| **Node.js 22** | Ejecutar n8n | Sí, para la Parte 3 |
| **n8n** | Integración: chatbot y dashboard | Sí, para la Parte 3 |
| **Meta XR Simulator** | Probar la realidad virtual sin visor | Sí, para la Parte 2 |

---

## 3. Instalar los programas

### 3.1 Git y Git LFS

1. Descarga Git desde https://git-scm.com/download/win e instálalo con las opciones por defecto. Las versiones recientes de Git para Windows **ya incluyen Git LFS**.
2. Abre **PowerShell** (menú Inicio → escribe "PowerShell") y ejecuta:

   ```powershell
   git --version
   git lfs install
   ```

   Debe aparecer algo como `git version 2.x` y `Git LFS initialized.`

> ⚠️ **¿Por qué Git LFS?** Los archivos pesados (modelos `.fbx`, texturas `.psd`, audios `.wav`) están guardados con Git LFS. Si clonas **sin** Git LFS, esos archivos llegan "vacíos" y en Unity verás objetos rosados o modelos que faltan.

### 3.2 Unity Hub y Unity 6000.6.10f1

1. Descarga Unity Hub desde https://unity.com/download e instálalo.
2. Inicia sesión con tu cuenta de Unity. Si no tienes una, créala gratis; la licencia **Personal** es suficiente.
3. En Unity Hub ve a **Installs → Install Editor**.
4. Busca la versión **6000.6.10f1**. Si no aparece en la lista principal, búscala en la pestaña **Archive** o en https://unity.com/releases/editor/archive.
5. Al elegir los módulos, marca:
   - ✅ **Windows Build Support (IL2CPP)**, opcional pero recomendado.
   - ✅ **Android Build Support**, opcional. Solo hace falta si alguna vez quieres compilar para un visor Quest real. **Si no lo instalas, Unity mostrará el aviso "Android SDK not Found"; es normal y puedes cerrarlo.**
6. Espera a que termine la instalación.

> Usa **exactamente** la versión 6000.6.10f1. Con otra versión, Unity puede pedir "actualizar" el proyecto y aparecer errores.

### 3.3 Node.js y n8n

1. Descarga **Node.js 22 LTS** desde https://nodejs.org e instálalo con las opciones por defecto.
2. Cierra y vuelve a abrir PowerShell. Comprueba la instalación:

   ```powershell
   node -v
   ```

   Debe mostrar `v22.x.x`.
3. Instala n8n. Tarda varios minutos y es normal ver muchos mensajes:

   ```powershell
   npm install -g n8n
   ```

4. Comprueba la instalación:

   ```powershell
   n8n --version
   ```

### 3.4 Meta XR Simulator

1. Entra a https://developers.meta.com/horizon/downloads/package/meta-xr-simulator-windows
2. Descarga el instalador para Windows (un archivo `.msi` de unos 400 MB).
3. Haz **doble clic** en el `.msi` y sigue el asistente. Pedirá permisos de administrador.
4. Por defecto se instala en `C:\Program Files\MetaXRSimulator\`.

> Si el antivirus bloquea el simulador, desactívalo temporalmente (lo recomienda la consigna de la tarea).

---

## 4. Descargar (clonar) el repositorio

1. Abre PowerShell y ve a la carpeta donde quieres guardar el proyecto. Por ejemplo:

   ```powershell
   cd E:\Proyectos
   ```

2. Clona el repositorio:

   ```powershell
   git clone https://github.com/marcio4lex/herramientasTIC-TA5-2026-11-24_06-17-17.git herramientasTIC-TA5
   ```

   - Como el repositorio es **privado**, Git abrirá una ventana para iniciar sesión en GitHub. Inicia sesión con una cuenta que tenga acceso.
   - Al final verás un mensaje como `Filtering content: 100%`. Es Git LFS descargando los archivos pesados.

3. Entra a la carpeta y verifica que los archivos LFS se descargaron bien:

   ```powershell
   cd herramientasTIC-TA5
   git lfs ls-files
   ```

   Debe aparecer una lista de archivos (por ejemplo `House.fbx`, `Car.fbx`, texturas…). Si sale vacía o hay errores, ejecuta:

   ```powershell
   git lfs pull
   ```

---

## 5. Parte 1: abrir y jugar Objeto3D (FPS)

### 5.1 Agregar el proyecto a Unity Hub

1. Abre **Unity Hub** → pestaña **Projects**.
2. Haz clic en la flecha junto a **Add** → **Add project from disk**.
3. Selecciona la carpeta **`herramientasTIC-TA5\Objeto3D`**. Elige esta carpeta, **no** la carpeta raíz del repositorio.
4. Comprueba que en la columna de versión aparezca **6000.6.10f1** y haz clic en el proyecto para abrirlo.

### 5.2 Primera apertura (paciencia)

- La primera vez Unity **tarda varios minutos** (entre 5 y 15 según la PC). Tiene que regenerar la carpeta `Library` y descargar paquetes. Es normal.
- Si aparece una ventana preguntando por **Safe Mode**, elige **Ignore**. Si entró en Safe Mode, espera a que termine de compilar.
- Si aparece **"Script Updating Consent"**, elige **"Yes, for these and other files that might be found later"**.

### 5.3 Abrir la escena y jugar

1. En la ventana **Project** (abajo), entra a **Assets → Scenes**.
2. Haz doble clic en **`PlantaReciclaje`**.
3. Presiona el botón **▶ Play** (arriba, al centro).
4. Haz clic dentro de la ventana **Game** para que capture el mouse.

**Controles FPS:**

| Tecla | Acción |
|---|---|
| W / A / S / D | Caminar |
| Mouse | Mirar alrededor |
| Shift | Correr |
| Espacio | Saltar |
| E | Recoger o depositar un residuo (también funciona solo con acercarte) |
| Esc | Liberar el mouse |

> Si n8n **no** está corriendo, el juego funciona igual en **modo local**: los paneles dirán "(n8n sin conexión)". Para la integración completa, sigue la sección 6.

---

## 6. Parte 3: iniciar la integración con n8n

n8n tiene que estar **corriendo antes** de presionar Play en Unity, para que la escena pueda comunicarse con él.

### 6.1 Iniciar n8n

1. Abre PowerShell en la carpeta del repositorio:

   ```powershell
   cd E:\Proyectos\herramientasTIC-TA5
   ```

2. Ejecuta el script de inicio:

   ```powershell
   powershell -ExecutionPolicy Bypass -File .\Integracion\iniciar_n8n.ps1
   ```

   El script hace tres cosas automáticamente:
   - **importa** el workflow `Integracion/n8n/planta_reciclaje_workflow.json`;
   - lo **publica** (activa);
   - **inicia** n8n con las variables necesarias para leer el CSV de `Data/`.

3. Espera a ver estas líneas:

   ```
   Activated workflow "Planta de Reciclaje ODS 12"
   Editor is now accessible via:
   http://localhost:5678
   ```

4. **No cierres esa ventana de PowerShell.** Mientras siga abierta, n8n sigue funcionando. Para detenerlo, presiona `Ctrl + C`.

> Si aparece `AxiosError: timeout of 6000ms exceeded`, **ignóralo**: es n8n intentando conectarse a su propio servidor de noticias y no afecta al proyecto.

### 6.2 Primera vez en n8n

1. Abre en el navegador http://localhost:5678
2. La primera vez te pedirá crear una **cuenta de propietario** (correo y contraseña). Es solo local: se guarda en tu PC y no se envía a ningún lado.
3. Verás el workflow **"Planta de Reciclaje ODS 12"**. Ábrelo para ver los tres flujos:

| Flujo | Dirección | Qué hace |
|---|---|---|
| Evento reciclaje | `POST /webhook/reciclaje` | Recibe cada clasificación, actualiza los contadores, lee el CSV y responde a Unity |
| Chatbot | `GET /webhook/chatbot` | Explica en qué contenedor va cada residuo |
| Dashboard | `GET /webhook/dashboard` | Página web con estadísticas, eventos y datos abiertos |

### 6.3 Ver el dashboard

Abre **http://localhost:5678/webhook/dashboard**. Se actualiza solo cada 3 segundos y muestra:

- total de eventos, correctos, incorrectos y precisión;
- eventos por tipo de residuo;
- los últimos eventos recibidos;
- la tabla de datos abiertos de residuos municipales y la tasa de reaprovechamiento del distrito.

### 6.4 Prueba rápida sin Unity (opcional)

Con n8n corriendo, abre **otra** ventana de PowerShell y ejecuta:

```powershell
Invoke-RestMethod "http://localhost:5678/webhook/chatbot?tipo=Vidrio&residuo=Frasco"
```

Si responde con una explicación sobre el vidrio, n8n está funcionando correctamente.

---

## 7. Parte 2: abrir ObjetoVR y usar el Meta XR Simulator

### 7.1 Agregar y abrir el proyecto

1. En **Unity Hub** → **Add → Add project from disk**.
2. Selecciona la carpeta **`herramientasTIC-TA5\ObjetoVR`**.
3. Ábrelo. La **primera apertura tarda más** que Objeto3D (10 a 20 minutos): descarga los paquetes de **Meta XR SDK** y **OpenXR**.
4. Puede aparecer la ventana **"Welcome to Meta XR SDK"** u otras notificaciones de Meta. Puedes cerrarlas.

> 💡 Si tu PC tiene poca RAM, **cierra Objeto3D** antes de abrir ObjetoVR.

### 7.2 Abrir la escena

En **Project → Assets → Scenes**, haz doble clic en **`PlantaReciclaje`**.

En la jerarquía (ventana **Hierarchy**) verás el **OVRPlayerController**, que contiene el Building Block **[BuildingBlock] Camera Rig**.

### 7.3 Activar el simulador

1. Asegúrate de haber instalado el Meta XR Simulator (sección 3.4).
2. En el menú superior de Unity: **Window → Meta → Meta XR Simulator → Activate**.
   - Si **Activate** aparece en gris, el simulador no está instalado o Unity no lo detecta. Reinstala el `.msi` y reinicia Unity.
3. En la consola de Unity debe aparecer: `[Meta XR Simulator is activated]`.

> La activación **se guarda en la PC**: en esa computadora solo hay que hacerla una vez.

### 7.4 Jugar en VR

1. Si n8n no está corriendo, inícialo (sección 6).
2. Presiona **▶ Play** en Unity.
3. Si aparece **"Android SDK not Found"**, haz clic en **OK** o cierra la ventana. Es normal y no afecta.
4. Se abrirá la ventana **Meta XR Simulator**, que muestra lo que vería el visor.

**Controles del simulador** (haz clic dentro de la ventana del simulador):

| Control | Acción |
|---|---|
| W / A / S / D | Moverse |
| Clic derecho (mantener) + mover mouse | Girar la cabeza o mirar |
| Rueda del mouse | Ajustar la altura o el movimiento de la vista |
| Acercarse a un residuo | Se recoge automáticamente |
| Acercarse a un contenedor con un residuo | Se deposita y se envía el evento a n8n |

> En la consola de Unity aparecerán mensajes rojos como `ErrorFunctionUnsupported: xrGetInstanceProcAddr…`. **Son normales**: son funciones del visor Quest real que el simulador no implementa, y no afectan al proyecto.

---

## 8. Cómo se juega

1. **Ve a la banda transportadora** (la mesa gris larga, delante de ti) donde están los 14 residuos.
2. **Acércate a un residuo** (o presiona **E**). Lo verás "en la mano" y el **panel de mensajes** mostrará la explicación del chatbot de n8n.
3. **Llévalo al contenedor correcto.** Los colores siguen la norma peruana NTP 900.058:2019:

| Color | Contenedor | Ejemplos de la escena |
|---|---|---|
| ⚪ Blanco | Plástico | Botella PET, envase de yogur |
| 🔵 Azul | Papel y cartón | Caja de cartón, periódico |
| 🟢 Verde | Vidrio | Botella de vidrio, frasco |
| 🟡 Amarillo | Metales | Lata de aluminio, lata de conserva |
| 🟤 Marrón | Orgánicos | Cáscara de plátano, restos de fruta |
| ⚫ Negro | No aprovechables | Papel higiénico usado, envoltura metalizada |
| 🔴 Rojo | Peligrosos | Pila usada, foco fluorescente |

4. **Acércate al contenedor.** El residuo se deposita y:
   - el **panel de mensajes** dice "¡Correcto!" o "Incorrecto" con la explicación;
   - el **panel Dashboard** de la escena actualiza los contadores;
   - el **dashboard web** registra el evento.
5. El residuo **vuelve a la banda**, así que puedes repetir el recorrido las veces que quieras.

**Paneles de la escena:**
- **PLANTA DE RECICLAJE – ODS 12**: el título.
- **Panel de mensajes** (izquierda): instrucciones y respuestas del chatbot.
- **Panel Dashboard n8n** (derecha): contadores que devuelve n8n.
- **Panel de datos abiertos** (a la izquierda de la banda): toneladas mensuales de residuos de San Juan de Miraflores, leídas del CSV.

---

## 9. Comprobar que todo funciona

| ✔ | Comprobación | Resultado esperado |
|---|---|---|
| ☐ | `git lfs ls-files` | Lista los archivos `.fbx`, `.psd`, `.wav`… |
| ☐ | Objeto3D abre sin errores rojos | La consola de Unity está limpia |
| ☐ | Play en Objeto3D | Caminas, recoges residuos y los depositas |
| ☐ | n8n iniciado | `Activated workflow "Planta de Reciclaje ODS 12"` |
| ☐ | Panel de mensajes con n8n | Muestra la explicación del chatbot (no "sin conexión") |
| ☐ | Dashboard web | http://localhost:5678/webhook/dashboard muestra los eventos |
| ☐ | ObjetoVR abre sin Safe Mode | La consola no tiene errores de compilación |
| ☐ | Simulador activado | `[Meta XR Simulator is activated]` en la consola |
| ☐ | Play en ObjetoVR | Se abre la ventana Meta XR Simulator y puedes clasificar residuos |

---

## 10. Solución de problemas

### Los modelos se ven rosados o faltan la casa y los autos
No se descargaron los archivos de Git LFS. En la carpeta del repositorio ejecuta:
```powershell
git lfs install
git lfs pull
```
Luego vuelve a abrir el proyecto en Unity.

### Unity pide "actualizar" el proyecto o abre con otra versión
Estás usando una versión distinta a **6000.6.10f1**. Instala esa versión exacta (sección 3.2) y, en Unity Hub, cambia la versión del proyecto en la columna **Editor Version**.

### Unity entra en Safe Mode
Hay errores de compilación, casi siempre porque los paquetes no terminaron de descargarse.
1. Verifica que tienes internet.
2. Dentro de Safe Mode, espera a que termine de importar.
3. Si no sale solo, cierra Unity, borra la carpeta `Library` del proyecto (Unity la regenera) y vuelve a abrirlo.

### Los paneles dicen "(n8n sin conexión)" o "(chatbot sin conexión)"
n8n no está corriendo o no es accesible.
1. Revisa que la ventana de PowerShell con n8n siga abierta.
2. Abre http://localhost:5678 en el navegador. Si no carga, vuelve a ejecutar el script de la sección 6.1.
3. Inicia n8n **antes** de presionar Play.

### "running scripts is disabled on this system" al ejecutar el script
Windows bloquea los scripts por seguridad. Usa el comando con `-ExecutionPolicy Bypass`:
```powershell
powershell -ExecutionPolicy Bypass -File .\Integracion\iniciar_n8n.ps1
```

### "n8n no se reconoce como un comando"
n8n no está instalado, o PowerShell se abrió antes de instalarlo. Cierra PowerShell, ábrelo de nuevo y ejecuta `npm install -g n8n`.

### "Port 5678 is already in use"
Ya hay un n8n corriendo, quizá en otra ventana. Ciérralo con `Ctrl + C` o usa el que ya está abierto.

### El dashboard muestra la tasa de reaprovechamiento en error o vacía
n8n no encuentra el CSV. Asegúrate de iniciar n8n **con el script** `iniciar_n8n.ps1`, porque configura la ruta a `Data\residuos_solidos_sjm_2026.csv`, y de que esa carpeta exista.

### "Activate" del Meta XR Simulator aparece en gris
- El simulador no está instalado: instala el `.msi` (sección 3.4).
- O ya está activado: revisa **Window → Meta → Meta XR Simulator → Status**.

### No se abre la ventana del simulador al dar Play
1. Revisa que en la consola aparezca `[Meta XR Simulator is activated]`.
2. Desactiva temporalmente el antivirus.
3. Ve a **Edit → Project Settings → XR Plug-in Management** y comprueba que en la pestaña de Windows esté marcado **OpenXR**.

### "Android SDK not Found"
Es normal si no instalaste el módulo de Android. Haz clic en **OK**: no afecta al simulador.

### Mensajes rojos `ErrorFunctionUnsupported` en ObjetoVR
Son normales con el simulador (funciones del visor real no disponibles). No afectan.

### La PC va muy lenta
Abre **un solo proyecto de Unity a la vez** y cierra programas que no uses. Unity con Meta XR consume bastante RAM.

---

## 11. Armar el ZIP de entrega

La consigna pide un ZIP llamado `LAB_U5_GRUPO_#` **sin** las carpetas temporales de Unity.

1. Cierra Unity.
2. Copia a una carpeta nueva:
   - `Objeto3D/`
   - `ObjetoVR/`
   - `Data/`
   - `Integracion/`
   - tu video y tu informe PDF.
3. **Dentro de `Objeto3D` y de `ObjetoVR`, borra** estas carpetas, si existen:
   - `Library/`
   - `Temp/`
   - `Logs/`
   - `UserSettings/`
   - `obj/`
4. Comprime la carpeta y nómbrala `LAB_U5_GRUPO_#` (reemplaza `#` por el número de tu grupo).

> Estas carpetas Unity las regenera solo y pesan varios GB. Sin ellas el ZIP queda en unos pocos MB.

---

## Resumen rápido (cuando ya está todo instalado)

```powershell
# 1. Iniciar n8n (dejar esta ventana abierta)
cd E:\Proyectos\herramientasTIC-TA5
powershell -ExecutionPolicy Bypass -File .\Integracion\iniciar_n8n.ps1
```

2. Unity Hub → abrir **Objeto3D** o **ObjetoVR** → escena **PlantaReciclaje** → **▶ Play**.
3. Dashboard: http://localhost:5678/webhook/dashboard
