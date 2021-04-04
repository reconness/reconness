namespace ReconNess.Core.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string UserName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string[] Roles();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool AreYouMember();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool AreYouAdmin();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool AreYouOwner();
    }
}
