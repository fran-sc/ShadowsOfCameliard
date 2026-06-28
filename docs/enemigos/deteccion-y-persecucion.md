# Detección y persecución

Los enemigos usan un objeto hijo `Detector` asignado al componente `EnemyDetector`.

Cuando Lancelot entra en el área de detección, el enemigo empieza a perseguirlo. `EnemyPathfinding` usa dos velocidades:

| Parámetro | Valor observado |
|---|---:|
| `Move Speed` | `1.5` |
| `Chase Speed` | `2.5` |

El enemigo se desplaza más despacio durante roaming y acelera durante persecución.

[< volver](README.md)