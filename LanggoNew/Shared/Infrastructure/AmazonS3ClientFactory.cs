using Amazon.S3;

namespace LanggoNew.Shared.Infrastructure;

public interface IAmazonS3ClientFactory
{
    IAmazonS3 CreateInternalClient();
    IAmazonS3 CreatePublicClient();
}

public class AmazonS3ClientFactory : IAmazonS3ClientFactory
{
    private readonly string _internalEndpoint;
    private readonly string _publicEndpoint;
    private readonly string _accessKey;
    private readonly string _secretKey;

    public AmazonS3ClientFactory(IConfiguration config)
    {
        var minioConfig = config.GetSection("MinIO");
        _internalEndpoint = minioConfig["Endpoint"]!;
        _publicEndpoint = minioConfig["PublicEndpoint"]!;
        _accessKey = minioConfig["AccessKey"]!;
        _secretKey = minioConfig["SecretKey"]!;
    }

    public IAmazonS3 CreateInternalClient() => CreateClient(_internalEndpoint);
    public IAmazonS3 CreatePublicClient() => CreateClient(_publicEndpoint);

    private IAmazonS3 CreateClient(string endpoint)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true,
            UseHttp = true
        };

        return new AmazonS3Client(_accessKey, _secretKey, config);
    }
}