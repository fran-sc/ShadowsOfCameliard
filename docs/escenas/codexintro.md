# Escena CodexIntro

`CodexIntro` es la escena narrativa inicial. Presenta la premisa del juego mediante una doble página de códice medieval: ilustración a la izquierda y texto a la derecha.

## Jerarquía principal

```text
CodexIntro
├── Setup
│   ├── Camera
│   └── CursorMode
├── Scene
│   └── UI
│       ├── Fade Canvas
│       │   └── Fade Image
│       └── MessageCanvas
├── Codex
│   ├── P0 - Cover
│   ├── P1 - Chapter 1 Start
│   ├── P2 - News
│   ├── P3 - Oath
│   ├── P4 - Journey
│   ├── P5 - Chapter 2 Start
│   └── P6 - Chapter 2 End
├── IntroManager
└── Global Volume
```

## Función

La escena introduce:

- la ausencia de Arturo, ocupado en Francia;
- el viaje de Ginebra a Cameliard;
- la invasión de Mordred;
- el rapto de la reina;
- el encargo a Lancelot.

![code](../img/codex.png)

## Nota técnica

`CodexIntro` y `CodexEnd` comparten una estructura similar. Actualmente están separadas para simplificar el flujo, aunque en una versión futura podrían unificarse en una única escena de códice parametrizable.

[< volver](README.md)