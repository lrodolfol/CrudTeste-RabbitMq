using CreateUser.Core.Entities;

namespace CreateUser.Repository
{
    public interface IUserPostRepository
    {
        void Execute(User user);
    }
}