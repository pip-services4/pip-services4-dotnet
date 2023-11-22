namespace PipServices4.Http.Controllers
{
    /// <summary>
    /// Interface to perform on-demand registrations.
    /// </summary>
    public interface IRegisterable
    {
        /// <summary>
        /// Perform required registration steps.
        /// </summary>
        void Register();
    }
}
