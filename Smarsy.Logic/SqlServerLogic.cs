﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SmarsyEntities;

namespace Smarsy.Logic
{
    public class SqlServerLogic : IDatabaseLogic
    {
        private readonly string _stringConn =
            "Data Source = localhost;Initial Catalog=Smarsy; Integrated Security = True; Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout = 60; Encrypt=False;TrustServerCertificate=True";
        //"Data Source=(localdb)\\ProjectsV13;Initial Catalog=Smarsy;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True";
        
        public void UpsertLessons(List<string> lessons)
        {
            foreach (var lesson in lessons)
            {
                InsertLessonIfNotExists(lesson);
            }
            ;
        }

        private void InsertLessonIfNotExists(string lesson)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_InsertLessonIfNotExists", objconnection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@lessonName", SqlDbType.NVarChar, 100);
                    objcmd.Parameters["@lessonName"].Value = lesson;

                    objcmd.ExecuteNonQuery();
                }
            }

        }

        public List<MarksRowElement> UpserStudentAllLessonsMarks(int childId, List<MarksRowElement> marks)
        {
            var result = new List<MarksRowElement>();
            int studentId = GetStudentIdByChildId(childId);
            foreach (var mark in marks)
            {
                var lessonId = GetLessonIdByName(mark.LessonName);
                var lesson = new MarksRowElement()
                {
                    LessonName = mark.LessonName,
                    LessonId = lessonId
                };
                lesson.Marks = UpsertStudentMarks(studentId, lessonId, mark.Marks);
                if (lesson.Marks.Any()) result.Add(lesson);
            }
            return result;
        }

        private int GetStudentIdByChildId(int childId)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();

                using (var objcmd = new SqlCommand("select dbo.fn_GetStudentIdByChildId(@studentId)", objconnection))
                {

                    objcmd.CommandType = CommandType.Text;
                    objcmd.Parameters.AddWithValue("@studentId", childId);
                    var res = objcmd.ExecuteScalar();
                    return int.Parse(res.ToString());
                }

            }
        }

        public int GetLessonIdByName(string markLessonName)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();

                using (var objcmd = new SqlCommand("select dbo.fn_GetLessonIdByLessonName(@lessonName)", objconnection))
                {

                    objcmd.CommandType = CommandType.Text;
                    objcmd.Parameters.AddWithValue("@lessonName", markLessonName);
                    var res = objcmd.ExecuteScalar();
                    return int.Parse(res.ToString());
                }

            }
        }

        private List<StudentMark> UpsertStudentMarks(int studentId, int lessonId, List<StudentMark> marks)
        {
            var result = new List<StudentMark>();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_UpsertStudentMarksByLesson", objconnection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@studentId", SqlDbType.Int);
                    objcmd.Parameters["@studentId"].Value = studentId;
                    objcmd.Parameters.Add("@lessonId", SqlDbType.Int);
                    objcmd.Parameters["@lessonId"].Value = lessonId;

                    var marksWithDates = new DataTable("marksWithDates");
                    marksWithDates.Columns.Add("Mark", typeof(int));
                    marksWithDates.Columns.Add("MarkDate", typeof(DateTime));
                    marksWithDates.Columns.Add("Reason", typeof(string));

                    foreach (var mark in marks)
                    {
                        marksWithDates.Rows.Add(mark.Mark, mark.Date, mark.Reason);
                    }

                    SqlParameter tvpParam = objcmd.Parameters.AddWithValue("@marksWithDates",
                        marksWithDates);

                    tvpParam.SqlDbType = SqlDbType.Structured;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        var mark = new StudentMark()
                        {
                            Mark = int.Parse(res["Mark"].ToString()),
                            Date = Convert.ToDateTime(res["MarkDate"].ToString()),
                            Reason = res["Reason"].ToString()
                        };
                        result.Add(mark);
                    }
                }
                return result;
            }

        }
        
        public Student GetStudentBySmarsyChildId(int childId)
        {
            var result = new Student();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetStudentBySmarsyId", objconnection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@smarsyChildId", SqlDbType.Int);
                    objcmd.Parameters["@smarsyChildId"].Value = childId;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        result.StudentId = int.Parse(res["Id"].ToString());
                        result.Name = res["Name"].ToString();
                        result.Login = res["Login"].ToString();
                        result.Password = res["Password"].ToString();
                        result.SmarsyChildId = int.Parse(res["SmarsyChildId"].ToString());
                    }
                }
                return result;
            }

        }
    }
}