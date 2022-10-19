using SharedModels;

namespace UserApi.Models
{
    public class UserConverter : IConverter<User, UserDto>
    {
        public User Convert(UserDto sharedUser)
        {
            return new User
            {
                Id = sharedUser.Id,
                Name = sharedUser.Name,
                Email = sharedUser.Email,
                Phone = sharedUser.Phone,
                BillingAddress = sharedUser.BillingAddress,
                ShippingAddress = sharedUser.ShippingAddress,
                CreditStanding = sharedUser.CreditStanding
            };
        }
        public UserDto Convert(User hiddenUser)
        {
            return new UserDto
            {
                Id = hiddenUser.Id,
                Name = hiddenUser.Name,
                Email = hiddenUser.Email,
                Phone = hiddenUser.Phone,
                BillingAddress = hiddenUser.BillingAddress,
                ShippingAddress = hiddenUser.ShippingAddress,
                CreditStanding = hiddenUser.CreditStanding
            };
        }
    }
}

