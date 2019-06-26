/*
 * 
 *  RRRRRRRRRRRRRRRRR   TTTTTTTTTTTTTTTTTTTTTTT       CCCCCCCCCCCCCLLLLLLLLLLL             IIIIIIIIIIBBBBBBBBBBBBBBBBB   
 *  R::::::::::::::::R  T:::::::::::::::::::::T    CCC::::::::::::CL:::::::::L             I::::::::IB::::::::::::::::B  
 *  R::::::RRRRRR:::::R T:::::::::::::::::::::T  CC:::::::::::::::CL:::::::::L             I::::::::IB::::::BBBBBB:::::B 
 *  RR:::::R     R:::::RT:::::TT:::::::TT:::::T C:::::CCCCCCCC::::CLL:::::::LL             II::::::IIBB:::::B     B:::::B
 *    R::::R     R:::::RTTTTTT  T:::::T  TTTTTTC:::::C       CCCCCC  L:::::L                 I::::I    B::::B     B:::::B
 *    R::::R     R:::::R        T:::::T       C:::::C                L:::::L                 I::::I    B::::B     B:::::B
 *    R::::RRRRRR:::::R         T:::::T       C:::::C                L:::::L                 I::::I    B::::BBBBBB:::::B 
 *    R:::::::::::::RR          T:::::T       C:::::C                L:::::L                 I::::I    B:::::::::::::BB  
 *    R::::RRRRRR:::::R         T:::::T       C:::::C                L:::::L                 I::::I    B::::BBBBBB:::::B 
 *    R::::R     R:::::R        T:::::T       C:::::C                L:::::L                 I::::I    B::::B     B:::::B
 *    R::::R     R:::::R        T:::::T       C:::::C                L:::::L                 I::::I    B::::B     B:::::B
 *    R::::R     R:::::R        T:::::T        C:::::C       CCCCCC  L:::::L         LLLLLL  I::::I    B::::B     B:::::B
 *  RR:::::R     R:::::R      TT:::::::TT       C:::::CCCCCCCC::::CLL:::::::LLLLLLLLL:::::LII::::::IIBB:::::BBBBBB::::::B
 *  R::::::R     R:::::R      T:::::::::T        CC:::::::::::::::CL::::::::::::::::::::::LI::::::::IB:::::::::::::::::B 
 *  R::::::R     R:::::R      T:::::::::T          CCC::::::::::::CL::::::::::::::::::::::LI::::::::IB::::::::::::::::B  
 *  RRRRRRRR     RRRRRRR      TTTTTTTTTTT             CCCCCCCCCCCCCLLLLLLLLLLLLLLLLLLLLLLLLIIIIIIIIIIBBBBBBBBBBBBBBBBB   
 *    
 *    
 * Rtc interface
 * Description : super easy library for SCANLAB's RTC 5, 6, 6Ethernet
 * Author : hong chan, choi / sepwind@gmail.com (https://sepwind.blogspot.com)
 * 
 * 
 */

using System;
using System.Numerics;

namespace sepwind
{
    public enum LaserMode { Co2=0, Yag1, Yag2 };
    public enum ScannerHead { None=0, Primary, Secondary};
    public enum CorrectionTableIndex { TableNone = 0, Table1, Table2, Table3, Table4 };
    public enum RtcStatus { Busy, NotBusy, List1Busy, List2Busy, NoError, Aborted, PositionAckOK, PowerOK, TempOK };

    public interface IRtc : IDisposable
    {
        /// <summary>
        /// id no if multiple RTC cards used (0,1,2,...)
        /// </summary>
        uint Index { get; }

        #region commands for control
        bool Initialize(double bitPerMm, LaserMode laserMode, string ctbFileName);
        bool CtlLoadCorrectionFile(CorrectionTableIndex tableIndex, string ctbFileName);
        bool CtlSelectCorrection(CorrectionTableIndex primaryHeadTableIndex, CorrectionTableIndex secondaryHeadTableIndex = CorrectionTableIndex.TableNone);
        bool CtlLaserOn();
        bool CtlLaserOff();
        bool CtlMove(Vector2 position);
        bool CtlMatrix(Matrix3x2 matrix);
        bool CtlFrequency(double frequencyHz, double pulseWidthUsec);
        bool CtlDelay(double laserOnUSec, double laserOffUSec, double scannerJumpUSec, double scannerMarkUSec, double scannerPolygonUSec);
        bool CtlSpeed(double jumpMmPerSec, double markMmPerSec);
        bool CtlGetStatus(RtcStatus status);
        bool CtlBusyWait();
        bool CtlAbort();
        bool CtlReset();
        #endregion

        #region commands for list
        bool ListBegin();
        bool ListFrequency(double frequencyHz, double pulseWidthUSec);
        bool ListDelay(double laserOnUSec, double laserOffUSec, double scannerJumpUSec, double scannerMarkUSec, double scannerPolygonUSec);
        bool ListSpeed(double jumpMmPerSec, double markMmPerSec);
        bool ListWait(double msec);
        bool ListLaserOn(double msec);
        bool ListLaserOn();
        bool ListLaserOff();
        bool ListJump(Vector2 position);
        bool ListMark(Vector2 position);
        bool ListArc(Vector2 center, double sweepAngle);
        bool ListMatrix(Matrix3x2 matrix);
        bool ListEnd();
        bool ListExecute(bool busyWait=true);
        #endregion
    }
}
