using System;

public enum EventType
{
    Acknowledge = 0,
    Join,
    JoinLobby,
    Leave,
    LeaveLobby,
    UpdateRotation,
    UpdateLocation,
    StartGame,
    EndGame,
    KickPlayer,
    MessageSent,
    MessageReceived
}