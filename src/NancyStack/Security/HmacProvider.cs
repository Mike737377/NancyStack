using Nancy.Cryptography;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;

namespace NancyStack.Security
{
    public class HmacProvider : IHmacProvider
    {
        /// <summary>
        /// HMAC length
        /// </summary>
        private readonly int hmacLength = new HMACSHA256().HashSize / 8;

        /// <summary>
        /// Preferred key size for HMACSHA256
        /// </summary>
        private const int PreferredKeySize = 64;

        /// <summary>
        /// Key
        /// </summary>
        private static readonly byte[] key;

        static HmacProvider()
        {
            MachineKeySection section = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
            key = ASCIIEncoding.ASCII.GetBytes(section.ValidationKey);
        }

        /// <summary>
        /// Gets the length of the HMAC signature
        /// </summary>
        public int HmacLength
        {
            get { return this.hmacLength; }
        }

        /// <summary>
        /// Create a hmac from the given data using the given passPhrase
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>String representation of the hmac</returns>
        public byte[] GenerateHmac(string data)
        {
            return this.GenerateHmac(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Create a hmac from the given data
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>Hmac bytes</returns>
        public byte[] GenerateHmac(byte[] data)
        {
            var hmacGenerator = new HMACSHA256(key);

            return hmacGenerator.ComputeHash(data);
        }
    }
}
