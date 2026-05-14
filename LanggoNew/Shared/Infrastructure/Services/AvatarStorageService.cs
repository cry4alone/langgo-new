using Amazon.S3;
using Amazon.S3.Model;
using LanggoNew.Shared.Infrastructure;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface IAvatarStorageService
{
    public Task EnsureBucketExistsAsync();
    public Task<string> UploadAsync(IFormFile file, string userId);
    public Task<string> GetPresignedUrlAsync(string key, TimeSpan expiration);
    public Task DeleteAsync(string key);
}

public class AvatarStorageService(IAmazonS3ClientFactory factory, IConfiguration config) : IAvatarStorageService
{
    private readonly IAmazonS3 _internalS3 = factory.CreateInternalClient();
    private readonly IAmazonS3 _publicS3 = factory.CreatePublicClient();
    private readonly string _bucket = config["MinIO:BucketName"]!;

    public async Task EnsureBucketExistsAsync()
    {
        await _internalS3.EnsureBucketExistsAsync(_bucket);
    }

    public async Task<string> UploadAsync(IFormFile file, string userId)
    {
        var safeFileName = Path.GetFileName(file.FileName);
        var key = $"avatars/{userId}/{Guid.NewGuid()}_{safeFileName}";
        
        await using var stream = file.OpenReadStream();
        await _internalS3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        });

        return key;
    }
    
    public async Task<string> GetPresignedUrlAsync(string key, TimeSpan expiration)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = key,
            Expires = DateTime.UtcNow.Add(expiration),
            Protocol = Protocol.HTTP
        };

        return await _publicS3.GetPreSignedURLAsync(request);
    }

    public async Task DeleteAsync(string key)
    {
        await _internalS3.DeleteObjectAsync(_bucket, key);
    }
}
