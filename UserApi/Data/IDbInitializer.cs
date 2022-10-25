namespace UserApi.Data
{
    public interface IDbInitializer
    {
        void Initialize(UserApiContext context);
    }
}
