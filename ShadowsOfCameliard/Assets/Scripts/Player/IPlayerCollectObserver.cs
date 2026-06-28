// -----------------------------------------------------------------------------
// IPlayerCollectObserver
//
// Patrón Observer para notificar a múltiples sistemas cuando el jugador recoge
// un item. Los observadores reciben el itemID del objeto recogido para reaccionar
// según su lógica específica.
// -----------------------------------------------------------------------------
public interface IPlayerCollectObserver
{
    void OnNotify(string itemID);
}