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
        /// <summary>
        /// initializing rtc card
        /// </summary>
        /// <param name="bitPerMm">k factor = bits/mm</param>
        /// <param name="laserMode">Co2, YAG1,2,3,...</param>
        /// <param name="ctbFileName">.ct5 filename</param>
        /// <returns></returns>
        bool Initialize(double bitPerMm, LaserMode laserMode, string ctbFileName);
        /// <summary>
        /// load .ct5 correction file into RTC internal memory(tables)
        /// </summary>
        /// <param name="tableIndex">table 1,2,3,4 </param>
        /// <param name="ctbFileName">.ct5 filename</param>
        /// <returns></returns>
        bool CtlLoadCorrectionFile(CorrectionTableIndex tableIndex, string ctbFileName);
        /// <summary>
        /// select table(pre-loaded correction table) on Scan Head A or B
        /// </summary>
        /// <param name="primaryHeadTableIndex">Head A (Primary)</param>
        /// <param name="secondaryHeadTableIndex">Head B (Secondary)</param>
        /// <returns></returns>
        bool CtlSelectCorrection(CorrectionTableIndex primaryHeadTableIndex, CorrectionTableIndex secondaryHeadTableIndex = CorrectionTableIndex.TableNone);
        /// <summary>
        /// laser pulse on by manually (you should pre-assigned freq/pulse width by CtlFrequency)
        /// </summary>
        /// <returns></returns>
        bool CtlLaserOn();
        /// <summary>
        /// laser pulse off by manually
        /// </summary>
        /// <returns></returns>
        bool CtlLaserOff();
        /// <summary>
        /// move the scanner position by manually
        /// </summary>
        /// <param name="position">X,Y position (mm)</param>
        /// <returns></returns>
        bool CtlMove(Vector2 position);
        /// <summary>
        /// set 3*3 matrix 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        bool CtlMatrix(Matrix3x2 matrix);
        /// <summary>
        /// default laser pulse repetition rate and pulse width
        /// </summary>
        /// <param name="frequencyHz">repetition rate (Hz)</param>
        /// <param name="pulseWidthUsec">pulse width (usec)</param>
        /// <returns></returns>
        bool CtlFrequency(double frequencyHz, double pulseWidthUsec);
        /// <summary>
        /// default scanner and laser delay
        /// </summary>
        /// <param name="laserOnUSec">laser on delay (usec)</param>
        /// <param name="laserOffUSec">laser off delay (usec)</param>
        /// <param name="scannerJumpUSec">scanner jump delay (usec)</param>
        /// <param name="scannerMarkUSec">scanner mark delay (usec)</param>
        /// <param name="scannerPolygonUSec">scanner polygon delay (usec)</param>
        /// <returns></returns>
        bool CtlDelay(double laserOnUSec, double laserOffUSec, double scannerJumpUSec, double scannerMarkUSec, double scannerPolygonUSec);
        /// <summary>
        /// default scanner speed
        /// </summary>
        /// <param name="jumpMmPerSec">speed of jump (mm/s)</param>
        /// <param name="markMmPerSec">speed of mark and arc (mm/s)</param>
        /// <returns></returns>
        bool CtlSpeed(double jumpMmPerSec, double markMmPerSec);
        /// <summary>
        /// rtc internal status (like as. busy, power ok, ...)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        bool CtlGetStatus(RtcStatus status);
        /// <summary>
        /// wait until finished list commands
        /// </summary>
        /// <returns></returns>
        bool CtlBusyWait();
        /// <summary>
        /// abort working list commands immediately
        /// </summary>
        /// <returns></returns>
        bool CtlAbort();
        /// <summary>
        /// reset abort status
        /// </summary>
        /// <returns></returns>
        bool CtlReset();
        #endregion

        #region commands for list
        /// <summary>
        /// prepare internal list buffers
        /// </summary>
        /// <returns></returns>
        bool ListBegin();
        /// <summary>
        /// insert list command
        /// </summary>
        /// <param name="frequencyHz">Hz</param>
        /// <param name="pulseWidthUSec">usec</param>
        /// <returns></returns>
        bool ListFrequency(double frequencyHz, double pulseWidthUSec);
        /// <summary>
        /// insert list command
        /// </summary>
        /// <param name="laserOnUSec">usec</param>
        /// <param name="laserOffUSec">usec</param>
        /// <param name="scannerJumpUSec">usec</param>
        /// <param name="scannerMarkUSec">usec</param>
        /// <param name="scannerPolygonUSec">usec</param>
        /// <returns></returns>
        bool ListDelay(double laserOnUSec, double laserOffUSec, double scannerJumpUSec, double scannerMarkUSec, double scannerPolygonUSec);
        /// <summary>
        /// insert list command
        /// </summary>
        /// <param name="jumpMmPerSec">mm/s</param>
        /// <param name="markMmPerSec">mm/s</param>
        /// <returns></returns>
        bool ListSpeed(double jumpMmPerSec, double markMmPerSec);
        /// <summary>
        /// insert list command
        /// do nothing during assigned time
        /// </summary>
        /// <param name="msec">msec</param>
        /// <returns></returns>
        bool ListWait(double msec);
        /// <summary>
        /// insert list command
        /// laser pulses on during assiged time
        /// </summary>
        /// <param name="msec">msec</param>
        /// <returns></returns>
        bool ListLaserOn(double msec);
        /// <summary>
        /// insert list command
        /// (laser pulsed on until called ListLaserOff)
        /// </summary>
        /// <returns></returns>
        bool ListLaserOn();
        /// <summary>
        /// insert list command
        /// laser off if called ListLaserOn
        /// </summary>
        /// <returns></returns>
        bool ListLaserOff();
        /// <summary>
        /// insert list command
        /// jump scanner position
        /// </summary>
        /// <param name="position">mm</param>
        /// <returns></returns>
        bool ListJump(Vector2 position);
        /// <summary>
        /// insert list command
        /// mark(draw line) scanner position
        /// </summary>
        /// <param name="position">mm</param>
        /// <returns></returns>
        bool ListMark(Vector2 position);
        /// <summary>
        /// insert list command
        /// arc(draw arc)
        /// start position = current position
        /// center position = assigned
        /// sweep angle = + : CCW , - : CW
        /// </summary>
        /// <param name="center">center position of arc</param>
        /// <param name="sweepAngle">degree  (+ : CCW, - : CW)(</param>
        /// <returns></returns>
        bool ListArc(Vector2 center, double sweepAngle);
        /// <summary>
        /// insert list command
        /// set 3*3 matrix 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        bool ListMatrix(Matrix3x2 matrix);
        /// <summary>
        /// close internal list buffers
        /// </summary>
        /// <returns></returns>
        bool ListEnd();
        /// <summary>
        /// execute inserted list commands if remains
        /// </summary>
        /// <param name="busyWait">wait until whole list commands finished</param>
        /// <returns></returns>
        bool ListExecute(bool busyWait=true);
        #endregion
    }
}
