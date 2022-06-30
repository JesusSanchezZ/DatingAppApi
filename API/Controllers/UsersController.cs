using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.DTOs;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        //private readonly DataContext _context;
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)      // sin uso de repositorios (DataContext context)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
            //_context = context;
           
        }

        [HttpGet]
        //[AllowAnonymous]
        [Route("api/[controller]")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){
            var users = await _userRepository.GetMembersAsync(); /*await _userRepository.GetUsersAsync();
            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users); 
            */
            return Ok(users);  // return await _context.Users.ToListAsync();
        }

        // api/users/3
        //[Authorize]
        [HttpGet]
        [Route("api/[controller]/{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username){
            return await _userRepository.GetMemberAsync(username);       //GetUserByUsernameAsync(username);
            //return _mapper.Map<MemberDto>(user); //return await _context.Users.FindAsync(id);
        }

        [HttpPut]
        [Route("api/[controller]")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Fallo al actualizar el usuario");
        }

        [HttpPost]
        [Route("api/[controller]/add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if(result != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0){
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync()){
                return CreatedAtRoute("GetUser", new {username = user.UserName},_mapper.Map<PhotoDto>(photo));
            }
            
            return BadRequest("Problemas al cargar la foto");
        }
    }
}