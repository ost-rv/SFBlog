using SFBlog.BLL.Models;
using SFBlog.BLL.Response;
using SFBlog.DAL.Models;
using SFBlog.DAL.Repository;
using SFBlog.DAL.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Services
{
    public class RoleService : IRoleService
    {
        private IUnitOfWork _UoW;
        private Repository<Role> _roleRepository;


        public RoleService(IUnitOfWork UoW)
        {
            _UoW = UoW;
            _roleRepository = (Repository<Role>)_UoW.GetRepository<Role>();
        }

        public async Task<EntityBaseResponse<RoleDomain>> Get(int id)
        {
            Role role = await _roleRepository.Get(id);

            if (role != null)
            {
                await _roleRepository.LoadNavigateProperty(role);
                return new EntityBaseResponse<RoleDomain>(Helper.Mapper.Map<RoleDomain>(role));
            }
            else
            {
                return new EntityBaseResponse<RoleDomain>($"Роль с Id = {id} не найдена.");
            }
        }

        public async Task<EntityBaseResponse<RoleDomain>> Add(RoleDomain roleDomain)
        {
            Role newRole = Helper.Mapper.Map<Role>(roleDomain);
            await _roleRepository.Create(newRole);

            return new EntityBaseResponse<RoleDomain>(Helper.Mapper.Map<RoleDomain>(newRole));
        }


        public async Task<EntityBaseResponse<RoleDomain>> Update(RoleDomain roleDomain)
        {
            Role role = await _roleRepository.Get(roleDomain.Id);

            if (role != null)
            {
                await _roleRepository.LoadNavigateProperty(role);
                
                role = Helper.Mapper.Map(roleDomain, role);
                await _roleRepository.Update(role);
                
                return new EntityBaseResponse<RoleDomain>(Helper.Mapper.Map<RoleDomain>(role));
            }
            else
            {
                return new EntityBaseResponse<RoleDomain>(false, $"Роль {roleDomain.Name} (Id = {roleDomain.Id}) не найдена.");
            }
        }


        public async Task<EntityBaseResponse<RoleDomain>> Delete(RoleDomain roleDomain)
        {
            Role role = await _roleRepository.Get(roleDomain.Id);

            if (role != null)
            {
                await _roleRepository.Delete(role);
                return new EntityBaseResponse<RoleDomain>(true, $"Роль удалена {role.Name} (Id = {role.Id})", roleDomain);
            }
            else
            {
                return new EntityBaseResponse<RoleDomain>(false, $"Роль {roleDomain.Name} (Id = {roleDomain.Id}) не найдена.");
            }
        }


        public EntityBaseResponse<IEnumerable<RoleDomain>> GetAll()
        {
            var roleList = _roleRepository.GetAll(t => t.UserRoles);
            return new EntityBaseResponse<IEnumerable<RoleDomain>>(Helper.Mapper.Map<IEnumerable<RoleDomain>>(roleList));
        }
    }
}
