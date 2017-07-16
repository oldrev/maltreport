#addin "Cake.Npm"

var target = Argument("target", "Default");
var config = Argument("config", "Release");

const string outputDir = "./build";
const string appOutputDir = "./build/app";
const string installerOutputDir = "./build/installer";
const string clientOutputDir = "./src/HWBridge.Web/wwwroot";
const string clientAppDir = "./src/HWBridge.Web";
const string slnPath = "./HWBridge.sln";
const string publishProj = "./src/HWBridge.Web/HWBridge.Web.csproj";
const string srcPath = "./src";
const string sevenZipPath = "./lib/7z/7z.exe";
const string libreofficeUrl = "	http://spr-1253419854.coscd.myqcloud.com/lib/libreoffice_portable.exe";
const string libreofficeExePath = "./tools/libreoffice_portable.exe";
const string libreofficeDir = "./build/libreoffice";

Task("Default")
    .IsDependentOn("Build")
    .Does(() => {
        Information("开始执行默认任务....");
});


Task("MakeBuildDir").Does(() => {

    if (!DirectoryExists(outputDir)) {
        CreateDirectory(outputDir);
    }
    if (!DirectoryExists(appOutputDir)) {
        CreateDirectory(appOutputDir);
    }
    if (!DirectoryExists(installerOutputDir)) {
        CreateDirectory(installerOutputDir);
    }

});

Task("DownloadLibreOffice")
    .Does(() => {
        if (!FileExists(libreofficeExePath)) {
            Information("文件不存在，开始下载 LibreOffice，文件较大，请稍等几分钟...");
            DownloadFile(libreofficeUrl, libreofficeExePath);
            Information("LibreOffice 下载完毕");
        }
});

Task("ExtractLibreOffice")
    .IsDependentOn("DownloadLibreOffice")
    .Does(() => {
        Information("开始解压 LibreOffice");
        var args = string.Format("x -y -bb1 -o{0} {1}", libreofficeDir, libreofficeExePath);
        using(var process = StartAndReturnProcess(sevenZipPath, new ProcessSettings{ Arguments = args }))
        {
            process.WaitForExit();
            // This should output 0 as valid arguments supplied
            Information("Exit code: {0}", process.GetExitCode());
        }

});

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() => {
        Information("开始执行全部重新构建任务....");
});

//全部构建任务
Task("Build")
    .IsDependentOn("BuildServer")
    .IsDependentOn("BuildClient")
    .Does(() => {
        Information("开始执行全部构建任务....");
});

//清理服务器端构建输出任务
Task("CleanServer")
    .IsDependentOn("MakeBuildDir")
    .Does(() => {

    CleanDirectories(srcPath + "/**/bin/" + config);
    CleanDirectories(srcPath + "/**/obj/" + config);

});

//清理服务器端构建输出任务
Task("CleanClient")
    .IsDependentOn("MakeBuildDir")
    .Does(() => {

        //清理 wwwroot 目录
        if (DirectoryExists(clientOutputDir)) {
            CleanDirectory(clientOutputDir);
        }
});



//清理构建输出任务
Task("Clean")
    .IsDependentOn("MakeBuildDir")
    .IsDependentOn("CleanServer")
    .IsDependentOn("CleanClient")
    .Does(() => {

        //清理 build 目录
        if (DirectoryExists(outputDir)) {
            CleanDirectory(outputDir);
        }
});


//恢复 nuget 任务
Task("Restore").Does(() => {
        DotNetCoreRestore(slnPath);
});


//编译服务器端构建任务
Task("BuildServer")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(slnPath);
});


//构建客户端任务
Task("BuildClient")
    .IsDependentOn("NpmInstall")
    .Does(() => {
        Information("开始执行构建客户端....");
        var settings = new NpmRunScriptSettings {
            LogLevel = NpmLogLevel.Warn,
            WorkingDirectory = clientAppDir,
            ScriptName = "ng",
        };
        settings.WithArguments("build --prod --aot=false");
        NpmRunScript(settings);
});


//运行客户端任务
Task("RunClient")
    .IsDependentOn("NpmInstall")
    .Does(() => {
        Information("开始执行构建客户端....");
        var settings = new NpmRunScriptSettings {
            LogLevel = NpmLogLevel.Warn,
            WorkingDirectory = clientAppDir,
            ScriptName = "ng",
        };
        settings.WithArguments("build --watch --env=dev");
        NpmRunScript(settings);
});


//NPM 安装包
Task("NpmInstall").Does(() => {
        var settings = new NpmInstallSettings();

        settings.LogLevel = NpmLogLevel.Warn;
        settings.WorkingDirectory = clientAppDir;
        settings.Production = false;

        NpmInstall(settings);
});


//发布任务
Task("Publish")
    .IsDependentOn("Rebuild")
    .Does(() => {
        var settings = new DotNetCorePublishSettings
        {
            Framework = "netcoreapp1.1",
            Configuration = config,
            OutputDirectory = appOutputDir,
            Runtime = "win7-x86",
        };
        DotNetCorePublish(publishProj, settings);

        Information("复制客户端文件到发布目录...");
        var appWwwrootDir = appOutputDir + "/wwwroot";
        if(!DirectoryExists(appWwwrootDir)) {
            CreateDirectory(appWwwrootDir);
        }
        var files = GetFiles(clientOutputDir + "/**/*");
        CopyFiles(files, appWwwrootDir, true);

});


//制作安装包任务
Task("Installer")
    .IsDependentOn("Publish")
    .IsDependentOn("ExtractLibreOffice")
    .Does(() => {
        InnoSetup("./setup/setup.iss");

});


RunTarget(target);

