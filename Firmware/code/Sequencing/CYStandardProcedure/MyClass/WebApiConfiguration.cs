using System;
using System.IO;
using System.Xml;

namespace CYStandardProcedure.MyClass
{
    public class WebApiConfiguration
    {
        public int Port { get; set; } = 8081;
        public bool EnableLogging { get; set; } = true;
        public bool EnableCors { get; set; } = true;
        public int RequestTimeout { get; set; } = 30000;
        public string BaseUrl { get; set; } = "http://localhost";

        private readonly string _configPath;

        public WebApiConfiguration()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemCfg.xml");
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    CreateDefaultConfiguration();
                    return;
                }

                var doc = new XmlDocument();
                doc.Load(_configPath);

                var webApiNode = doc.SelectSingleNode("//WebApiConfig");
                if (webApiNode != null)
                {
                    var portNode = webApiNode.SelectSingleNode("Port");
                    int port;
                    if (portNode != null && int.TryParse(portNode.InnerText, out port))
                    {
                        Port = port;
                    }

                    var enableLoggingNode = webApiNode.SelectSingleNode("EnableLogging");
                    bool enableLogging;
                    if (enableLoggingNode != null && bool.TryParse(enableLoggingNode.InnerText, out enableLogging))
                    {
                        EnableLogging = enableLogging;
                    }

                    var enableCorsNode = webApiNode.SelectSingleNode("EnableCors");
                    bool enableCors;
                    if (enableCorsNode != null && bool.TryParse(enableCorsNode.InnerText, out enableCors))
                    {
                        EnableCors = enableCors;
                    }

                    var requestTimeoutNode = webApiNode.SelectSingleNode("RequestTimeout");
                    int timeout;
                    if (requestTimeoutNode != null && int.TryParse(requestTimeoutNode.InnerText, out timeout))
                    {
                        RequestTimeout = timeout;
                    }

                    var baseUrlNode = webApiNode.SelectSingleNode("BaseUrl");
                    if (baseUrlNode != null && !string.IsNullOrEmpty(baseUrlNode.InnerText))
                    {
                        BaseUrl = baseUrlNode.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载Web API配置时发生错误: {ex.Message}");
            }
        }

        private void CreateDefaultConfiguration()
        {
            try
            {
                var doc = new XmlDocument();
                
                if (File.Exists(_configPath))
                {
                    doc.Load(_configPath);
                }
                else
                {
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                    var rootElement = doc.CreateElement("SystemConfiguration");
                    doc.AppendChild(rootElement);
                }

                var root = doc.DocumentElement;
                if (root == null)
                {
                    root = doc.CreateElement("SystemConfiguration");
                    doc.AppendChild(root);
                }

                var existingWebApiNode = root.SelectSingleNode("WebApiConfig");
                if (existingWebApiNode == null)
                {
                    var webApiNode = doc.CreateElement("WebApiConfig");
                    
                    var portNode = doc.CreateElement("Port");
                    portNode.InnerText = Port.ToString();
                    webApiNode.AppendChild(portNode);

                    var enableLoggingNode = doc.CreateElement("EnableLogging");
                    enableLoggingNode.InnerText = EnableLogging.ToString();
                    webApiNode.AppendChild(enableLoggingNode);

                    var enableCorsNode = doc.CreateElement("EnableCors");
                    enableCorsNode.InnerText = EnableCors.ToString();
                    webApiNode.AppendChild(enableCorsNode);

                    var requestTimeoutNode = doc.CreateElement("RequestTimeout");
                    requestTimeoutNode.InnerText = RequestTimeout.ToString();
                    webApiNode.AppendChild(requestTimeoutNode);

                    var baseUrlNode = doc.CreateElement("BaseUrl");
                    baseUrlNode.InnerText = BaseUrl;
                    webApiNode.AppendChild(baseUrlNode);

                    var descriptionNode = doc.CreateElement("Description");
                    descriptionNode.InnerText = "Web API配置 - 用于远程硬件控制";
                    webApiNode.AppendChild(descriptionNode);

                    root.AppendChild(webApiNode);
                    doc.Save(_configPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"创建Web API默认配置时发生错误: {ex.Message}");
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                var doc = new XmlDocument();
                
                if (File.Exists(_configPath))
                {
                    doc.Load(_configPath);
                }
                else
                {
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                    var rootElement = doc.CreateElement("SystemConfiguration");
                    doc.AppendChild(rootElement);
                }

                var root = doc.DocumentElement;
                if (root == null)
                {
                    root = doc.CreateElement("SystemConfiguration");
                    doc.AppendChild(root);
                }

                var webApiNode = root.SelectSingleNode("WebApiConfig");
                if (webApiNode == null)
                {
                    webApiNode = doc.CreateElement("WebApiConfig");
                    root.AppendChild(webApiNode);
                }

                UpdateOrCreateNode(doc, webApiNode, "Port", Port.ToString());
                UpdateOrCreateNode(doc, webApiNode, "EnableLogging", EnableLogging.ToString());
                UpdateOrCreateNode(doc, webApiNode, "EnableCors", EnableCors.ToString());
                UpdateOrCreateNode(doc, webApiNode, "RequestTimeout", RequestTimeout.ToString());
                UpdateOrCreateNode(doc, webApiNode, "BaseUrl", BaseUrl);

                doc.Save(_configPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存Web API配置时发生错误: {ex.Message}");
            }
        }

        private void UpdateOrCreateNode(XmlDocument doc, XmlNode parent, string nodeName, string value)
        {
            var node = parent.SelectSingleNode(nodeName);
            if (node == null)
            {
                node = doc.CreateElement(nodeName);
                parent.AppendChild(node);
            }
            node.InnerText = value;
        }

        public string GetApiBaseUrl()
        {
            return $"{BaseUrl}:{Port}/api/hardware";
        }

        public override string ToString()
        {
            return $"WebAPI配置: Port={Port}, EnableLogging={EnableLogging}, EnableCors={EnableCors}, RequestTimeout={RequestTimeout}ms, BaseUrl={BaseUrl}";
        }
    }
}