using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public enum EventCode
    {
    None=0,
    Common=1,
    ClosetWall=2,
    CanelClosetWall=3,
    HangUp=4,
    HangDown=5,
    HangToClimb=6,
    StandToDownHang=7,
    ClimbUpLadder=8,

    #region UI
    OpenInventory=5000,
    OpenCharacter=5001,
    TestGraphRecognition=5001,
    #endregion

    #region BuffEventCode
    PickedUp = 10001,
    Force=10002,
    #endregion
    #region  DEBUG
    Test=-100,
    Debug=-101,
    #endregion
}

