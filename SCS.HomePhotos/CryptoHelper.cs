using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SCS.Cryptography
{
    public class CryptoHelper
    {
        public static X509Certificate2 GenerateCertificate(string subjectName, DateTime expirationUtc, string password, params string[] dnsNames)
        {
            SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);

            foreach (var dnsName in dnsNames)
            {
                sanBuilder.AddDnsName(dnsName);
            }
            X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={subjectName}");

            using (RSA rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));


                request.CertificateExtensions.Add(
                   new X509EnhancedKeyUsageExtension(
                       new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

                request.CertificateExtensions.Add(sanBuilder.Build());

                var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(expirationUtc));
                certificate.FriendlyName = subjectName;

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet);
            }
        }

        public static void AddCertificateToStore(X509Certificate2 certificate, StoreName storeName = StoreName.My, StoreLocation locationName = StoreLocation.LocalMachine)
        {
            var x509store = new X509Store(storeName, locationName);

            x509store.Open(OpenFlags.ReadWrite);

            byte[] pfx = certificate.Export(X509ContentType.Pfx);
            certificate = new X509Certificate2(pfx, (string)null, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

            x509store.Add(certificate);
            x509store.Close();
        }

        public static X509Certificate2 GetCertificateFromStore(string distinguishedName, StoreName storeName = StoreName.My, StoreLocation locationName = StoreLocation.LocalMachine)
        {
            distinguishedName = EnsureNameFormat(distinguishedName);

            var x509store = new X509Store(storeName, locationName);

            try
            {
                x509store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = x509store.Certificates;
                X509Certificate2Collection signingCert = certCollection.Find(X509FindType.FindBySubjectDistinguishedName, distinguishedName, false);

                if (signingCert.Count == 0)
                    return null;

                // Return the first certificate in the collection, has the right name and is current. 
                return signingCert[0];
            }
            finally
            {
                x509store.Close();
            }
        }

        public static bool CertificateExistsInStore(string distinguishedName, StoreName storeName = StoreName.My, StoreLocation locationName = StoreLocation.LocalMachine)
        {
            distinguishedName = EnsureNameFormat(distinguishedName);

            var x509store = new X509Store(storeName, locationName);

            try
            {
                x509store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = x509store.Certificates;
                X509Certificate2Collection signingCert = certCollection.Find(X509FindType.FindBySubjectDistinguishedName, distinguishedName, false);

                return (signingCert.Count > 0);
            }
            finally
            {
                x509store.Close();
            }
        }

        public static void SaveCertificate(X509Certificate2 certificate, string savePath, string password = null)
        {
            File.WriteAllText(savePath, Convert.ToBase64String(certificate.Export(X509ContentType.Cert, password)));
        }

        public static X509Certificate2 LoadCertificate(string certificatePath, string password = null)
        {
            var certificate = new X509Certificate2();
            certificate.Import(certificatePath, password, X509KeyStorageFlags.MachineKeySet);

            return certificate;
        }

        public static void RemoveCertificateFromStore(X509Certificate2 certificate, StoreName storeName = StoreName.My, StoreLocation locationName = StoreLocation.LocalMachine)
        {
            var store = new X509Store(storeName, locationName);

            store.Open(OpenFlags.ReadWrite);
            store.Remove(certificate);
            store.Close();
        }

        private static string EnsureNameFormat(string distinguishedName)
        {
            if (!distinguishedName.StartsWith("CN=", StringComparison.InvariantCultureIgnoreCase))
            {
                distinguishedName = "CN=" + distinguishedName;
            }
            return distinguishedName;
        }
    }
}
