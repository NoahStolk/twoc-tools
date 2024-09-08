using StrongInject;
using TwocTools.App;
using TwocTools.Core.Serializers;


using Container container = new();
using Owned<Application> app = container.Resolve();
app.Value.Run();
