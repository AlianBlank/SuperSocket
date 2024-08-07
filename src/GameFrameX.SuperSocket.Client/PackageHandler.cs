using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GameFrameX.SuperSocket.Client
{
    public delegate ValueTask PackageHandler<TReceivePackage>(EasyClient<TReceivePackage> sender, TReceivePackage package)
        where TReceivePackage : class;
}