# Escena CodexEnd

`CodexEnd` reutiliza el formato de códice para mostrar el cierre narrativo de la demo y las páginas correspondientes al último capítulo jugado.

## Jerarquía principal

```text
CodexEnd
├── Setup
│   ├── Camera
│   ├── CursorMode
│   └── Global Volume
├── Scene
│   └── UI
│       ├── Fade Canvas
│       │   └── Fade Image
│       └── MessageCanvas
│           └── Message
├── Codex
│   ├── P0 - Cover
│   ├── P1 - Chapter 1 Start
│   ├── P2 - News
│   ├── P3 - Oath
│   ├── P4 - Journey
│   ├── P5 - Chapter 2 Start
│   ├── P6 - Avalon
│   ├── P7 - Pendragon
│   ├── P8 - Cameliard
│   ├── P9 - Chapter 3 Start
│   └── P10 - Chapter 3 End
└── IntroManager
```

## Funcionamiento

La escena se posiciona automáticamente en la primera página nueva del cierre y avanza por las páginas del capítulo final. Visualmente es similar a `CodexIntro`, pero contiene más páginas y actúa como continuación narrativa.

## Posible mejora

La escena podría fusionarse con `CodexIntro` en una única escena `Codex`, usando parámetros como página inicial, lista de páginas activas, modo de avance y escena de destino.

[< volver](README.md)