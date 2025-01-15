namespace SA.Agnostic {
    using System.Runtime.Serialization;
    using System.Xml;
    using Encoding = System.Text.Encoding;
    using MemoryStream = System.IO.MemoryStream;

    public static class Persistence<Model> {

        public static void Store(Model graph, string filename) {
            DataContractSerializer serializer = new(typeof(Model));
            using XmlWriter writer = XmlWriter.Create(filename, HumanReadableXmlWriterSettings);
            serializer.WriteObject(writer, graph);
        } //Store   

        public static string Store(Model graph) {
            DataContractSerializer serializer = new(typeof(Model));
            using MemoryStream memoryStream = new();
            serializer.WriteObject(memoryStream, graph);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        } //Store

        public static Model Load(string filename) {
            DataContractSerializer serializer = new(typeof(Model));
            using XmlReader reader = XmlReader.Create(filename, FastestXmlReaderSettings);
            return (Model)serializer.ReadObject(reader);
        } //Load    

        public static Model LoadFromString(string content) {
            DataContractSerializer serializer = new(typeof(Model));
            using MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(content));
            return (Model)serializer.ReadObject(memoryStream);
        } //LoadFromString

        static XmlWriterSettings HumanReadableXmlWriterSettings {
            get {
                XmlWriterSettings settings = new();
                settings.NewLineHandling = NewLineHandling.Entitize;
                settings.Indent = true;
                settings.IndentChars = DefinitionSet.xmlIndentChars;
                return settings;
            } //get HumanReadableXmlWriterSettings
        } //HumanReadableXmlWriterSettings

        static XmlReaderSettings FastestXmlReaderSettings {
            get {
                XmlReaderSettings settings = new();
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;
                return settings;
            } //get FastestXmlReaderSettings
        } //FastestXmlReaderSettings

    } //Persistence

}
