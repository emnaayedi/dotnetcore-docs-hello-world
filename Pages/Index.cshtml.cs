using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

namespace dotnetcoresample.Pages;

public class IndexModel : PageModel
{

    public string OSVersion { get { return RuntimeInformation.OSDescription; }  }
    
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {        
    }
    [HttpPost]
public IActionResult DoWorkOne(TheModel model) { 
     string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            string blobContainerName = "soaktestreports".ToString();
            // Find the container and return a container client object


            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var latestBlob = containerClient.GetBlobs()
       .OrderByDescending(m => m.Properties.LastModified)
    .ToList()
    .First();
            Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
            {
                BlobContainerName = "soaktestreports",
                BlobName = latestBlob.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),//Let SAS token expire after 5 minutes.
            };
            blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);//User will only be able to read the blob and it's properties
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("pdfreportsaquila", "QC+mUrcvLW8Mxajn4BgAbXijRIsE9Sg/dG/lM/yKlWEA22vwAH4w6cY2pBcqyugIzSkEbb8WrQlL+AStjuxszA==")).ToString();
          
            Response.RedirectPermanent("https://pdfreportsaquila.blob.core.windows.net/soaktestreports/" +latestBlob.Name+"?"+ sasToken);}

}


