using LocalWifiEnableAndConnect;
using System;
using System.IO;
using System.Text;

public class LoggerTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(string value)
    {
        Logger.Log(value);
    }

    public override void Write(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Logger.Log(value);
        }
    }
}