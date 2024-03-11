using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneWayFolderSync.Extensions.ServiceExtensions;
using OneWayFolderSync.Models;
using OneWayFolderSync.Services;
using Xunit;

namespace TestOneWayFolderSync;

public class TestSyncService
{
    private readonly IHost host;
    private readonly string source;
    private readonly string destination;

    public TestSyncService(){

        host = ConfigureServices.CreateApplicationHost();

        // Plese use your own local directory path

        source = "/Users/funakoshisilva/Desktop/Test/TestLab";
        destination = "/Users/funakoshisilva/Desktop/Test/TestLab1";

    }
    
    /// <summary>
    /// Compares two folder paths for equality based on byte size
    /// Comparison by byte size is not the strongest folder comparisom on a real world context
    /// </summary>
    [Fact]
    public void FoldersAreEqual()
    {

        Directory.CreateDirectory($"{source}");
        Directory.CreateDirectory($"{destination}");

        var sourceBytesize = Helper.GetByteSize(source);
        var destinationBytesize = Helper.GetByteSize(destination);

        Assert.Equal(sourceBytesize, destinationBytesize);

    }

    /// <summary>
    /// Compares two folder paths for difference based on byte size
    /// Comparison by byte size is not the strongest folder comparisom on a real world context
    /// </summary>
    [Fact]
    public void FoldersAreDifferent()
    {

        Directory.CreateDirectory($"{source}");
        var fw = File.Create($"{source}/newfile.txt");

        var title = new UTF8Encoding(true).GetBytes("New Text File");
        fw.Write(title, 0, title.Length);
        fw.Close();


        Directory.CreateDirectory($"{destination}");

        var sourceBytesize = Helper.GetByteSize(source);
        var destinationBytesize = Helper.GetByteSize(destination);

        Assert.NotEqual(sourceBytesize, destinationBytesize);

    }

    /// <summary>
    /// Copies files to target directory 
    /// </summary>
    [Fact]
    public void CopyFilesToTargetDirectory()
    {
        Random randomNumber = new Random();
        string sourcePath = $"{source}/{randomNumber.Next()}";
        string destinationPath = $"{destination}/{randomNumber.Next()}";

        Directory.CreateDirectory(sourcePath);
        Directory.CreateDirectory(destinationPath);

        File.Create($"{sourcePath}/newfile.txt").Close();

        var request = new Request()
        {
            SourcePath = sourcePath,
            DestinationPath = destinationPath
        };

        var syncService = host.Services.GetRequiredService<ISyncService>();
        syncService.CopyFiles(request);

        var srcFileInfo = new DirectoryInfo(request.SourcePath);
        var destinationFileInfo = new DirectoryInfo(request.DestinationPath);

        var sourceFileCount = srcFileInfo.GetFiles().Count();
        var destinationFileCount = destinationFileInfo.GetFiles().Count();

        Directory.Delete(sourcePath, true);
        Directory.Delete(destinationPath, true);

        Assert.Equal(sourceFileCount, destinationFileCount);
    }


    /// <summary>
    /// Copies directories to target directory 
    /// </summary>
    [Fact]
    public void CopyFoldersToTargetDirector()
    {
        string toCreate = $"{source}/source/MoreTest";
        string sourcePath = $"{source}";
        string destinationPath = $"{destination}/destination";

        Directory.CreateDirectory(toCreate);
        Directory.CreateDirectory(destinationPath);

        File.Create($"{sourcePath}/newfile.txt").Close();

        var request = new Request()
        {
            SourcePath = sourcePath,
            DestinationPath = destinationPath
        };

        var syncService = host.Services.GetRequiredService<ISyncService>();
        syncService.CopyAllDirectories(request);

        var srcInfo = new DirectoryInfo(request.SourcePath);
        var destinationInfo = new DirectoryInfo(request.DestinationPath);

        var sourceFolderCount = Directory.GetDirectories(srcInfo.FullName, "*", SearchOption.AllDirectories).Count();
        var destinationFolderCount = Directory.GetDirectories(destinationInfo.FullName, "*", SearchOption.AllDirectories).Count();

        Directory.Delete(sourcePath, true);
        Directory.Delete(destinationPath, true);

        Assert.Equal(sourceFolderCount, destinationFolderCount);
    }

    /// <summary>
    /// Deletes files in target folder that do not exist in source folder 
    /// </summary>
    [Fact]
    public void DeleteFiles()
    {

        string toCreate = $"{source}/source/MoreTest";
        string sourcePath = $"{source}";
        string destinationPath = $"{destination}/destination";

        Directory.CreateDirectory(toCreate);
        Directory.CreateDirectory(destinationPath);

        File.Create($"{destinationPath}/newfile.txt").Close();
        File.Create($"{sourcePath}/source.txt").Close();

        var request = new Request()
        {
            SourcePath = sourcePath,
            DestinationPath = destinationPath
        };

        var syncService = host.Services.GetRequiredService<ISyncService>();

        var srcInfo = new DirectoryInfo(request.SourcePath);
        var destinationInfo = new DirectoryInfo(request.DestinationPath);

        syncService.DeleteFiles(srcInfo, destinationInfo);

        var sourceFolderCount = srcInfo.GetFiles().Count();
        var destinationFolderCount = destinationInfo.GetFiles().Count();

        Directory.Delete(sourcePath, true);
        Directory.Delete(destinationPath, true);

        Assert.True(sourceFolderCount > destinationFolderCount);
    }

    /// <summary>
    /// Deletes files in target folder that do not exist in source folder 
    /// </summary>
    [Fact]
    public void DeleteFilesSubdirectories()
    {
        string toCreatedestination = $"{destination}/destination/MoreTest";
        string toCreatesourcePath = $"{source}/source/MoreTest";
        string sourcePath = $"{source}/source";
        string destinationPath = $"{destination}/destination";

        Directory.CreateDirectory(toCreatedestination);
        Directory.CreateDirectory(toCreatesourcePath);

        File.Create($"{toCreatedestination}/newfile.txt").Close();
        File.Create($"{toCreatesourcePath}/source.txt").Close();

        var request = new Request()
        {
            SourcePath = sourcePath,
            DestinationPath = destinationPath
        };

        var syncService = host.Services.GetRequiredService<ISyncService>();

        var srcInfo = new DirectoryInfo(request.SourcePath);
        var destinationInfo = new DirectoryInfo(request.DestinationPath);

        syncService.DeleteFilesSubdirectory(srcInfo, destinationInfo);

        var sourceFolderCount = Directory.GetFiles(srcInfo.FullName, "*", SearchOption.AllDirectories).Count();
        var destinationFolderCount = Directory.GetFiles(destinationInfo.FullName, "*", SearchOption.AllDirectories).Count();

        Directory.Delete(sourcePath, true);
        Directory.Delete(destinationPath, true);

        Assert.True(sourceFolderCount > destinationFolderCount);
    }

}
