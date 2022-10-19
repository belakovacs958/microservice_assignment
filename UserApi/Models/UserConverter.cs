using SharedModels;

namespace UserApi.Models
{
    public class UserConverter : IConverter<Customer, UserDto>
    {
        public Customer Convert(UserDto sharedUser)
        {
            return new Customer
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
        public UserDto Convert(Customer hiddenUser)
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

