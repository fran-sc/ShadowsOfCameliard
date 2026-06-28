# Audio

El audio se gestiona desde `AudioManager`, con dos fuentes diferenciadas:

- música;
- efectos.

## Música

`GameManager` asigna el tema principal al iniciar la partida:

```csharp
AudioManager.Instance.PlayMusic(mainTheme);
```

`AudioManager` reproduce el clip en bucle y permite detenerlo con o sin fade.

## Efectos

Los efectos se reproducen mediante un enum `AudioManager.Effect`, evitando referencias directas a clips desde todos los scripts.

Ejemplos:

- ataque de espada;
- bloqueo de escudo;
- ataque de hacha;
- disparo de arco;
- muerte de orco;
- muerte de esqueleto;
- muerte del jugador;
- recolección;
- explosión;
- curación.


[< volver](README.md)