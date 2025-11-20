using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_changer : MonoBehaviour
{
    [SerializeField]
    private Level_connections _Connections;

    [SerializeField]
    private string _target_scene_name;

    [SerializeField]
    private Transform _spawn_point;

    private void Start()
    {
        if (_Connections == Level_connections.ActiveConnection)
        {
            FindFirstObjectByType<TestPlayerController>().transform.position = _spawn_point.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<TestPlayerController>();
        Level_connections.ActiveConnection = _Connections;
        SceneManager.LoadScene(_target_scene_name);
    }
}
