using System.Collections.Generic;

// -----------------------------------------------------------------------------
// PlayerCollectSubject
//
// Responsabilidades:
// - Gestiona la lista de observadores del patrón Observer de recogida de items.
// - Notifica a todos los observadores cuando el jugador recoge un item.
//
// Notas:
// - Los observadores se registran llamando a AddObserver() en su Start().
// - NotifyObservers() es llamado desde SpawnedItemCollect al colisionar.
// -----------------------------------------------------------------------------
public class PlayerCollectSubject : SceneSingleton<PlayerCollectSubject>
{   
    List<IPlayerCollectObserver> observers = new List<IPlayerCollectObserver>();
    
    // Añade el observador solo si no estaba ya registrado
    public void AddObserver(IPlayerCollectObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IPlayerCollectObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    // -----------------------------------------------------------------------------
    // NotifyObservers
    //
    // - Itera sobre todos los observadores registrados y les envía el itemID recogido.
    // -----------------------------------------------------------------------------
    public void NotifyObservers(string itemID)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(itemID);
        }
    }
}
