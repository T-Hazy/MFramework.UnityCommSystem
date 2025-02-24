using System.Threading.Tasks;
namespace MFramework.CommSystem
{
    public delegate Task StringHandler<T>(T t1) where T : StringContainer;
}