using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrintController : ControllerBase
    {
        // To convert a .PEM PrivateKey:
        // openssl pkcs12 -export -inkey private-key.pem -in digital-certificate.txt -out private-key.pfx
        private static X509KeyStorageFlags STORAGE_FLAGS = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable;

        [HttpGet("SignMessage")]
        public IActionResult SignMessage(string request)
        {
            string base64Certificate = "LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tDQpNSUlFdlFJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLY3dnZ1NqQWdFQUFvSUJBUURYcFl0TDhWOE0wWk44DQpUZy9sVUg3dVJ1MzJhRlFPemNUVFlUQnExSE1EdllsTWtMK3EwaXVDbDdUNnFEUGloQ2VHSW1zMlBJTG5tejRoDQpSaWRBdVBaVlFwK1J0K0xnTTlCYmtYbkNHektTcTE3eGdXUFhucEpFanZ4VkN2Z0JPSUNaWDk3bzBLQlYxNzJkDQp6UElONTVXZ0lpemsxSldiQlU5azFEOTA5Tlh3VHBWa2IxK2xCTFpYOEFPU084OGRwaU5vQzdCVXI1K2t4SWw0DQo2Szd6U1cwQjBqNWhZdWhVYXc2NEFmaVZSWU9FajJYaW44TUdUdVlHTFNKK0FqMitBM3poYVZyNjJQdFV4VWR5DQowZkRBOU14RzJRT1RxMXZWUm9vcVFQVWdnNjhySFh6aDFHOWwxUEFHdkZGbzQvTjZmaTJPdmxJRmFraHVzODU5DQpvQ3dxa25OM0FnTUJBQUVDZ2dFQUJVM2hHMEVZb1d3V1FKeDY3NTBIUWowa1pVY3pxaG1ObWRWdUozS0tVYkU3DQp5MUpVSkpOKzFudDh6QjJQQ1NkUEFaR2ZLWHBiamU3WG5HSHBYVUl2YlBGeWsxYitrWjAwcHN2eFBzRUVnVzhODQpxc3NQOWFIRTMwTFZHaEptLzBDd2lSVSttbU5uVGpVOFNNRmR3dEhMY0dEbTBjb1kwL05SZkoxcjRpaVRpcEdZDQpxVFYwN0FhMzNHRFV5RkRzNWo2VmJ6dkc1Uy9VMjFJMkRSdWxKbm4xbWkzSGk0NExobTdPQWlmSnhYMFZnc1RhDQo5a3hkUEVhVVFCS0RNZEVQT1FLcUdPL2tkZEVES09ZdCt4enpoZEpMOXpQVEowK1Y1elNVQlR5N0FYR0VPRG11DQpMTlN3UVkwYmtqdk0zVG8vUnBlVlJnc0NIaU00ZzNpU0xxTUk3WTZtZ1FLQmdRRC8vK1hZMUV5MHRXVVU4WmYrDQpWUmhuaVdVWmZ0cGVuL3U0M3VWeXduZ0EvYmxlcVVMZHNWQUpLYlJyWlBFa3VuMzNlNngzc1RWaTFXQThyMEJTDQo4dGVQSjRZd01PL0pDaXQveloyaEhIdmdoVUNRcDl0WmFXMlNvVzFsRnEwRUpnYVdUd0dCTDQvVHVoYzJZdDJYDQo5OFdNSldmMnRHUjZ4ZU1wQ1U3N3d2OXFSd0tCZ1FEWHBhRlR3c3o4c2xCT1hhcyt6NStTU25MTUNzS0I4b0s0DQpraDdUck5TVjYydlE1WndQQWdWYWhtVVJIWFJLZkRHTnFjSFI1cGtjZnlHQUhoOGh1djJ5dG1Vckp3NkZOQ2VkDQpnN1RZYWIzZ3VCbXBzRXoxUmZsOHBVcnl5anRvQ3lQY0FzNVBycitJWk9JN1FlRE1jd1dEN3JFcmZlbHVsUE5TDQpYVUZnbHFZVlVRS0JnUUMxRGpoeWpWd2U1cmd5QXRpYjRYVmNMTmxiR2RPNTN1NTZodkxDb25XYlJkZW44cEEvDQpHUnZoRHlUeVc3Y05ZZlNiWjFYSVpZVzdlK3dQbjJjZ0EyNmZhYlhIT2FVdGk4ck9IZzloRmpNcXpUT24yNEFRDQprYmNVWUtWdFBJZHJpclZYODFLK0duUmZxNXFsTlhGQi9OUXoxWFRkRUNLTWpvNmx4RmFmU3YrNEd3S0JnRDdPDQo3djRHYnMzcTRaanhqejdzR2YzeDBSMjh4SW8wcC80NkVPQjJKalRFTE5MYkRXa1JuWjJRSnorazVJb05RaUdRDQpuL01TeHVXVjRadi9LZklaRkd0STFJNk5BWjJxWXg5dGpPNjdYYUpsL0pOLzFlcGFSeUFXeEkyU2VWc0VZNTFnDQo4OVloZjRlcVdub0g1Z0dtUHFUK2tIdXFZS0xrbnpvT0QweWlsQjBCQW9HQUdoTzJ4anltais1SGpIMStsaHdnDQpEOEppY3VET2NpY1Z6TkJqcUx0UTcvemtPOVVEWklnZlBXdTlXUWNuNUpGZU1qRzZrdVVVZ1JoTS9pUk9hekovDQpLWHd5UGMwZVdCMis1cERDMnBlcVhwYWh5V3ZsRGZGS09iVWYzRDhjYTZrb1ErSTJEa3dxSEtFWkJsYVMyMGRVDQpmVHhmSlZ0cms1eUk2ZTRxTzVjNHZzWT0NCi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0NCg==";
            var PASS = "364BB88E36904FC0AE2";

            try
            {
                byte[] data = new ASCIIEncoding().GetBytes(request);
                byte[] certificateBytes = Convert.FromBase64String(base64Certificate);
                var cert = new X509Certificate2(certificateBytes, PASS, STORAGE_FLAGS);

                RSA rsa = cert.GetRSAPrivateKey();
                var signed = rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                string base64 = Convert.ToBase64String(signed);
                return Content(base64);
            }
            catch (Exception ex)
            {
                if ((STORAGE_FLAGS & X509KeyStorageFlags.MachineKeySet) == X509KeyStorageFlags.MachineKeySet)
                {
                    // IISExpress may fail with "Invalid provider type specified"; remove MachineKeySet flag, try again
                    STORAGE_FLAGS = STORAGE_FLAGS & ~X509KeyStorageFlags.MachineKeySet;
                }
                return null;
            }
        }
    }
}
