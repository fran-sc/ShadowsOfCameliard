# UI y mensajes

La UI se divide en varios sistemas independientes.

| Sistema | Elementos |
|---|---|
| `UIManager` | Corazones de vida e icono de dash. |
| `InventoryManager` | Panel `INVENTARIO` con cinco slots. |
| `MessageManager` | Panel de mensajes contextuales. |
| `UIFade` / `PortalCanvas` | Fade de transiciones. |

## HUD

El HUD visible durante gameplay muestra corazones en la esquina superior izquierda y un icono de _dash_ bajo la vida.

Este icono se anima durante la recarga mediante el rellenado horizontal del sprite.

## Inventario

El inventario aparece centrado sobre la escena. Tiene cinco slots, selección mediante borde claro y muestra los objetos obtenidos.

## Mensajes

Los mensajes contextuales aparecen centrados y se cierran con `X`. Sirven para instrucciones, objetos, señales y frases narrativas.

Ejemplo:

> El "Acero de Benwick". Una hoja humilde en manos de un héroe puede cambiar el destino de un reino.

[< volver](README.md)