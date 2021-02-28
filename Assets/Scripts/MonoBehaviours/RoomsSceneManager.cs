using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsSceneManager : MonoBehaviour
{
    public static RoomsSceneManager Instance { get; private set; }

    RoomAttendant[] _roomAttendants;
    UIRoomsCanvas _uiRoomsCanvas;
    GameObject _roomsCameras; 
    PlayerController _playerController;

    public void DisableSceneObjects()
    {
        SetPlayerExplorerActive(false);
        SetRoomsCamerasActive(false);
        SetRoomsCanvasActive(false);
    }

    public void ActivateScene()
    {
        SetRoomsCamerasActive(true);
        SetRoomsCanvasActive(true);
        SetPlayerExplorerActive(true);
    }

    void SetRoomsCamerasActive(bool isActive)
    {
        if (_roomsCameras == null)
            _roomsCameras = GameObject.FindWithTag("RoomsCameras");

        _roomsCameras.SetActive(isActive);
    }

    void SetRoomsCanvasActive(bool isActive)
    {
        if (_uiRoomsCanvas == null)
            _uiRoomsCanvas = FindObjectOfType<UIRoomsCanvas>();

        _uiRoomsCanvas.gameObject.SetActive(isActive);
    }

    void SetPlayerExplorerActive(bool isActive)
    {
        if (_playerController == null)
            _playerController = FindObjectOfType<PlayerController>();

        _playerController.gameObject.SetActive(isActive);
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _roomAttendants = FindObjectsOfType<RoomAttendant>();
    }
}
