using System;

public enum EventType
{
    Join = 0,
    JoinLobby,
    Leave,
    LeaveLobby,
    UpdateRotation,
    UpdateLocation,
    StartGame,
    EndGame,
    KickPlayer
}