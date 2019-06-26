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
 * interface for field correction file
 * Description : super easy library for SCANLAB's RTC 5, 6, 6Ethernet
 * Author : hong chan, choi / sepwind@gmail.com (https://sepwind.blogspot.com)
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.Numerics;

namespace sepwind
{
    interface IRtcCorrection
    {
        /// <summary>
        /// absolute position values (x, y are measured location values)
        /// z value reserved for 3d calibration.
        /// </summary>
        /// <param name="v">x, y, (z)</param>
        /// <returns></returns>
        bool Add(Vector3 v);

        bool AddRange(IEnumerable<Vector3> v);

        /// <summary>
        /// clear all data
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// count of added values
        /// </summary>
        uint Count();

        /// <summary>
        /// try to convert to create new ct5 file
        /// </summary>
        /// <returns></returns>
        bool Convert();

        /// <summary>
        /// result string
        /// </summary>
        /// <returns></returns>
        string Result();      
    }
}
