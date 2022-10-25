using Microsoft.AspNetCore.Mvc;
using SharedModels;
using UserApi.Data;
using UserApi.Models;

namespace UserApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    
        private readonly IRepository<User> repository;
        private readonly IConverter<User, UserDto> userConverter;

        public UserController(IRepository<User> repos, IConverter<User, UserDto> converter)
        {
            repository = repos;
            userConverter = converter;
        }

    // GET user/5
    //TODO implement getting user
    [HttpGet("{id}", Name = "GetUser")]
    public IActionResult Get(int id)
    {
        var item = repository.Get(id);
        if (item == null)
        {
            return NotFound();
        }
        var userDto = userConverter.Convert(item);
        return new ObjectResult(userDto);
    }



    // POST user
    [HttpPost]
        public IActionResult Post([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }

            var user = userConverter.Convert(userDto);
            var newUser = repository.Add(user);

            return CreatedAtRoute("GetUser", new { id = newUser.Id },
                    userConverter.Convert(newUser));
        }

        // PUT user/5
        // TODO implement editing user
       
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserDto userDto)
        {
            if (userDto == null || userDto.Id != id)
            {
                return BadRequest();
            }

            var modifiedUser = repository.Get(id);

            if (modifiedUser == null)
            {
                return NotFound();
            }

            modifiedUser.Phone = userDto.Phone;
            modifiedUser.Email = userDto.Email;
            modifiedUser.BillingAddress = userDto.BillingAddress;
            modifiedUser.ShippingAddress = userDto.ShippingAddress;

            repository.Edit(modifiedUser);
            return new NoContentResult();
        }


    
    

   
}

