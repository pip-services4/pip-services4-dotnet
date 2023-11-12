using PipServices4.Components.Context;
using System.Collections;
using System.Threading.Tasks;

namespace PipServices4.Components.Exec
{
    /// <summary>
    /// Helper class that notifies components.
    /// </summary>
    /// See <see cref="INotifiable"/>
    public class Notifier
    {
        /// <summary>
        /// Notifies specific component.
        /// 
        /// To be notiied components must implement INotifiable interface. If they don't
        /// the call to this method has no effect.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="component">the component that is to be notified.</param>
        /// <param name="args">notifiation arguments.</param>
        /// See <see cref="INotifiable"/>
        public static async Task NotifyOneAsync(IContext context, object component, Parameters args)
        {
                var notifiable = component as INotifiable;
                if (notifiable != null)
                    await notifiable.NotifyAsync(context, args);
        }

        /// <summary>
        /// Notifies multiple components.
        /// 
        /// To be notified components must implement INotifiable interface. If they don't
        /// the call to this method has no effect.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="component">the component that is to be notified.</param>
        /// <param name="args">notifiation arguments.</param>
        public static async Task NotifyAsync(IContext context, IEnumerable components, Parameters args)
        {
            if (components == null) return;

            foreach (var component in components)
                await NotifyOneAsync(context, component, args);
        }
    }
}