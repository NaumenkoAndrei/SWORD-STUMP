using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector3 _playerPosition;

    private void Awake()
    {
        if(!_player)
            _player = FindAnyObjectByType<Hero>().transform;
    }

    private void Update()
    {
        _playerPosition = _player.position;
        _playerPosition.z = -10f;
        _playerPosition.y += 1f;

        transform.position = Vector3.Lerp(transform.position, _playerPosition, Time.deltaTime);
    }
}
