# Códice narrativo

La narración se presenta mediante escenas de códice medieval: `CodexIntro` y `CodexEnd`.

## Presentación visual

Cada pantalla usa composición de doble página:

- Ilustración medieval a la izquierda
- Texto narrativo a la derecha
- Fondo de pergamino
- Estética de manuscrito iluminado

## CodexIntro

Introduce la premisa: Arturo en Francia, Ginebra en Cameliard, invasión de Mordred y misión de Lancelot.

## CodexEnd

Reutiliza el sistema visual, añade páginas del último capítulo jugado y se posiciona automáticamente en la primera página nueva.

## Mejora futura

Ambas escenas podrían unificarse en una única escena `Codex` parametrizable con:

- Página inicial
- Páginas activas
- Modo intro/final
- Escena destino
- Avance manual o automático

## Implementación 

EL códice se basa en un sistema de páginas que define un mesh parametrizado y le aplica dos animaciones:

1. Rotación (en ambos sentidos) sobre el eje del lomo
2. Curvatura (dependiente del sentido de movimiento) de la página durante la rotación

Para poder generar el efecto de curvatura en el paso de las páginas, en el momento de crear cada una de las páginas ([**`CodexPage.GeneratePage(int xSegments, int ySegments)`**](../ShadowsOfCameliard/Assets/Scripts/Codex/CodexPage.cs)), se crea y almacena la malla de la misma. 

A dicha malla se le aplican dos materiales independientes, correspondientes a cada una de las páginas (caras) de la hoja del códice.

En el momento del paso de la página, se aplica tanto una rotación global a los vertices de la malla (rotación de la página) ([**`TurnPageCoroutine(bool forward = true)`**](../ShadowsOfCameliard/Assets/Scripts/Codex/CodexPage.cs)) así como un efecto parametrizable de curvatura de la misma ([**`ApplyCurl(float progress, bool forward = true)`**](../ShadowsOfCameliard/Assets/Scripts/Codex/CodexPage.cs)).

[< volver](README.md)