using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;

public class CrashesButtonController : MonoBehaviour
{
    [DllImport("nativecrash")]
    private static extern void crash();

    public void OnClickOneSec()
    {
        System.Threading.Thread.Sleep(1_000);
    }

    public void OnClickFiveSec()
    {
        System.Threading.Thread.Sleep(5_000);
    }

    public void OnClickTenSec()
    {
        System.Threading.Thread.Sleep(10_000);
    }

    public void OnClickNullReferenceException()
    {
        Debug.Log("Triggering NullReferenceException...");
        GameObject nullObject = null;
        nullObject.transform.position = Vector3.zero;
    }

    public void OnClickDivideByZeroException()
    {
        Debug.Log("Triggering DivideByZeroException...");
        int x = 1;
        int y = 0;
        int result = x / y;
    }

    public void OnClickIndexOutOfRangeException()
    {
        Debug.Log("Triggering IndexOutOfRangeException...");
        int[] array = new int[1];
        int value = array[5];
    }

    public void OnClickCustomException()
    {
        Debug.Log("Triggering Custom Exception...");
        throw new CustomGameException("This is a custom game exception!");
    }

    public void OnClickNativeCrash()
    {
        Debug.Log("Trigger native plugin crash");
        crash();
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }
}

public class CustomGameException : Exception
{
    public CustomGameException(string message) : base(message)
    {
    }
}