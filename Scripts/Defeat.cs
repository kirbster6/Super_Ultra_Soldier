// Author: Tien-Yi Lee

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defeat : MonoBehaviour
{
    public void ClickToContinue()
    {
        GameStateManager.QuitToTitle();
    }
}
