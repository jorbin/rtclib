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
 * Rtc5 concrete class
 * Description : super easy library for SCANLAB's RTC 5, 6, 6Ethernet
 * Author : hong chan, choi / sepwind@gmail.com (https://sepwind.blogspot.com)
 * 
 * 
 */

using System;
using System.Diagnostics;
using RTC5Import;
using System.Numerics;

namespace sepwind
{

    public sealed class Rtc5 : IRtc
    {
        public uint Index { get; }

        private double kFactor;
        private string[] ctbFileName = new string[4 + 1];
        private bool aborted;
        private uint listIndex;
        private uint listCount;
        private Matrix3x2 matrix;
        private bool disposed = false;

        public Rtc5(uint index)
        {
            this.Index = index;
            this.listIndex = 1;
            this.matrix = new Matrix3x2();
            this.matrix = Matrix3x2.Identity;
        }
        ~Rtc5()
        {
            if (this.disposed)
                return;
            this.Dispose(false);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
            }
            this.disposed = true;
        }
        public bool Initialize(double kFactor, LaserMode laserMode, string ctbFileName)
        {
            Debug.Assert(kFactor > 0);
            RTC5Wrap.n_stop_execution(this.Index + 1);
            uint error = RTC5Wrap.n_load_program_file(this.Index + 1, string.Empty);
            uint cardCnt = RTC5Wrap.rtc5_count_cards();
            uint dllVersion = RTC5Wrap.get_dll_version();
            uint hexVersion = RTC5Wrap.get_hex_version();
            uint rtcVersion = RTC5Wrap.get_rtc_version();
            uint lastError = RTC5Wrap.n_get_last_error(this.Index + 1);            
            uint serialNo = RTC5Wrap.n_get_serial_number(this.Index + 1);
            this.kFactor = kFactor;
            ///active high
            RTC5Wrap.n_set_laser_control(this.Index + 1, 0);

            if (!this.CtlLoadCorrectionFile(CorrectionTableIndex.Table1, ctbFileName))
                return false;
            if (!this.CtlSelectCorrection(CorrectionTableIndex.Table1))
                return false;
            RTC5Wrap.n_config_list(this.Index + 1, 4_000 * 2, 4_000 * 2);
            RTC5Wrap.n_set_laser_mode(this.Index + 1, (uint)laserMode);
            RTC5Wrap.n_set_firstpulse_killer(this.Index + 1, 0);
            RTC5Wrap.n_set_standby(this.Index + 1, 0, 0);
            return true;
        }
        public bool CtlLoadCorrectionFile(CorrectionTableIndex tableIndex, string ctbFileName)
        {
            if (this.CtlGetStatus(RtcStatus.Busy))            
                return false;
            this.ctbFileName[(uint)tableIndex] = ctbFileName;
            uint error = RTC5Wrap.n_load_correction_file(this.Index + 1,
                    ctbFileName, Convert.ToUInt32(tableIndex), 2 );
            return true;
        }
        public bool CtlSelectCorrection(CorrectionTableIndex primaryHeadTableIndex, CorrectionTableIndex secondaryHeadTableIndex = CorrectionTableIndex.TableNone)
        {
            RTC5Wrap.n_select_cor_table(this.Index + 1,
                Convert.ToUInt32(primaryHeadTableIndex),
                Convert.ToUInt32(secondaryHeadTableIndex));
            return true;
        }
        public bool CtlLaserOn()
        {
            RTC5Wrap.n_laser_signal_on(this.Index + 1);
            return true;
        }
        public bool CtlLaserOff()
        {
            RTC5Wrap.n_laser_signal_off(this.Index + 1);
            return true;
        }
        public bool CtlMove(Vector2 vPosition)
        {           
            Vector2 v = Vector2.Transform(vPosition, this.matrix);
            int xBits = Convert.ToInt32(v.X * this.kFactor);
            int yBits = Convert.ToInt32(v.Y * this.kFactor);
            RTC5Wrap.n_goto_xy(this.Index + 1, xBits, yBits);            
            return true;
        }        
        public bool CtlMatrix(Matrix3x2 matrix)
        {
            this.matrix = matrix;
            return true;
        }
        public bool CtlFrequency(double frequency, double pulseWidth)
        {
            if (this.CtlGetStatus(RtcStatus.Busy))
                return false;
            double period = 1.0f / frequency * (double)1.0e6;        
            double halfPeriod = period / 2.0f;
            RTC5Wrap.n_set_start_list(this.Index + 1, 1);
            RTC5Wrap.n_set_laser_timing(this.Index + 1,
                (uint)(halfPeriod * 64.0),
                (uint)(pulseWidth * 64.0),
                0,
                0);
            RTC5Wrap.n_set_end_of_list(this.Index + 1);
            RTC5Wrap.n_execute_list(this.Index + 1, 1);
            this.CtlBusyWait();
            return true;
        }
        public bool CtlDelay(double laserOn, double laserOff, double scannerJump, double scannerMark, double scannerPolygon)
        {
            if (this.CtlGetStatus(RtcStatus.Busy))
                return false;
            RTC5Wrap.n_set_start_list(this.Index + 1, 1);
            RTC5Wrap.n_set_scanner_delays(this.Index + 1,
                (uint)(scannerJump / 10.0),
                (uint)(scannerMark / 10.0),
                (uint)(scannerPolygon / 10.0)
                );
            RTC5Wrap.n_set_laser_delays(this.Index + 1,
                (int)(laserOn * 2.0),
                (uint)(laserOff * 2.0)
                );
            RTC5Wrap.n_set_end_of_list(this.Index + 1);
            RTC5Wrap.n_execute_list(this.Index + 1, 1);
            this.CtlBusyWait();
            return true;
        }
        public bool CtlSpeed(double jump, double mark)
        {
            if (this.CtlGetStatus(RtcStatus.Busy))
                return false;
            double jump_bitperms = jump / (double)1.0e3 * this.kFactor;
            double mark_bitperms = mark / (double)1.0e3 * this.kFactor;
            RTC5Wrap.n_set_jump_speed_ctrl(this.Index + 1, jump_bitperms);
            RTC5Wrap.n_set_mark_speed_ctrl(this.Index + 1, mark_bitperms);
            return true;
        }        
        public bool CtlGetStatus(RtcStatus status)
        {
            bool result = false;
            uint busy, position;
            busy = position = 0;
            switch (status)
            {
                case RtcStatus.Busy:
                    RTC5Wrap.n_get_status(this.Index + 1, out busy, out position);
                    result = Convert.ToBoolean(busy > 0);
                    break;
                case RtcStatus.NotBusy:
                    result = !this.CtlGetStatus(RtcStatus.Busy);
                    break;
                case RtcStatus.List1Busy:
                    uint l1Status = RTC5Wrap.n_read_status(this.Index + 1);
                    result = Convert.ToBoolean(l1Status & 0x0F);
                    break;
                case RtcStatus.List2Busy:
                    uint l2Status = RTC5Wrap.n_read_status(this.Index + 1);
                    result = Convert.ToBoolean(l2Status & 0x10);
                    break;
                case RtcStatus.NoError:
                    bool aborted = this.CtlGetStatus(RtcStatus.Aborted);                    
                    uint lastError = RTC5Wrap.n_get_last_error(this.Index + 1);
                    bool error = 0 != lastError;
                    result = !aborted && !error;
                    break;
                case RtcStatus.Aborted:
                    result = this.aborted;
                    break;
                case RtcStatus.PositionAckOK:
                    uint posAckStatus = RTC5Wrap.n_get_head_status(this.Index + 1, 1);
                    result = Convert.ToBoolean(posAckStatus & 0x0F + posAckStatus & 0x10);
                    break;
                case RtcStatus.PowerOK:
                    uint powStatus = RTC5Wrap.n_get_head_status(this.Index + 1, 1);
                    result = Convert.ToBoolean(powStatus & 0x80);
                    break;
                case RtcStatus.TempOK:
                    uint tempStatus = RTC5Wrap.n_get_head_status(this.Index + 1, 1);
                    result = Convert.ToBoolean(tempStatus & 0x40);
                    break;
            }
            return result;
        }
        public string CtlGetErrMsg(uint errorCode)
        {
            if (0 == errorCode)
                return string.Empty;
            uint errCode = RTC5Wrap.n_get_error(this.Index + 1);
            if (Convert.ToBoolean(errCode & (0x01 << 0)))
                return ("no rtc board founded via init_rtc_dll");
            if (Convert.ToBoolean(errCode & (0x01 << 1)))
                return ("access denied via init_rtc_dll, select, acquire_rtc");
            if (Convert.ToBoolean(errCode & (0x01 << 2)))
                return ("command not forwarded. PCI or driver error");
            if (Convert.ToBoolean(errCode & (0x01 << 3)))
                return ("rtc timed out. no response from board");
            if (Convert.ToBoolean(errCode & (0x01 << 4)))
                return ("invalid parameter");
            if (Convert.ToBoolean(errCode & (0x01 << 5)))
                return ("list processing is (not) active");
            if (Convert.ToBoolean(errCode & (0x01 << 6)))
                return ("list command rejected, illegal input pointer");
            if (Convert.ToBoolean(errCode & (0x01 << 7)))
                return ("list command wad converted to a List_mop");
            if (Convert.ToBoolean(errCode & (0x01 << 8)))
                return ("dll, rtc or hex version error");
            if (Convert.ToBoolean(errCode & (0x01 << 9)))
                return ("download verification error. load_program_file ?");
            if (Convert.ToBoolean(errCode & (0x01 << 10)))
                return ("DSP version is too old");
            if (Convert.ToBoolean(errCode & (0x01 << 11)))
                return ("out of memeory. dll internal windows memory request failed");
            if (Convert.ToBoolean(errCode & (0x01 << 12)))
                return ("EEPROM read or write error");
            if (Convert.ToBoolean(errCode & (0x01 << 16)))
                return ("error reading PCI configuration reqister druing init_rtc_dll");
            return ($"unknown error code : {errorCode}");
        }
        public bool CtlBusyWait()
        {
            uint busy, position;
            do {
                System.Threading.Thread.Sleep(1);
                RTC5Wrap.n_get_status(this.Index + 1, out busy, out position);
            }
            while (0 != busy);
            return true;
        }
        public bool CtlAbort()
        {
            RTC5Wrap.n_stop_execution(this.Index + 1);
            this.aborted = true;
            return this.CtlGetStatus(RtcStatus.NotBusy);
        }
        public bool CtlReset()
        {
            uint lastError = RTC5Wrap.n_get_last_error(this.Index + 1);
            if (0 != lastError)
                RTC5Wrap.n_reset_error(this.Index + 1, lastError);

            this.aborted = false;
            return true;
        }
        public bool ListBegin()
        {
            Debug.Assert(this.CtlGetStatus(RtcStatus.NotBusy));
            this.listIndex = 1;
            this.listCount = 0;
            this.aborted = false;
            RTC5Wrap.n_set_start_list(this.Index + 1, this.listIndex);            
            return true;
        }
        public bool ListFrequency(double frequency, double pulseWidth)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            double period = 1.0f / frequency * (double)1.0e6;    
            double halfPeriod = period / 2.0f;
            if (!this.IsListReady(1))
                return false;
            RTC5Wrap.n_set_laser_timing(this.Index + 1,
                (uint)(halfPeriod * 64.0),
                (uint)(pulseWidth * 64.0),
                0, 0);
            return true;
        }
        public bool ListDelay(double laserOn, double laserOff, double scannerJump, double scannerMark, double scannerPolygon)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            if (!this.IsListReady(2))
                return false;
            RTC5Wrap.n_set_scanner_delays(this.Index + 1,
                (uint)(scannerJump / 10.0),
                (uint)(scannerMark / 10.0),
                (uint)(scannerPolygon / 10.0)
                );
            RTC5Wrap.n_set_laser_delays(this.Index + 1,
                (int)(laserOn * 2.0),
                (uint)(laserOff * 2.0)
                );
            return true;
        }
        public bool ListSpeed(double jump, double mark)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            double jump_bitperms = jump / (double)1.0e3 * this.kFactor;
            double mark_bitperms = mark / (double)1.0e3 * this.kFactor;
            if (!this.IsListReady(2))
                return false;
            RTC5Wrap.n_set_jump_speed(this.Index + 1, jump_bitperms);
            RTC5Wrap.n_set_mark_speed(this.Index + 1, mark_bitperms);
            return true;
        }
        public bool ListWait(double msec)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            if (msec * 1.0e3 >= 10)
            {
                if (!this.IsListReady(1))
                    return false;
                RTC5Wrap.n_long_delay(this.Index + 1, (uint)(msec * 100.0)); 
            }
            return true;
        }
        public bool ListLaserOn(double msec)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            if (!this.IsListReady(1))
                return false;
            RTC5Wrap.n_laser_on_list(this.Index + 1, (uint)(msec / 100.0));
            return true;
        }
        public bool ListLaserOn()
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            if (!this.IsListReady(1))
                return false;
            RTC5Wrap.n_laser_signal_on_list(this.Index + 1);
            return true;
        }
        public bool ListLaserOff()
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            if (!this.IsListReady(1))
                return false;
            RTC5Wrap.n_laser_signal_off_list(this.Index + 1);
            return true;
        }
        public bool ListJump(Vector2 vPosition)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;            
            Vector2 v = Vector2.Transform(vPosition, this.matrix);
            int xBits = (int)(v.X * this.kFactor);
            int yBits = (int)(v.Y * this.kFactor);            
            if (!this.IsListReady(1))
                return false;

            RTC5Wrap.n_jump_abs(this.Index + 1, xBits, yBits);
            return true;
        }
        public bool ListMark(Vector2 vPosition)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;

            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            Vector2 v = Vector2.Transform(vPosition, this.matrix);
            int xBits = (int)(v.X * this.kFactor);
            int yBits = (int)(v.Y * this.kFactor);
            if (!this.IsListReady(1))
                return false;

            RTC5Wrap.n_mark_abs(this.Index + 1, xBits, yBits);
            return true;
        }
        public bool ListArc(Vector2 center, double sweepAngle)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
          
            Vector2 v = Vector2.Transform(center, this.matrix);
            int quot = (int)(Math.Abs(sweepAngle) / 360.0);
            double rem = sweepAngle - Math.Sign(sweepAngle) * 360.0f * quot;
            if (!this.IsListReady((uint)(quot + 1)))
                return false;
            for (int i = 0; i < quot; i++)
                RTC5Wrap.n_arc_abs(this.Index + 1, (int)(center.X * this.kFactor), (int)(center.Y * this.kFactor), Math.Sign(sweepAngle) * -360.0);
            RTC5Wrap.n_arc_abs(this.Index + 1, (int)(center.X * this.kFactor), (int)(center.Y * this.kFactor), -rem);
            return true;
        }
        public bool ListMatrix(Matrix3x2 matrix)
        {
            this.matrix = matrix;
            return true;
        }
        public bool ListEnd()
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            RTC5Wrap.n_set_end_of_list(this.Index + 1);            
            return true;
        }
        public bool ListExecute(bool busyWait=true)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            uint busy, position;
            busy = position = 0;
            RTC5Wrap.n_get_status(this.Index + 1, out busy, out position);
            if (busy > 0)
                RTC5Wrap.n_auto_change(this.Index + 1);
            if (busyWait)
                this.CtlBusyWait();
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            return true;
        }        
        private bool IsListReady(uint count)
        {
            if (this.CtlGetStatus(RtcStatus.Aborted))
                return false;
            const uint RTC5_LIST_BUFFER_MAX = 4000;
            if ((this.listCount + count) >= RTC5_LIST_BUFFER_MAX)
            {
                uint busy, position;
                busy = position = 0;
                RTC5Wrap.n_get_status(this.Index + 1, out busy, out position);
                if (0 != busy)
                {
                    RTC5Wrap.n_set_end_of_list(this.Index + 1);
                    RTC5Wrap.n_execute_list(this.Index + 1, this.listIndex);
                    this.listIndex = this.listIndex ^ 0x03;
                    RTC5Wrap.n_set_start_list(this.Index + 1, this.listIndex);
                }
                else
                {
                    RTC5Wrap.n_set_end_of_list(this.Index + 1);
                    if (this.CtlGetStatus(RtcStatus.Aborted))
                        return false;

                    RTC5Wrap.n_auto_change(this.Index + 1);
                    uint readStatus = 0;
                    switch (this.listIndex)
                    {
                        case 1:
                            do {
                                readStatus = RTC5Wrap.n_read_status(this.Index + 1);
                                System.Threading.Thread.Sleep(1);
                            }
                            while (Convert.ToBoolean(readStatus & 0x20));
                            break;

                        case 2:
                            do {
                                readStatus = RTC5Wrap.n_read_status(this.Index + 1);
                                System.Threading.Thread.Sleep(1);
                            }
                            while (Convert.ToBoolean(readStatus & 0x10));
                            break;
                    }
                    if (this.CtlGetStatus(RtcStatus.Aborted))
                        return false;
                    this.listIndex = this.listIndex ^ 0x03;
                    RTC5Wrap.n_set_start_list(this.Index + 1, this.listIndex);
                }
                this.listCount = count;
            }
            this.listCount += count;
            return true;
        }
    }
}
