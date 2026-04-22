/*
© ASTRA - of the Space Hardware Club at UAH, 2026
Author: Roald Schaum, roaldschaum2019@gmail.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
namespace RosSharp.RosBridgeClient
{
#if ROS2
    public class QOS
    {
        public Policy.History HistoryPolicy;

        public uint Depth;

        public Policy.Reliability ReliabilityPolicy;
        public Policy.Durability DurabilityPolicy;

        public Duration Deadline;
        public Duration Lifespan;

        public QOS(QOS q)
        {
            HistoryPolicy = q.HistoryPolicy;
            Depth = q.Depth;
            ReliabilityPolicy = q.ReliabilityPolicy;
            DurabilityPolicy = q.DurabilityPolicy;
            Deadline = q.Deadline;
            Lifespan = q.Lifespan;
        }

        public QOS(
            Policy.History history,
            uint depth,
            Policy.Reliability reliability,
            Policy.Durability durability,
            double deadline,
            double lifespan)
        {
            HistoryPolicy = history;
            Depth = depth;
            ReliabilityPolicy = reliability;
            DurabilityPolicy = durability;
            Deadline = Duration.DoubleToTime(deadline);
            Lifespan = Duration.DoubleToTime(lifespan);
        }

        public QOS(
            Policy.History history,
            uint depth,
            Policy.Reliability reliability,
            Policy.Durability durability,
            Duration deadline,
            Duration lifespan)
        {
            HistoryPolicy = history;
            Depth = depth;
            ReliabilityPolicy = reliability;
            DurabilityPolicy = durability;
            Deadline = deadline;
            Lifespan = lifespan;
        }

        /// <summary> Default QOS profiles as specified by the RMW qos_profiles.h file </summary>
        public class Presets
        {
            /// <summary> Default QOS profile </summary>
            public static readonly QOS Default = new QOS(
                Policy.History.Keep_last,
                10,
                Policy.Reliability.Reliable,
                Policy.Durability.Volatile,
                Policy.Duration_Unspecified,
                Policy.Duration_Unspecified);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS SensorData = new QOS(
                Policy.History.Keep_last,
                5,
                Policy.Reliability.Best_Effort,
                Policy.Durability.Volatile,
                Policy.Duration_Unspecified,
                Policy.Duration_Unspecified);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS Services = new QOS(
                Policy.History.Keep_last,
                10,
                Policy.Reliability.Reliable,
                Policy.Durability.Volatile,
                Policy.Duration_Unspecified,
                Policy.Duration_Unspecified);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS ParameterEvents = new QOS(
                Policy.History.Keep_last,
                1000,
                Policy.Reliability.Reliable,
                Policy.Durability.Volatile,
                Policy.Duration_Unspecified,
                Policy.Duration_Unspecified);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS Rosout = new QOS(
                Policy.History.Keep_last,
                1000,
                Policy.Reliability.Reliable,
                Policy.Durability.Transient_Local,
                Policy.Duration_Unspecified,
                new Duration(10, 0));

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS SystemDefault = new QOS(
                Policy.History.System_Default,
                0,
                Policy.Reliability.System_Default,
                Policy.Durability.System_Default,
                Policy.Duration_Unspecified,
                Policy.Duration_Unspecified);
        }

        public static class Policy
        {
            public static Duration Duration_Unspecified = new Duration(0ul, 0ul);
            public static Duration Duration_Infinite = new Duration(9223372036ul, 854775807ul);

            public enum History
            {
                /// <summary> Implementation default for history policy </summary>
                System_Default = 0,

                /// <summary> Only store up to a maximum number of samples, dropping oldest once max is exceeded </summary>
                Keep_last = 1,

                /// <summary> Store all samples, subject to resource limits </summary>
                Keep_All = 2,

                /// <summary> History policy has not yet been set </summary>
                Unknown = 3
            }
            public enum Reliability
            {
                /// <summary> Implementation specific default </summary>
                System_Default = 0,

                /// <summary> Guarantee that samples are delivered, may retry multiple times </summary>
                Reliable = 1,

                /// <summary> Attempt to deliver samples, but some may be lost if the network is not robust </summary>
                Best_Effort = 2,

                /// <summary> Reliability policy has not yet been set </summary>
                Unknown = 3,
            }
            public enum Durability
            {
                /// <summary> Implementation specific default </summary>
                System_Default = 0,

                /// <summary> The rmw publisher is responsible for persisting samples for “late-joining” subscribers </summary>
                Transient_Local = 1,

                /// <summary> Samples are not persistent </summary>
                Volatile = 2,

                /// <summary> Durability policy has not yet been set </summary>
                Unknown = 3,
            }
        }

        public class Duration
        {
            public ulong Seconds;
            public ulong Nanoseconds;
            public Duration(ulong seconds, ulong nanoseconds)
            {
                Seconds = seconds;
                Nanoseconds = nanoseconds;
            }

            public static Duration DoubleToTime(double time)
            {
                const ulong kcubed = 1000 * 1000 * 1000;
                double ftime = System.Math.Floor(time);

                ulong seconds = (ulong)(ftime);
                ulong nseconds = (ulong)((time - ftime) * kcubed);
                return new Duration(seconds, nseconds);
            }
        }
    }
#endif
}
