using System;

namespace ReconNess.Web.Dtos
{
    public class UserDto
    {
        public Guid? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConfirmationPassword { get; set; }

        public string Image { get; set; }

        public bool Owner { get; set; }

    }
}