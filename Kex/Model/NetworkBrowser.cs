using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Kex.Model
{
    public sealed class NetworkBrowser
    {
        [DllImport("Netapi32", CharSet = CharSet.Auto,SetLastError = true),SuppressUnmanagedCodeSecurityAttribute]
        public static extern int NetServerEnum(
            string serverName, 
            int dwLevel,
            ref IntPtr pBuf,
            int dwPrefMaxLen,
            out int dwEntriesRead,
            out int dwTotalEntries,
            int dwServerType,
            string domain, 
            out int dwResumeHandle
        );

        [DllImport("Netapi32", SetLastError = true),SuppressUnmanagedCodeSecurity]
        public static extern int NetApiBufferFree(IntPtr pBuf);


        [StructLayout(LayoutKind.Sequential)]
        public struct ServerInfo
        {
            internal int PlatformId;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string Name;
        } 

        public List<string> GetNetworkComputers()
        {
            var networkComputers = new List<string>();
            const int MAX_PREFERRED_LENGTH = -1;
            int SV_TYPE_WORKSTATION = 1;
            int SV_TYPE_SERVER = 2;
            IntPtr buffer = IntPtr.Zero;
            IntPtr tmpBuffer = IntPtr.Zero;
            int entriesRead = 0;
            int totalEntries = 0;
            int resHandle = 0;
            int sizeofINFO = Marshal.SizeOf(typeof(ServerInfo));

            try
            {
                int ret = NetServerEnum(null, 100, ref buffer,
                    MAX_PREFERRED_LENGTH,
                    out entriesRead,
                    out totalEntries, SV_TYPE_WORKSTATION |
                    SV_TYPE_SERVER, null, out 
                    resHandle);

                if (ret == 0)
                {
                    for (int i = 0; i < totalEntries; i++)
                    {
                        tmpBuffer = new IntPtr((int)buffer +
                                   (i * sizeofINFO));
                        var svrInfo = (ServerInfo)Marshal.PtrToStructure(tmpBuffer,typeof(ServerInfo));
                        networkComputers.Add(svrInfo.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NetApiBufferFree(buffer);
            }

            return networkComputers;
        }
    }
}
