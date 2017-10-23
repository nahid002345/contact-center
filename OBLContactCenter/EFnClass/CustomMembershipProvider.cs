using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using OBLContactCenter.EFnClass;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Specialized;

namespace OBLContactCenter.EFnClass
{
    public sealed class CustomMembershipProvider : MembershipProvider
    {
        #region variables
        
        CCUSER oCCUSER = null;
        public OBLCONTACTCENTEREntities oDBContext = new OBLCONTACTCENTEREntities();

        private int newPasswordLength = 8;  
        private string connectionString;  
        private string applicationName;  
        private bool enablePasswordReset;  
        private bool enablePasswordRetrieval;  
        private bool requiresQuestionAndAnswer;  
        private bool requiresUniqueEmail;  
        private int maxInvalidPasswordAttempts;  
        private int passwordAttemptWindow;  
        private MembershipPasswordFormat passwordFormat;  
        private int minRequiredNonAlphanumericCharacters;  
        private int minRequiredPasswordLength;  
        private string passwordStrengthRegularExpression;  
        private MachineKeySection machineKey; //Used when determining encryptio
        
        #endregion
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                string configPath = "~/web.config";
                Configuration newConfig = WebConfigurationManager.OpenWebConfiguration(configPath);
                MembershipSection mbershipSection = (MembershipSection)newConfig.GetSection("system.web/membership");
                ProviderSettingsCollection setting = mbershipSection.Providers;
                NameValueCollection mbershipParams = setting[mbershipSection.DefaultProvider].Parameters;
                config = mbershipParams;
            }
            if (string.IsNullOrEmpty(name))
                name = "CustomMembershipProvider";

            base.Initialize(name, config);
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            oCCUSER = oDBContext.CCUSERs.FirstOrDefault(t => t.USERID == username && t.USERPASSWORD == password);
            if (oCCUSER != null)
            {
                FormsAuthentication.SetAuthCookie(username, false);
                Roles.AddUserToRole(oCCUSER.USERID, oCCUSER.CCROLE.ROLENAME);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}