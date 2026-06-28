# Build y plataformas

## Plataformas objetivo

- Windows
- WebGL
- Linux

## Versión de Unity

El proyecto está desarrollado con **Unity 6000.3.9f1** y **URP**.

## Checklist previa a build

Antes de generar una build final:

1. Verificar escenas en Build Settings.
2. Revisar que `CodexIntro`, `Countryside`, `CountryCave` y `CodexEnd` estén incluidas.
3. Comprobar referencias de portales por nombre de escena.
4. Revisar recursos en `Resources/SpawnOnce`.
5. Confirmar que no hay clips `Missing`, especialmente `Bow Attack`.
6. Probar viaje `Countryside → CountryCave → Countryside`.
7. Probar inventario, mensajes y muerte del jugador.

[< volver](README.md)