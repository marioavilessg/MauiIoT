# 🤖 MauiIoT — Asistente IoT con Chat + IA

Una aplicación **MAUI Blazor Hybrid** que permite controlar luces de una placa **Wokwi** mediante lenguaje natural. Escribe un comando en el chat, la IA lo interpreta y enciende el LED correspondiente en tiempo real.

---

## 📐 Arquitectura del sistema

```
┌─────────────────────┐      HTTP POST        ┌──────────────────────┐
│   App MAUI (Blazor) │ ──────────────────▶  │  MCP Server           │
│   localhost (app)   │   /tools/encender_*   │  localhost:5000       │
└─────────────────────┘                       └──────────┬───────────┘
         │                                               │ HTTP POST
         │ HTTP POST                                     ▼
         │ /v1/chat/completions              ┌──────────────────────┐
         ▼                                   │  Node-RED             │
┌─────────────────────┐                      │  localhost:1880/leds  │
│  LM Studio (LLM)   │                       └──────────┬───────────┘
│  localhost:1234     │                                  │ Wokwi API
└─────────────────────┘                                  ▼
                                             ┌──────────────────────┐
                                             │  Placa Wokwi (LEDs)  │
                                             └──────────────────────┘
```

| Componente | Descripción | Puerto |
|---|---|---|
| **MauiIoT** | App de escritorio con chat UI | — |
| **McpServer** | API REST que traduce comandos a señales LED | `5000` |
| **LM Studio** | Modelo de lenguaje local (inferencia) | `1234` |
| **Node-RED** | Broker HTTP → Wokwi | `1880` |
| **Wokwi** | Simulador de placa con LEDs | online |

---

## ✅ Prerrequisitos

### 1. .NET 9 SDK + MAUI Workload
```bash
# Instalar .NET 9 SDK desde https://dotnet.microsoft.com/download
dotnet --version   # debe mostrar 9.x.x

# Instalar workload de MAUI
dotnet workload install maui
```

### 2. LM Studio
1. Descargar desde [lmstudio.ai](https://lmstudio.ai)
2. Descargar cualquier modelo compatible con **tool calling** (recomendado: `qwen2.5-7b-instruct` o similar)
3. Ir a **Local Server** → activar el servidor en el puerto **1234**
4. Asegurarse de tener el modo **tool calling** habilitado

### 3. Node-RED
```bash
npm install -g node-red
node-red
```
Accede en `http://localhost:1880` y crea un flujo que:
- Escuche `POST /leds`
- Envíe el payload (`encender_rojo`, `encender_verde`, `encender_amarillo`) a tu placa Wokwi via su API

### 4. Wokwi
- Configura tu proyecto en [wokwi.com](https://wokwi.com) con LEDs rojo, verde y amarillo
- Conecta Node-RED a la API de Wokwi según tu configuración de pines

---

## 🚀 Instalación y ejecución

### Clonar el repositorio
```bash
git clone https://github.com/marioavilessg/MauiIoT.git
cd MauiIoT
```

### Paso 1 — Iniciar LM Studio
Abre LM Studio → carga tu modelo → activa el servidor local en `localhost:1234`.

### Paso 2 — Iniciar Node-RED
```bash
node-red
```
Importa o crea el flujo en `http://localhost:1880`.

### Paso 3 — Arrancar el MCP Server

> El MCP Server actúa como puente entre la app MAUI y Node-RED.

```bash
cd McpServer
dotnet run
```

Verás en consola:
```
Now listening on: http://localhost:5000
```

Endpoints disponibles:
| Método | Ruta | Acción |
|---|---|---|
| `GET` | `/tools` | Lista los tools disponibles |
| `POST` | `/tools/encender_luz_roja` | Enciende LED rojo |
| `POST` | `/tools/encender_luz_verde` | Enciende LED verde |
| `POST` | `/tools/encender_luz_amarilla` | Enciende LED amarillo |

### Paso 4 — Arrancar la app MAUI

```bash
cd MauiIoT
dotnet run -f net9.0-windows10.0.19041.0
```

> En macOS usa `-f net9.0-maccatalyst`, en Android `-f net9.0-android`.

---

## 💬 Uso de la aplicación

Una vez arrancado todo, la app mostrará una interfaz de chat. Puedes:

**Escribir comandos en lenguaje natural:**
- `"Enciende la luz roja"`
- `"Activa el LED verde"`
- `"Pon la luz amarilla"`

**O usar los botones de acción rápida** (Roja / Verde / Amarilla) en la barra superior para activar los LEDs directamente, sin pasar por el LLM.

El indicador de conexión en la cabecera muestra si la app puede comunicarse correctamente con los servicios.

---

## 🗂 Estructura del proyecto

```
MauiIoT/
├── MauiIoT/                     # App principal MAUI Blazor Hybrid
│   ├── Components/
│   │   ├── Layout/
│   │   │   └── MainLayout.razor        # Layout raíz (full-screen)
│   │   └── Pages/
│   │       └── Home.razor              # Interfaz de chat completa
│   ├── Services/
│   │   ├── IoTService.cs               # Envío de comandos a Node-RED
│   │   ├── LLMService.cs               # Llamadas a LM Studio (tool calling)
│   │   └── McpClientService.cs         # Llamadas al MCP Server
│   └── wwwroot/
│       ├── index.html                  # Entry point (Tailwind CDN, fuentes)
│       └── css/app.css                 # Estilos base + animaciones LED
│
└── McpServer/                   # API REST intermediaria
    └── Program.cs               # Endpoints /tools/*  →  Node-RED
```

---

## ⚙️ Configuración de puertos

Si necesitas cambiar los puertos por defecto, edita:

| Archivo | Variable | Valor por defecto |
|---|---|---|
| `Services/LLMService.cs` | URL de LM Studio | `http://localhost:1234` |
| `Services/McpClientService.cs` | URL del MCP Server | `http://localhost:5000` |
| `McpServer/Program.cs` | Puerto del servidor | `5000` |
| `McpServer/Program.cs` | URL de Node-RED | `http://localhost:1880` |

---

## 🛠 Troubleshooting

| Síntoma | Causa probable | Solución |
|---|---|---|
| `⚠️ Error de conexión` en el chat | LM Studio no está corriendo | Inicia el servidor local en LM Studio |
| Los LEDs no responden | MCP Server no iniciado | `cd McpServer && dotnet run` |
| MCP Server no reenvía a Wokwi | Node-RED no está activo | `node-red` en terminal |
| La app no compila | Falta workload MAUI | `dotnet workload install maui` |
| Modelo no hace tool calling | Modelo no compatible | Usa un modelo `instruct` con soporte de functions |

---

## 📦 Dependencias NuGet

| Paquete | Versión |
|---|---|
| `Microsoft.Maui.Controls` | `$(MauiVersion)` |
| `Microsoft.AspNetCore.Components.WebView.Maui` | `$(MauiVersion)` |
| `Microsoft.Extensions.Logging.Debug` | `9.0.5` |

> Tailwind CSS se carga vía **CDN** (`cdn.tailwindcss.com`) — no requiere instalación de npm.

