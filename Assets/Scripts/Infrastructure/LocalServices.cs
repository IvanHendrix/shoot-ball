using Services;

namespace Infrastructure
{
    public class LocalServices
    {
        private static LocalServices _instance;
        public static LocalServices Container => _instance ?? (_instance = new LocalServices());

        public void RegisterSingle<TService>(TService implementation) where TService : IService =>
            Implementation<TService>.ServiceInstance = implementation;

        public TService Single<TService>() where TService : IService =>
            Implementation<TService>.ServiceInstance;

        private class Implementation<TService> where TService : IService
        {
            public static TService ServiceInstance;
        }
    }
}