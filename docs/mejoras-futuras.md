# Mejoras futuras

## Unificar escenas de códice

`CodexIntro` y `CodexEnd` podrían convertirse en una sola escena `Codex` parametrizable.

## Ampliar GameManager solo si aporta valor

Actualmente `GameManager` solo establece la música principal. Podría evolucionar para controlar pausa, victoria, derrota o estados globales.

## Diferenciar enemigos

`Orc` y `Skeleton` comparten comportamiento aunque su movimiento y ataque está parametrizado. En el futuro podrían añadirse algunos otros elementos diferenciadores distintos patrones de movimiento. Esto será fundamental para la construcción de los bosses.

## Inventario dinámico

El inventario de cinco slots es suficiente para la demo. Si el número de objetos crece, podría generarse dinámicamente.

## Mejora de pathfinding enemigo

La persecución actual es simple. Si los mapas crecen, se podría añadir evasión básica de obstáculos o navegación por nodos.

[< volver](README.md)
