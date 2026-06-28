# Tilemaps

El proyecto usa `Tilemap` para construir los mapas 2D principales.

## Countryside

```text
Grid
├── Deco
├── Mountains
├── Roads
├── Water Anim
├── Ground
├── Tower
└── Tower Deco
```

| Tilemap | Uso |
|---|---|
| `Ground` | Base transitable del terreno. |
| `Roads` | Caminos que guían al jugador. |
| `Mountains` | Límites y zonas no caminables. |
| `Water Anim` | Ríos, lagos y mar con animación de orilla; no caminable. |
| `Deco` | Flores, detalles y pequeñas decoraciones. |
| `Tower` | Suelo/estructura principal de la torre. |
| `Tower Deco` | Decoración adicional de la torre. |

## CountryCave

```text
Grid
├── Deco
├── Walls
├── Water
└── Ground
```

| Tilemap | Uso |
|---|---|
| `Ground` | Suelo transitable de la cueva. |
| `Walls` | Paredes y límites rocosos. |
| `Water` | Agua subterránea no transitable. |
| `Deco` | Huesos, rocas, detalles y decoración. |

## Criterio de diseño

La separación de tilemaps permite controlar colisiones, sorting y edición por capas. Para la demo, el enfoque es suficiente y evita sistemas de navegación complejos.

En el caso del agua (ríos, mares, lagos) de `Countryside` se aplican tiles animados para generar un efecto de ola en la orilla.

[< volver](README.md)