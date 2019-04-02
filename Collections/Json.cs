using System;
using System.Globalization;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    public class Json : IHashable, IEquatable<Json>
    {

        private HashList<Json> hashList = null;
        private SingleLinkedList<Json> linkedList = null;
        private string key = null;
        private string value = null;
        private bool? bValue = null;
        private double? nValue = null;

        private static int jsonPosition, jsonLength;

        public static uint hashSize = 8;
        public static uint hashElementCount = 4096;

        public double? NValue { get => nValue; set { linkedList = null; hashList = null; this.value = null; bValue = null; nValue = value; } }
        public bool? BValue { get => bValue; set { linkedList = null; hashList = null; this.value = null; nValue = null; bValue = value; } }
        public string Value { get => value; set { linkedList = null; hashList = null; nValue = null; bValue = null; this.value = null; } }

        public bool IsObject()
        {
            return hashList != null;
        }

        public bool IsArray()
        {
            return linkedList != null;
        }

        public bool IsBoolean()
        {
            return BValue != null;
        }

        public bool IsNumber()
        {
            return NValue != null;
        }

        public bool IsString()
        {
            return hashList == null && linkedList == null && BValue == null && NValue == null;
        }

        public Json this[string key]
        {
            get
            {
                if (!IsObject()) return null;
                Json result;
                if (hashList.TryGetValue(new Json()
                {
                    key = key
                }, out result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                if (!IsObject()) return;
                if (value == null)
                {
                    if (hashList == null) return;
                    hashList.Remove(value);
                    if (hashList.Count == 0) hashList = null;
                }
                else
                {
                    if (hashList == null) hashList = new HashList<Json>(hashSize, hashElementCount) { EnableHashSizeGrow = true };
                    hashList.Add(value);
                }
            }
        }

        public Json this[int key]
        {
            get
            {
                if (!IsArray()) return null;
                if (linkedList == null || linkedList.Count <= key) return null;
                return linkedList[key];
            }
            set
            {
                if (!IsArray()) return;
                if (value == null)
                {
                    if (linkedList == null) return;
                    linkedList.Remove(value);
                    if (linkedList.Count == 0) linkedList = null;
                }
                else
                {
                    if (linkedList == null) linkedList = new SingleLinkedList<Json>();
                    if (linkedList.Count <= key) throw new IndexOutOfRangeException();
                    linkedList[key] = value;
                }
            }
        }

        public void Push(Json value)
        {
            if (!IsArray()) return;
            if (linkedList == null) linkedList = new SingleLinkedList<Json>();
            linkedList.Add(value);
        }


        public static Json DecodeJsonFromString(string json)
        {
            Json jsonObject = new Json();
            jsonLength = json.Length;
            jsonPosition = 0;
            EatWhiteSpaces(ref json);
            while (jsonPosition < jsonLength)
            {
                switch (json[jsonPosition++])
                {
                    case '{':
                        jsonObject.hashList = new HashList<Json>(hashSize, hashElementCount) { EnableHashSizeGrow = true };
                        jsonObject.hashList.Add(NewJsonObject(ref json));
                        break;
                    case '"':
                        break;
                    case '[':
                        break;
                    default:
                        jsonPosition--;
                        throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: {{ or \" or [");
                }
            }

            return jsonObject;
        }

        static Json NewJsonArray(ref string json)
        {
            Json result = new Json();
            if (result == null) throw new OutOfMemoryException();
            result.linkedList = new SingleLinkedList<Json>();
            if (result.linkedList == null) throw new OutOfMemoryException();

            EatWhiteSpaces(ref json);
            if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

            while (jsonPosition < jsonLength)
            {
                EatWhiteSpaces(ref json);
                switch (json[jsonPosition])
                {
                    case '"':
                        jsonPosition++;
                        Json jsonString = NewJsonString(ref json);
                        result.linkedList.Add(jsonString);
                        break;
                    case '{':
                        jsonPosition++;
                        Json jsonObject = NewJsonObject(ref json);
                        result.linkedList.Add(jsonObject);
                        break;
                    case '[':
                        jsonPosition++;
                        Json jsonArray = NewJsonArray(ref json);
                        result.linkedList.Add(jsonArray);
                        break;
                    case 'n':
                        Json jsonNull = NewJsonNull(ref json);
                        result.linkedList.Add(jsonNull);
                        break;
                    case 't':
                    case 'f':
                        Json jsonBoolean = NewJsonBoolean(ref json);
                        result.linkedList.Add(jsonBoolean);
                        break;
                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        Json jsonNumber = NewJsonNumber(ref json);
                        result.linkedList.Add(jsonNumber);
                        break;
                    default:
                        throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: \"");
                }

                EatWhiteSpaces(ref json);
                if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

                if (json[jsonPosition] == ']')
                {
                    jsonPosition++;
                    break;
                }
                else if (json[jsonPosition] == ',')
                {
                    jsonPosition++;
                    continue;
                }
                else
                {
                    throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: }} or ,");
                }
            }

            return result;
        }

        static Json NewJsonObject(ref string json)
        {
            Json result = new Json();
            if (result == null) throw new OutOfMemoryException();
            result.hashList = new HashList<Json>(hashSize, hashElementCount) { EnableHashSizeGrow = true };
            if (result.hashList == null) throw new OutOfMemoryException();

            EatWhiteSpaces(ref json);
            if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

            while (jsonPosition < jsonLength)
            {
                EatWhiteSpaces(ref json);
                switch (json[jsonPosition])
                {
                    case '"':
                        string key = GetString(ref json);
                        if (key == null || key == "") throw new FormatException($"Key cannot be null or empty string. At: {jsonPosition}");

                        EatWhiteSpaces(ref json);
                        if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

                        switch (json[jsonPosition])
                        {
                            case ':':
                                EatWhiteSpaces(ref json);
                                if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

                                switch (json[jsonPosition])
                                {
                                    case '{':
                                        jsonPosition++;
                                        Json jsonObject = NewJsonObject(ref json);
                                        jsonObject.key = key;
                                        result.hashList.Add(jsonObject);
                                        break;
                                    case '"':
                                        jsonPosition++;
                                        Json jsonString = NewJsonString(ref json);
                                        jsonString.key = key;
                                        result.hashList.Add(jsonString);
                                        break;
                                    case '[':
                                        jsonPosition++;
                                        Json jsonArray = NewJsonArray(ref json);
                                        jsonArray.key = key;
                                        result.hashList.Add(jsonArray);
                                        break;
                                    case 'n':
                                        Json jsonNull = NewJsonNull(ref json);
                                        jsonNull.key = key;
                                        result.hashList.Add(jsonNull);
                                        break;
                                    case 't':
                                    case 'f':
                                        Json jsonBoolean = NewJsonBoolean(ref json);
                                        jsonBoolean.key = key;
                                        result.hashList.Add(jsonBoolean);
                                        break;
                                    case '-':
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7':
                                    case '8':
                                    case '9':
                                        Json jsonNumber = NewJsonNumber(ref json);
                                        jsonNumber.key = key;
                                        result.hashList.Add(jsonNumber);
                                        break;
                                    default:
                                        jsonPosition--;
                                        throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: {{ or \" or [");
                                }
                                break;
                            default:
                                throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: :");
                        }
                        break;
                    default:
                        throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: \"");
                }

                EatWhiteSpaces(ref json);
                if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

                if (json[jsonPosition] == '}')
                {
                    jsonPosition++;
                    break;
                }
                else if (json[jsonPosition] == ',')
                {
                    jsonPosition++;
                    continue;
                }
                else
                {
                    throw new FormatException($"Invalid character at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: }} or ,");
                }
            }

            return result;
        }

        static Json NewJsonString(ref string json)
        {
            Json result = new Json();
            if (result == null) throw new OutOfMemoryException();

            result.Value = GetString(ref json);
            return result;
        }

        static string GetString(ref string json)
        {
            StringBuilder newKey = new StringBuilder();
            while (json[jsonPosition] != '"')
            {
                switch (json[jsonPosition])
                {
                    case '\\':
                        jsonPosition++;
                        newKey.Append(GetSpecialChar(ref json));
                        break;
                    default:
                        newKey.Append(json[jsonPosition++]);
                        break;
                }
            }
            jsonPosition++;
            return newKey.ToString();
        }

        static char GetSpecialChar(ref string json)
        {
            if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");
            switch (json[jsonPosition++])
            {
                case '\\':
                    return '\\';
                case '"':
                    return '"';
                case 't':
                    return '\t';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case '/':
                    return '/';
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'u':
                    string digits = "";
                    if (jsonPosition + 4 >= jsonLength) throw new IndexOutOfRangeException("Unfinished unicode escape sequence");
                    for (int i = 0; i < 4; i++) digits += json[jsonPosition++];
                    return (char)int.Parse(digits, NumberStyles.HexNumber);
                default:
                    jsonPosition--;
                    throw new FormatException($"Invalid escape sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting one of: \\\"tnr/bfu");
            }
        }

        static void EatWhiteSpaces(ref string json)
        {
            while (jsonPosition < jsonLength)
            {
                switch (json[jsonPosition])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        jsonPosition++;
                        continue;
                    default:
                        return;
                }
            }
        }

        static Json NewJsonBoolean(ref string json)
        {
            Json result = new Json();
            if (result == null) throw new OutOfMemoryException();

            result.bValue = GetBoolean(ref json);
            return result;
        }

        static bool GetBoolean(ref string json)
        {
            StringBuilder newBoolean = new StringBuilder(6);
            switch(json[jsonPosition])
            {
                case 't':
                    if (jsonPosition + 3 >= jsonLength) throw new IndexOutOfRangeException("Unfinished json string");
                    for (int i = 0; i < 4; i++) newBoolean.Append(json[jsonPosition++]);
                    if (newBoolean.ToString() == "true")
                    {
                        return true;
                    } else
                    {
                        jsonPosition -= 4;
                        throw new FormatException($"Invalid boolean sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: true or false");
                    }
                case 'f':
                    if (jsonPosition + 4 >= jsonLength) throw new IndexOutOfRangeException("Unfinished json string");
                    for (int i = 0; i < 5; i++) newBoolean.Append(json[jsonPosition++]);
                    if (newBoolean.ToString() == "false")
                    {
                        jsonPosition -= 5;
                        return false;
                    }
                    else
                    {
                        throw new FormatException($"Invalid boolean sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: true or false");
                    }
                default:
                    throw new Exception("Something went totally wrong...");
            }
            throw new Exception("Something went totally wrong...");
        }

        static Json NewJsonNumber(ref string json)
        {
            Json result = new Json();
            if (result == null) throw new OutOfMemoryException();

            result.nValue = GetNumber(ref json);
            return result;
        }

        static double GetNumber(ref string json)
        {
            StringBuilder newDouble = new StringBuilder(30);
            if (json[jsonPosition] == '-')
            {
                newDouble.Append('-');
                jsonPosition++;
            }
            if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");

            if (json[jsonPosition] == '.') throw new FormatException($"Invalid number sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting one of: 0-9");
            
            if (json[jsonPosition] == '0')
            {
                newDouble.Append('0');
                jsonPosition++;
                if (IsNumberEnd(ref json))
                {
                    return double.Parse(newDouble.ToString(), NumberStyles.Float);
                }
                if (json[jsonPosition] == '0') throw new FormatException($"Invalid number sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting one of: .eE,]}}");
            }

            newDouble.Append(GetDigitSequence(ref json));
            if (IsNumberEnd(ref json))
            {
                return double.Parse(newDouble.ToString(), NumberStyles.Float);
            }

            if (json[jsonPosition] == '.')
            {
                newDouble.Append('.');
                jsonPosition++;
                if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");
                newDouble.Append(GetDigitSequence(ref json));
                if (IsNumberEnd(ref json))
                {
                    return double.Parse(newDouble.ToString(), NumberStyles.Float);
                }
            }

            if (json[jsonPosition] == 'e' || json[jsonPosition] == 'E')
            {
                newDouble.Append(json[jsonPosition]);
                jsonPosition++;
                if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");
                if (json[jsonPosition] == '+' || json[jsonPosition] == '-')
                {
                    newDouble.Append(json[jsonPosition]);
                    jsonPosition++;
                    if (jsonPosition >= jsonLength) throw new FormatException("Unfinished json string");
                }
                newDouble.Append(GetDigitSequence(ref json));
                if (IsNumberEnd(ref json))
                {
                    return double.Parse(newDouble.ToString(), NumberStyles.Float);
                }
            }

            if (!IsNumberEnd(ref json))
            {
                throw new FormatException($"Invalid number sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting one of: ,]}}");
            }
            
            return double.Parse(newDouble.ToString(), NumberStyles.Float);
        }



        static string GetDigitSequence(ref string json)
        {
            StringBuilder digitSequence = new StringBuilder(30);
            while (jsonPosition < jsonLength && (
                json[jsonPosition] == '0' ||
                json[jsonPosition] == '1' ||
                json[jsonPosition] == '2' ||
                json[jsonPosition] == '3' ||
                json[jsonPosition] == '4' ||
                json[jsonPosition] == '5' ||
                json[jsonPosition] == '6' ||
                json[jsonPosition] == '7' ||
                json[jsonPosition] == '8' ||
                json[jsonPosition] == '9'))
            {
                digitSequence.Append(json[jsonPosition]);
                jsonPosition++;
            }
            return digitSequence.ToString();
        }

        static bool IsNumberEnd(ref string json)
        {
            EatWhiteSpaces(ref json);
            if (jsonPosition >= jsonLength ||
                json[jsonPosition] == ',' ||
                json[jsonPosition] == ']' ||
                json[jsonPosition] == '}')
            {
                return true;
            }
            return false;
        }

        static Json NewJsonNull(ref string json)
        {
            Json result = new Json();
            if (result == null) throw new OutOfMemoryException();

            result.value = GetNull(ref json);
            return result;
        }

        static string GetNull(ref string json)
        {
            StringBuilder newBoolean = new StringBuilder(6);
            switch (json[jsonPosition])
            {
                case 'n':
                    if (jsonPosition + 3 >= jsonLength) throw new IndexOutOfRangeException("Unfinished json string");
                    for (int i = 0; i < 4; i++) newBoolean.Append(json[jsonPosition++]);
                    if (newBoolean.ToString() == "null")
                    {
                        return null;
                    }
                    else
                    {
                        jsonPosition -= 4;
                        throw new FormatException($"Invalid boolean sequence at: {jsonPosition}, value: {json[jsonPosition]}. Expecting: null");
                    }
                default:
                    throw new Exception("Something went totally wrong...");
            }
            throw new Exception("Something went totally wrong...");
        }

        public uint GetHash()
        {
            int length = key.Length;
            if (key.Length <= 0) return default(uint);
            uint hash = key[0];
            int i = 0;
            while (i < length) hash *= 101 + (uint)key[i++];
            return hash;
        }

        public bool Equals(Json other)
        {
            return Value.Equals(other.Value);
        }
    }
}
