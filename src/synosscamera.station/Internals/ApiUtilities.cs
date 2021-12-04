using synosscamera.station.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.station.Internals
{
    /// <summary>
    /// Internal utilities
    /// </summary>
    internal static class ApiUtilities
    {
        private const int _segmentsPerDay = 48;
        private const int _days = 7;

        /// <summary>
        /// Create schedule string based on schedule and from and to times
        /// </summary>
        /// <remarks>Currently activates the schedule mode for the whole week!!</remarks>
        /// <param name="schedule"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int[,] CreateRecordingSchedule(RecordSchedule schedule, DateTime? from = null, DateTime? to = null, int[] currentSchedule = null)
        {
            /*
             A string consists of 48*7 digits to represent the
            scheduling. Note that each
            digit stands for the schedule type of half-hour:
            0: No scheduled plan
            1: Continuous Recording
            2: Motion Detection Recording
            3: Custom Detection 1
            4: Custom Detection 2
            */

            var weekSchedule = new int[_days, _segmentsPerDay];

            if (currentSchedule != null)
                throw new NotImplementedException("Schedule mergers currently not supported!");

            for (int i = 0; i < _days; i++)
                for (int j = 0; j < _segmentsPerDay; j++)
                    weekSchedule[i,j] = (int)schedule;

            return weekSchedule;
        }

        /// <summary>
        /// Converts a recording schedule to a string
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static string RecordingScheduleToString(int[,] schedule)
        {
            if (schedule == null)
                schedule = CreateSchedule(RecordSchedule.Off);

            var ret = new StringBuilder(_segmentsPerDay * _days);

            for (int i = 0; i < _days; i++)
                for (int j = 0; j < _segmentsPerDay; j++)
                {
                    ret.Append(schedule[i, j].ToString());
                }

            return ret.ToString();
        }

        /// <summary>
        /// Converts a recording schedule to a string
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string RecordingScheduleToSeparatedString(int[,] schedule, string separator = ",")
        {
            if (schedule == null)
                schedule = CreateSchedule(RecordSchedule.Off);


            var ret = new StringBuilder(_segmentsPerDay * _days + (_segmentsPerDay * 7 - _days) * separator.Length);

            for (int i = 0; i < _days; i++)
                for (int j = 0; j < _segmentsPerDay; j++)
                {
                    ret.Append(schedule[i, j].ToString());
                    if(!(i == (_days-1) && j == (_segmentsPerDay-1)))
                        ret.Append(separator);
                }

            return ret.ToString();
        }

        /// <summary>
        /// Converts a recording schedule to a flat array
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static int[] ToFlatArray(int[,] schedule)
        {
            if (schedule == null)
                schedule = CreateSchedule(RecordSchedule.Off);

            var ret = new int[_segmentsPerDay * _days];
            var idx = 0;

            for (int i = 0; i < _days; i++)
                for (int j = 0; j < _segmentsPerDay; j++)
                {
                    ret[idx++] = schedule[i, j];
                }

            return ret;
        }

        private static int[,] CreateSchedule(RecordSchedule schedule)
        {
            var weekSchedule = new int[_days, _segmentsPerDay];

            for (int i = 0; i < _days; i++)
                for (int j = 0; j < _segmentsPerDay; j++)
                    weekSchedule[i, j] = (int)schedule;

            return weekSchedule;
        }
    }
}
