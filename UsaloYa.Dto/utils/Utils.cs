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
