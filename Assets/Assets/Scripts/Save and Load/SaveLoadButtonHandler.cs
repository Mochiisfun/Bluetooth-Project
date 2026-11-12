
using UnityEngine;



public class SaveLoadButtonHandler : MonoBehaviour
{
    public void SaveGame()
    {
        GameVariableInitializer.SaveCurrentGame();
    }

    public void LoadGame()
    {
        GameVariableInitializer.LoadCurrentGame();
    }
}
