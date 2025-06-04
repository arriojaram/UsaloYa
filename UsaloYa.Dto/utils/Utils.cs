using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace UsaloYa.Dto.Utils
{
    public class Utils
    {
        private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random _random = new();

        public static string GenerateCode()
        {
            int length = 8;
            return new string(Enumerable.Repeat(Alphanumeric, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string EncryptPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool IsSha256Hash(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 64)
                return false;

            // Validar que todos los caracteres sean hexadecimales (0-9, a-f, A-F)
            return input.All(c =>
                (c >= '0' && c <= '9') ||
                (c >= 'a' && c <= 'f') ||
                (c >= 'A' && c <= 'F'));
        }

        public static DateTime GetMxDateTime()
        {
            // Obtener la zona horaria de México
            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            // Obtener la fecha y hora actual en UTC
            DateTime utcNow = DateTime.UtcNow;

            // Convertir la fecha y hora actual en UTC a la hora local de México
            DateTime mexicoTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, mexicoTimeZone);

            return mexicoTime;
        }

        /// <summary>
        /// Serializa una lista de PairSettingsDto a una cadena XML.
        /// </summary>
        /// <param name="settings">Lista de configuraciones.</param>
        /// <returns>Cadena XML representando la lista.</returns>
        public static string XmlSerializeSettings(List<PairSettingsDto> settings)
        {
            // Se define un atributo raíz para darle un nombre a la etiqueta contenedora
            var xmlRoot = new XmlRootAttribute("PairSettings");
            var serializer = new XmlSerializer(typeof(List<PairSettingsDto>), xmlRoot);

            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, settings);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Deserializa una cadena XML a una lista de PairSettingsDto.
        /// </summary>
        /// <param name="xml">Cadena XML a deserializar.</param>
        /// <returns>Lista de PairSettingsDto obtenida del XML.</returns>
        public static List<PairSettingsDto> DeserializeSettings(string xml)
        {
            var xmlRoot = new XmlRootAttribute("PairSettings");
            var serializer = new XmlSerializer(typeof(List<PairSettingsDto>), xmlRoot);

            using (var stringReader = new StringReader(xml))
            {
                return (List<PairSettingsDto>)serializer.Deserialize(stringReader);
            }
        }

        public static string? EmptyToNull(string? value)
        {
            return string.IsNullOrEmpty(value) ? null : value.Trim();
        }

    }
}
