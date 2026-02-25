
// https://www.youtube.com/watch?v=OJ0kMm8NXc8&t=618s

class ScopedVsAsyncLocal
{
    public static void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DI
        builder.Services.AddOpenApi();
        builder.Services.AddTransient<MyMessageHandler>();

        builder.Services.AddScoped<GuidValueScoped>();
        builder.Services.AddSingleton<GuidValueAsyncLocal>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient("http")
            .AddHttpMessageHandler<MyMessageHandler>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapGet("/scoped-vs-asynclocal",
            async Task<object> (GuidValueScoped guidValueScoped1, GuidValueAsyncLocal guidValueAsyncLocal1, IServiceScopeFactory scopeFactory) =>
            {
                guidValueScoped1.GuidValue = Guid.NewGuid();
                guidValueAsyncLocal1.GuidValue = Guid.NewGuid();

                var scope = scopeFactory.CreateScope();
                var guidValueScoped2 = scope.ServiceProvider.GetRequiredService<GuidValueScoped>();
                var guidValueAsyncLocal2 = scope.ServiceProvider.GetRequiredService<GuidValueAsyncLocal>();

                await Task.Delay(1000);

                return new
                {
                    guidValueScoped1 = guidValueScoped1.GuidValue,
                    guidValueScoped2 = guidValueScoped2.GuidValue,
                    guidValueAsyncLocal1 = guidValueAsyncLocal1.GuidValue,
                    guidValueAsyncLocal2 = guidValueAsyncLocal2.GuidValue
                };
            });

        app.MapGet("/ping", (string? value, IHttpContextAccessor httpContext) =>
         {

             return new { value, headers = httpContext.HttpContext!.Request.Headers };
         });

        app.MapGet("/pingcall", async Task<object> (IHttpClientFactory httpClientFactory) =>
        {
            HttpClient client = httpClientFactory.CreateClient("http");
            var result = await client.GetFromJsonAsync<object>("http://localhost:5253/ping");
            return result!;
        });

        app.MapGet("/", () =>
        {
            return "hello world";
        });

        app.Run();
    }

    public class GuidValueScoped
    {
        public Guid GuidValue { get; set; }
    }

    public class GuidValueAsyncLocal
    {
        public static AsyncLocal<Guid> _guidValueAsyncLocal { get; set; } = new AsyncLocal<Guid>();

        public Guid GuidValue
        {
            get => GuidValueAsyncLocal._guidValueAsyncLocal.Value;
            set => GuidValueAsyncLocal._guidValueAsyncLocal.Value = value;
        }
    }

    class MyMessageHandler : DelegatingHandler
    {
        private readonly GuidValueScoped guidValueScoped;

        public MyMessageHandler(GuidValueScoped guidValueScoped)
        {
            this.guidValueScoped = guidValueScoped;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("x-my-guid", guidValueScoped.GuidValue.ToString());
            return base.SendAsync(request, cancellationToken);
        }
    }
}

