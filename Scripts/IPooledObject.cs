// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// interface that objects that use an object pool implement
public interface IPooledObject
{
    // methods that classes that act as spawners must implement
    public void OnObjectSpawn();

    public void OnObjectReturn();
}
