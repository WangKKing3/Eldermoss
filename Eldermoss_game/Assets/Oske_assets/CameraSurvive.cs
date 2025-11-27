using UnityEngine;

public class CameraSurvive : MonoBehaviour
{
    private static CameraSurvive instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
