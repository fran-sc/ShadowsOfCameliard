# Sombras de Cameliard - Wiki técnica

Documentación técnica de la demo **Sombras de Cameliard**, un juego 2D top-down de acción/RPG desarrollado en **Unity 6000.3.9f1**, con **URP**, estética **pixel art** y objetivo de build para **Windows**, **WebGL** (pendiente) y **Linux**.

La wiki documenta la arquitectura real de la demo: escenas, managers, jugador, enemigos, inventario, recursos persistentes, portales, UI, audio, códice narrativo y posibles mejoras.

## Índice principal

1. [Introducción del proyecto](introduccion.md)
2. [Estructura del proyecto](estructura-del-proyecto.md)
3. [Escenas](escenas/README.md)
4. [Sistemas principales](sistemas/README.md)
5. [Lancelot / jugador](jugador/README.md)
6. [Enemigos](enemigos/README.md)
7. [Inventario y recursos](recursos/README.md)
8. [Portales y cambio de escena](escenas/portales.md)
9. [UI y mensajes](ui-y-mensajes.md)
10. [Audio](audio.md)
11. [Códice narrativo](codice-narrativo.md)
12. [Recursos gráficos y sonoros](recursos-graficos-y-sonoros.md)
13. [Build y plataformas](build-y-plataformas.md)
14. [Mejoras futuras](mejoras-futuras.md)

## Lectura recomendada

Para entender el proyecto rápidamente:

- [Introducción](introduccion.md)
- [Estructura del proyecto](estructura-del-proyecto.md)
- [Escena Countryside](escenas/countryside.md)
- [Sistema de portales](escenas/portales.md)
- [Sistemas principales](sistemas/README.md)
- [Flujo de inventario y recursos](recursos/flujo-inventario-recursos.md)
- [Lancelot](jugador/lancelot.md)
- [Enemigos](enemigos/README.md)

## Convenciones técnicas

- Los sistemas globales que deben sobrevivir entre escenas heredan de `PersistentSingleton<T>`.
- Los sistemas propios de una escena pueden usar `SceneSingleton<T>`.
- El input se centraliza en `InputManager` y se organiza por mapas de control.
- Los recursos persistentes se definen mediante `ScriptableObject` y se clonan en ejecución para no modificar los assets originales.
- La demo no concentra la lógica en un `GameManager` monolítico: usa subsistemas pequeños e independientes.
- La documentación prioriza describir la versión funcional actual, no una arquitectura ideal futura.
