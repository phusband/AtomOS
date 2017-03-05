/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Compiler Logger
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Collections;
using System.IO;
using System.Diagnostics;
using Atomix.CompilerExt;

namespace Atomix
{
    public class Logger
    {
        private string LoggerPath;
        private Stopwatch Timer;
        private bool IsLogging;

        ArrayList Script;
        ArrayList Message;
        ArrayList Details;

        public Logger(string aPath, bool aDoLog)
        {
            IsLogging = aDoLog;

            if (!aDoLog)
                return;
            
            if (string.IsNullOrEmpty(aPath)
                throw new System.ArgumentNullException();  // Or default to a specific path?

            Script = new ArrayList();
            Message = new ArrayList();
            Details = new ArrayList();
            LoggerPath = Path.Combine(Path.GetDirectoryName(aPath), Path.GetFileName(aPath) + Helper.LoggerFile);
            Timer = Stopwatch.StartNew();
        }

        public void Write(string aScript, string aMessage, string aDetail)
        {
            if (!IsLogging)
                return;

            Script.Add(aScript);
            Message.Add(aMessage);
            Details.Add(aDetail);
        }

        public void Write(string aAppend, bool aSub = true)
        {
            if (!IsLogging)
                return;
            
            Details[Details.Count - 1] = string.Format(aSub ? "{0}<li>{1}</li>" : "{0}<br>{1}", Details[Details.Count - 1], aAppend);
        }

        public void Dump()
        {
            if (!IsLogging)
                return;

            Timer.Stop();
            log2html page = new log2html(Script, Message, Details, Timer.ElapsedMilliseconds.ToString());
            File.WriteAllText(LoggerPath, page.TransformText());
        }
    }
}
