using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace OBLContactCenter.EFnClass
{
    public class CustomRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            
        }

        


        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (OBLCONTACTCENTEREntities oEntity = new OBLCONTACTCENTEREntities())
            {

                string[] ret = oEntity.CCROLEs.Select(x => x.ROLENAME).ToArray();
                return ret; 
            }
        }

        public override string[] GetRolesForUser(string UserId)
        {
            using (OBLCONTACTCENTEREntities oEntity = new OBLCONTACTCENTEREntities())
            {

                string[] ret = oEntity.CCROLEs.Where(t => t.CCUSERs.FirstOrDefault(x => x.USERID == UserId) != null).Select(p => p.ROLENAME).ToArray();
                return ret;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (OBLCONTACTCENTEREntities db = new OBLCONTACTCENTEREntities())
            {
                CCUSER user = db.CCUSERs.FirstOrDefault(u => u.USERID == username && u.CCROLE.ROLENAME == roleName);
                if (user != null)
                    return true;
                else
                    return false;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}