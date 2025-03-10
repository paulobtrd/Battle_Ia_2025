﻿namespace BattleIA
{

    public enum BotState : byte
    {
        Undefined = 0,
        WaitingGUID = 1,
        ErrorGUID = 2,
        Ready = 3,
        Error = 4,
        Disconnect = 5,
        WaitingAnswerD = 6,
        WaitingAction = 7,
        IsDead = 8,
    }

    public enum CaseState : byte
    {
        Empty = 0,
        // OurBot = 1,
        Wall = 2,
        Energy = 3,
        Ennemy = 4,
        Bonus = 5,
        Malus = 6,
        Virtual = 7 // to search a path, not use by the server, it's for you :)
    }

    public enum BotAction : byte
    {
        None = 0,
        Move = 1,
        ShieldLevel = 2,
        CloakLevel = 3,
        Shoot = 4,
    }

    public enum MessageSize : byte
    {
        Dead = 1,
        OK = 2,
        Position = 3,
        Turn = 9,
        Change = 7,
    }

    public enum MoveDirection : byte
    {
        North = 1,
        East = 2,
        South = 3,
        West = 4,
        /*NorthWest = 5,
        SouthWest = 6,
        SouthEast = 7,
        NorthEast = 8,
        */
    }

}
