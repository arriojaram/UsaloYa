namespace UsaloYa.API.Enums
{
    public enum Role
    { 
        Unknown = 0,
        User = 1,
        Admin = 2,
        Root = 3
    }

    public static class EConverter
    {
        public static int GetEnumValueFromString<T>(string value) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (Enum.TryParse<T>(value, true, out T result))
            {
                return (int)(object)result; // Casting de forma segura a int
            }
            // Retornar 0 si no hay coincidencia
            return 0;
        }

        public static string GetEnumNameFromValue<T>(int value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return Enum.GetName(typeof(T), value); // Obtener el nombre asociado al valor
            }
            // Retornar una cadena vacía si no hay coincidencia
            return string.Empty;
        }

        public static T GetEnumFromValue<T>(int value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            return default;
        }

        public static T GetEnumFromValue<T>(string value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            return default;
        }
    }
}
