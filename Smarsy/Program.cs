﻿namespace Smarsy
{
    using System;
    using CommandLine;

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var options = new CommandLineOptions();
            if (Parser.Default.ParseArguments(args, options))
            {
                var op = new Operational("90018970");
                op.InitStudentFromDb();
                //options.Methods = "LoginToSmarsy,UpdateMarks,UpdateHomeWork,UpdateAds,UpdateStudents,UpdateRemarks,SendEmail";
                options.Methods = "LoginToSmarsy,UpdateMarks,UpdateHomeWork,UpdateAds,UpdateStudents,UpdateRemarks,SendEmail";

                var methods = options.Methods.Split(',');
                foreach (var method in methods)
                {
                    var classType = op.GetType();
                    var methodInfo = classType.GetMethod(method);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(op, null);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }
}