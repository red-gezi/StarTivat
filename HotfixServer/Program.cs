//using Blazored.LocalStorage;
//using Server;

////�����û����ݿ�
//Log.Init();
//Log.Summary("��־ϵͳ��ʼ��");
//HttpServer.Init();
//Log.Summary("��Դ�������ѳ�ʼ��");
//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddRazorPages();
//builder.Services.AddAntDesign();
//builder.Services.AddServerSideBlazor();
//builder.Services.AddBlazoredLocalStorage();
////��api
//builder.Services.AddControllers();
//builder.Services.AddRazorPages();
//builder.Services.AddSignalR(hubOptions =>
//{
//    hubOptions.EnableDetailedErrors = true;
//    hubOptions.MaximumReceiveMessageSize = null;
//    hubOptions.ClientTimeoutInterval = new TimeSpan(0, 5, 0);
//}).AddNewtonsoftJsonProtocol();
//var app = builder.Build();
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}
////app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.MapBlazorHub();
////��api
//app.MapControllers();
//app.MapFallbackToPage("/_Host");
//app.MapHub<HotFixHouHub>("/TouHouHub");
//app.Urls.Add("http://*:495");
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' https://www.baidu.com/");
//    await next.Invoke();
//});
////app.Urls.Add("https://*:49514");
//Console.WriteLine("�������Ӧ����");
//Console.WriteLine("�����������");
//app.Run();


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.MaximumReceiveMessageSize = null;
});
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapHub<HotFixHub>("/HotFixHub");
app.Urls.Add("http://*:233");
HotFixHub.Init();
app.Run();