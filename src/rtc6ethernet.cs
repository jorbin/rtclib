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
 * Rtc6 ethernet concrete class
 * Description : super easy library for SCANLAB's RTC 5, 6, 6Ethernet
 * Author : hong chan, choi / sepwind@gmail.com (https://sepwind.blogspot.com)
 * 
 * 
 */

using System.Diagnostics;
using RTC6Import;
using System.Net;

namespace sepwind
{
    internal sealed class Rtc6Ethernet : Rtc6 
    {
        private IPAddress ipAddress;
        private IPAddress subNetMask;
        private bool disposed = false;

        public Rtc6Ethernet(uint index, string ipAddress, string subNetMask=@"255.255.255.0")
            : base(index)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.subNetMask = IPAddress.Parse(subNetMask);
        }
        public override bool Initialize(double kFactor, LaserMode laserMode, string ctbFileName)
        {
            Debug.Assert(base.kFactor > 0);
            uint result = RTC6Wrap.init_rtc6_dll();
            RTC6Wrap.eth_set_search_cards_timeout(200 * 1000); 
            result = RTC6Wrap.eth_search_cards(
                RTC6Wrap.eth_convert_string_to_ip(this.ipAddress.ToString()), 
                RTC6Wrap.eth_convert_string_to_ip(this.subNetMask.ToString()));
            int assign_result = RTC6Wrap.eth_assign_card_ip(
                RTC6Wrap.eth_convert_string_to_ip(this.ipAddress.ToString()), 
                base.Index + 1);
            if (base.Index + 1 != assign_result)           
                return false;

            RTC6Wrap.n_stop_execution(base.Index + 1);
            uint error = RTC6Wrap.n_load_program_file(base.Index + 1, "");            
            uint cardCnt = RTC6Wrap.rtc6_count_cards();
            uint dllVersion = RTC6Wrap.get_dll_version();
            uint hexVersion = RTC6Wrap.get_hex_version();
            uint rtcVersion = RTC6Wrap.get_rtc_version();
           
            uint lastError = RTC6Wrap.n_get_last_error(base.Index + 1);           
            base.kFactor = kFactor;
            ///active high
            RTC6Wrap.n_set_laser_control(this.Index + 1, 0);

            if (!this.CtlLoadCorrectionFile(CorrectionTableIndex.Table1, ctbFileName))
                return false;
            if (!this.CtlSelectCorrection(CorrectionTableIndex.Table1))
                return false;
            RTC6Wrap.n_config_list(this.Index + 1, 4000 * 2, 4000 * 2);
            RTC6Wrap.n_set_laser_mode(this.Index + 1, (uint)laserMode);
            RTC6Wrap.n_set_firstpulse_killer(this.Index + 1, 0);
            RTC6Wrap.n_set_standby(this.Index + 1, 0, 0);
            return true;
        }
    }
}
