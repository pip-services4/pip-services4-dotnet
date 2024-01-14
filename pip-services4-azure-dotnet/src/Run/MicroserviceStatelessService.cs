﻿using Microsoft.ServiceFabric.Services.Runtime;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using System.Fabric;

namespace PipServices4.Azure.Run
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TS"></typeparam>
    /// <typeparam name="TC"></typeparam>
    public abstract class MicroserviceStatelessService<TC> : StatelessService, IReferenceable, IReconfigurable
        where TC : class
    {
        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        protected TC Controller { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected CompositeLogger Logger { get; } = new CompositeLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceContext"></param>
        protected MicroserviceStatelessService(StatelessServiceContext serviceContext) 
            : base(serviceContext)
        {
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <returns>Descriptor.</returns>
        public abstract Descriptor GetDescriptor();

        /// <summary>
        /// Sets the references.
        /// </summary>
        /// <param name="references">The references.</param>
        public virtual void SetReferences(IReferences references)
        {
            Logger.SetReferences(references);

            var locater = new Descriptor("*", "service", "*", "*", "*");

            Controller = references.GetOneRequired<TC>(locater);

            if (Controller == null)
                throw new ReferenceException(null, locater);
        }

        /// <summary>
        /// Configures the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public abstract void Configure(ConfigParams config);
    }
}
