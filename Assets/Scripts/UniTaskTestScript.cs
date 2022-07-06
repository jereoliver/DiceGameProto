using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UniTaskTestScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Foo();
        }
    }

    private static void Foo()
    {
        Debug.Log("Foo called");
        GetInt(23).Forget();
    }

    private static async UniTask<int> GetInt(int x)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        var y = x + 674;
        Debug.Log(y);
        return y;
    }
}