# Estructura del proyecto

El proyecto se organiza en escenas jugables, escenas narrativas, _managers_ persistentes, prefabs de personajes, prefabs de armas/items y recursos definidos mediante `ScriptableObject`.

## Carpetas principales de scripts

```text
Assets/Scripts
├── Codex
├── Enemy
├── Misc
├── NPC
├── Player
├── Spawn
├── Systems
├── UI
└── Weapons&Items
```

## Responsabilidades por carpeta

| Carpeta | Responsabilidad |
|---|---|
| `Codex` | Sistema de páginas narrativas, fade e intro/cierre. |
| `Enemy` | IA, pathfinding, detector, ataque y daño de enemigos. |
| `Misc` | Utilidades de escena, efectos, destructibles, cursor, señales. |
| `NPC` | Personajes no jugables, águila, soldados decorativos y spawners. |
| `Player` | Movimiento, vida, armas activas, recolección y eventos de animación. |
| `Spawn` | Definición de recursos persistentes mediante `ScriptableObject`. |
| `Systems` | Managers globales, escena, input, audio, recursos, portales. |
| `UI` | HUD, fade e icono de dash. |
| `Weapons&Items` | Armas, proyectiles, bombas, empuje, árbol mágico y bloqueos. |

## Escenas principales

| Escena | Tipo | Función |
|---|---|---|
| `CodexIntro` | Narrativa | Presenta la historia antes del gameplay. |
| `Countryside` | Jugable | Mapa exterior principal de la demo. |
| `CountryCave` | Jugable | Zona subterránea conectada por portales. |
| `CodexEnd` | Narrativa | Cierre narrativo tras el capítulo jugado. |

## Managers persistentes

Los managers principales viven en la escena inicial jugable y persisten entre escenas cuando es necesario. Esta decisión permite que el jugador, el inventario, la música y el estado de recursos se mantengan al viajar entre `Countryside` y `CountryCave`.

## Nota de diseño

La demo prioriza claridad frente a complejidad. Hay managers especializados, pero no se introduce una arquitectura de servicios pesada, sistemas de eventos globales complejos ni patrones avanzados innecesarios.

[< volver](README.md)