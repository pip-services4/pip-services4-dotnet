using PipServices4.Container.Containers;

namespace PipServices4.Container
{
	public class DummyProcess : ProcessContainer
	{
		public DummyProcess()
			: base("dummy", "Sample dummy process")
		{
			this._configPath = "./dummy.yml";
			this._factories.Add(new DummyFactory());
		}
	}
}
