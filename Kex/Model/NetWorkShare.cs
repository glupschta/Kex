using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Kex.Model
{
    /// <summary>
    /// Based on http://www.pinvoke.net/default.aspx/netapi32/netshareenum.html
    /// </summary>
    public class NetWorkShare
    {

        [DllImport("Netapi32.dll", SetLastError = true)]
        static extern int NetApiBufferFree(IntPtr Buffer);

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int NetShareEnum(
             StringBuilder ServerName,
             int level,
             ref IntPtr bufPtr,
             uint prefmaxlen,
             ref int entriesread,
             ref int totalentries,
             ref int resume_handle
             );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ShareInfo
        {
            public string shi1_netname;
            public uint shi1_type;
            public string shi1_remark;
            public ShareInfo(string sharename, uint sharetype, string remark)
            {
                shi1_netname = sharename;
                shi1_type = sharetype;
                shi1_remark = remark;
            }
            public override string ToString()
            {
                return shi1_netname;
            }
        }

        const uint MAX_PREFERRED_LENGTH = 0xFFFFFFFF;
        const int NERR_Success = 0;

        private enum NetError : uint
        {
            NERR_Success = 0,
            NERR_BASE = 2100,
            NERR_UnknownDevDir = (NERR_BASE + 16),
            NERR_DuplicateShare = (NERR_BASE + 18),
            NERR_BufTooSmall = (NERR_BASE + 23),
        }

        private enum SHARE_TYPE : uint
        {
            STYPE_DISKTREE = 0,
            STYPE_PRINTQ = 1,
            STYPE_DEVICE = 2,
            STYPE_IPC = 3,
            STYPE_SPECIAL = 0x80000000,
        }

        public ShareInfo[] GetShares(string serverName)
        {
            var shareInfos = new List<ShareInfo>();
            int entriesread = 0;
            int totalentries = 0;
            int resumeHandle = 0;
            int nStructSize = Marshal.SizeOf(typeof(ShareInfo));
            var bufPtr = IntPtr.Zero;
            var server = new StringBuilder(serverName);
            int ret = NetShareEnum(server, 1, ref bufPtr, MAX_PREFERRED_LENGTH, ref entriesread, ref totalentries, ref resumeHandle);
            if (ret == NERR_Success)
            {
                var currentPtr = bufPtr;
                for (int i = 0; i < entriesread; i++)
                {
                    var shi1 = (ShareInfo)Marshal.PtrToStructure(currentPtr, typeof(ShareInfo));
                    shareInfos.Add(shi1);
                    currentPtr = new IntPtr(currentPtr.ToInt32() + nStructSize);
                }
                NetApiBufferFree(bufPtr);
                return shareInfos.ToArray();
            }
            throw new Exception("Error while retrieving Network Share: " + ret);
        }
    }

}
