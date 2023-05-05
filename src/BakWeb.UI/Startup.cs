using Konstrukt.Extensions;
using BakWeb.Reservation.Entities;
using BakWeb.Reservation.Repositories;
using SendGrid;
using BakWeb.Options;
using BakWeb.Services;

namespace BakWeb;

public class Startup
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup" /> class.
    /// </summary>
    /// <param name="webHostEnvironment">The web hosting environment.</param>
    /// <param name="config">The configuration.</param>
    /// <remarks>
    /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337.
    /// </remarks>
    public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
    {
        _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <remarks>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
    /// </remarks>
    public void ConfigureServices(IServiceCollection services)
    {
        var umbracoBuilder = services.AddUmbraco(_env, _config)
               .AddBackOffice()
               .AddWebsite()
               .AddKonstrukt(cfg =>
               {
                   cfg.AddSectionAfter("media", "Repositories", sectionConfig => sectionConfig
                       .Tree(treeConfig => treeConfig
                           .AddCollection<ReservationEntity>(x => x.Id, "Reservation", "Reservations", "A reservation entity",
                           "icon-umb-users", "icon-umb-users", collectionConfig => collectionConfig
                               .SetNameProperty(p => p.Name)
                               .ListView(listViewConfig => listViewConfig
                                   .AddField(p => p.ProductId).SetHeading("Product Id")
                                   .AddField(p => p.MemberId).SetHeading("Member Id")
                                   .AddField(p => p.UniqueNumber).SetHeading("Unique Number")
                               )
                               .Editor(editorConfig => editorConfig
                                   .AddTab("General", tabConfig => tabConfig
                                       .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                           .AddField(p => p.ProductId).MakeRequired()
                                           .SetValidationRegex(@"^[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?$")
                                           .AddField(p => p.MemberId).MakeRequired()
                                           .SetValidationRegex(@"^[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?$")
                                           .AddField(p => p.UniqueNumber).MakeRequired()
                                           .AddField(p => p.ReservationDate).SetDefaultValue(DateTime.Now)
                                           .AddField(p => p.ReservationEndDate).SetDefaultValue(DateTime.Now.AddDays(1))
                                           .AddField(p => p.IsReservationClosed)
                                       )
                                   )
                               )
                           )
                       )
                   );
               })
               .AddComposers();

        // SendGrid
        var sendGridSection = _config.GetSection("SendGrid");
        services.Configure<SendGridOptions>(sendGridSection);
        var sendGridApiKey = sendGridSection.GetValue<string>("ApiKey");
        services.AddScoped<ISendGridClient, SendGridClient>(x => new SendGridClient(sendGridApiKey));
        services.AddScoped<SendGridService>();

        umbracoBuilder.Build();
    }

    /// <summary>
    /// Configures the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The web hosting environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseUmbraco()
            .WithMiddleware(u =>
            {
                u.UseBackOffice();
                u.UseWebsite();
            })
            .WithEndpoints(u =>
            {
                u.UseInstallerEndpoints();
                u.UseBackOfficeEndpoints();
                u.UseWebsiteEndpoints();
            });
    }
}
