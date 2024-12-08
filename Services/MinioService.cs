using Minio;
using Minio.DataModel.Args;

public class MinioService
{
    private readonly IMinioClient  _minioClient;
    private readonly string _bucketName;

    public MinioService(IConfiguration configuration)
    {
        _bucketName = configuration["MinIO:BucketName"]!;
        _minioClient = new MinioClient()
            .WithEndpoint(configuration["MinIO:Endpoint"])
            .WithCredentials(
                configuration["MinIO:AccessKey"],
                configuration["MinIO:SecretKey"])
            .Build();

        if (_bucketName == null)
        {
            throw new ArgumentNullException("MinIO:BucketName must be provided");
        }
    }

    public async Task EnsureBucketExistsAsync()
    {
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!exists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
    {
        await EnsureBucketExistsAsync();

        var objectName = $"{Guid.NewGuid()}/{fileName}";
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length));

        return objectName;
    }

    public string GetFileUrl(string objectName)
    {
        return $"{_minioClient.Config.Endpoint}/{_bucketName}/{objectName}";
    }
}
