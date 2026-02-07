namespace RosSharp.RosBridgeClient
{
#if ROS2
    public class QOS
    {
        public Policy.History HistoryPolicy;

        public uint Depth;

        public Policy.Reliability ReliabilityPolicy;
        public Policy.Durability DurabilityPolicy;

        public rmw_time_s Deadline;
        public rmw_time_s Lifespan;

        public Policy.Liveliness LivelinessPolicy;
        public rmw_time_s LivelinessLeaseDuration;

        public bool AvoidRosNamespaceConventions;

        public QOS(
            Policy.History history,
            uint depth,
            Policy.Reliability reliability,
            Policy.Durability durability,
            rmw_time_s deadline,
            rmw_time_s lifespan,
            Policy.Liveliness liveliness,
            rmw_time_s leaseDuration,
            bool avoidRosNamespaceConventions)
        {
            HistoryPolicy = history;
            Depth = depth;
            ReliabilityPolicy = reliability;
            DurabilityPolicy = durability;
            Deadline = deadline;
            Lifespan = lifespan;
            LivelinessPolicy = liveliness;
            LivelinessLeaseDuration = leaseDuration;
            AvoidRosNamespaceConventions = avoidRosNamespaceConventions;
        }

        /// <summary> Default QOS profiles as specified by the RMW qos_profiles.h file </summary>
        public class Presets
        {
            /// <summary> Default QOS profile </summary>
            public static readonly QOS Default = new QOS(
                Policy.History.RMW_QOS_POLICY_HISTORY_KEEP_LAST,
                10,
                Policy.Reliability.RMW_QOS_POLICY_RELIABILITY_RELIABLE,
                Policy.Durability.RMW_QOS_POLICY_DURABILITY_VOLATILE,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.Liveliness.RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                false);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS SensorData = new QOS(
                Policy.History.RMW_QOS_POLICY_HISTORY_KEEP_LAST,
                5,
                Policy.Reliability.RMW_QOS_POLICY_RELIABILITY_BEST_EFFORT,
                Policy.Durability.RMW_QOS_POLICY_DURABILITY_VOLATILE,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.Liveliness.RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                false);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS Services = new QOS(
                Policy.History.RMW_QOS_POLICY_HISTORY_KEEP_LAST,
                10,
                Policy.Reliability.RMW_QOS_POLICY_RELIABILITY_RELIABLE,
                Policy.Durability.RMW_QOS_POLICY_DURABILITY_VOLATILE,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.Liveliness.RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                false);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS ParameterEvents = new QOS(
                Policy.History.RMW_QOS_POLICY_HISTORY_KEEP_LAST,
                1000,
                Policy.Reliability.RMW_QOS_POLICY_RELIABILITY_RELIABLE,
                Policy.Durability.RMW_QOS_POLICY_DURABILITY_VOLATILE,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.Liveliness.RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                false);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS Rosout = new QOS(
                Policy.History.RMW_QOS_POLICY_HISTORY_KEEP_LAST,
                1000,
                Policy.Reliability.RMW_QOS_POLICY_RELIABILITY_RELIABLE,
                Policy.Durability.RMW_QOS_POLICY_DURABILITY_TRANSIENT_LOCAL,
                Policy.RMW_DURATION_UNSPECIFIED,
                new rmw_time_s(10, 0),
                Policy.Liveliness.RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                false);

            /// <summary> Default Sensor QOS profile </summary>
            public static readonly QOS SystemDefault = new QOS(
                Policy.History.RMW_QOS_POLICY_HISTORY_SYSTEM_DEFAULT,
                0,
                Policy.Reliability.RMW_QOS_POLICY_RELIABILITY_SYSTEM_DEFAULT,
                Policy.Durability.RMW_QOS_POLICY_DURABILITY_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.RMW_DURATION_UNSPECIFIED,
                Policy.Liveliness.RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT,
                Policy.RMW_DURATION_UNSPECIFIED,
                false);
        }

        public static class Policy
        {
            public static rmw_time_s RMW_DURATION_UNSPECIFIED = new rmw_time_s(0ul, 0ul);
            public static rmw_time_s RMW_DURATION_INFINITE = new rmw_time_s(9223372036ul, 854775807ul);

            public enum History
            {
                /// Implementation default for history policy
                RMW_QOS_POLICY_HISTORY_SYSTEM_DEFAULT,

                /// Only store up to a maximum number of samples, dropping oldest once max is exceeded
                RMW_QOS_POLICY_HISTORY_KEEP_LAST,

                /// Store all samples, subject to resource limits
                RMW_QOS_POLICY_HISTORY_KEEP_ALL,

                /// History policy has not yet been set
                RMW_QOS_POLICY_HISTORY_UNKNOWN
            }
            public enum Reliability
            {
                /// Implementation specific default
                RMW_QOS_POLICY_RELIABILITY_SYSTEM_DEFAULT,

                /// Guarantee that samples are delivered, may retry multiple times.
                RMW_QOS_POLICY_RELIABILITY_RELIABLE,

                /// Attempt to deliver samples, but some may be lost if the network is not robust
                RMW_QOS_POLICY_RELIABILITY_BEST_EFFORT,

                /// Reliability policy has not yet been set
                RMW_QOS_POLICY_RELIABILITY_UNKNOWN,

                /// Will match the majority of endpoints and use a reliable policy if possible
                /**
                 * A policy will be chosen at the time of creating a subscription or publisher.
                 * A reliable policy will by chosen if it matches with all discovered endpoints,
                 * otherwise a best effort policy will be chosen.
                 *
                 * The QoS policy reported by functions like `rmw_subscription_get_actual_qos` or
                 * `rmw_publisher_get_actual_qos` may be best available, reliable, or best effort.
                 *
                 * Services and clients are not supported and default to the reliability value in
                 * `rmw_qos_profile_services_default`.
                 *
                 * The middleware is not expected to update the policy after creating a subscription or
                 * publisher, even if the chosen policy is incompatible with newly discovered endpoints.
                 * Therefore, this policy should be used with care since non-deterministic behavior
                 * can occur due to races with discovery.
                 */
                RMW_QOS_POLICY_RELIABILITY_BEST_AVAILABLE
            }
            public enum Durability
            {
                /// Impplementation specific default
                RMW_QOS_POLICY_DURABILITY_SYSTEM_DEFAULT,

                /// The rmw publisher is responsible for persisting samples for “late-joining” subscribers
                RMW_QOS_POLICY_DURABILITY_TRANSIENT_LOCAL,

                /// Samples are not persistent
                RMW_QOS_POLICY_DURABILITY_VOLATILE,

                /// Durability policy has not yet been set
                RMW_QOS_POLICY_DURABILITY_UNKNOWN,

                /// Will match the majority of endpoints and use a transient local policy if possible
                /**
                 * A policy will be chosen at the time of creating a subscription or publisher.
                 * A transient local policy will by chosen if it matches with all discovered endpoints,
                 * otherwise a volatile policy will be chosen.
                 *
                 * In the case that a volatile policy is chosen for a subscription, any messages sent before
                 * the subscription was created by transient local publishers will not be received.
                 *
                 * The QoS policy reported by functions like `rmw_subscription_get_actual_qos` or
                 * `rmw_publisher_get_actual_qos` may be best available, transient local, or volatile.
                 *
                 * Services and clients are not supported and default to the durability value in
                 * `rmw_qos_profile_services_default`.
                 *
                 * The middleware is not expected to update the policy after creating a subscription or
                 * publisher, even if the chosen policy is incompatible with newly discovered endpoints.
                 * Therefore, this policy should be used with care since non-deterministic behavior
                 * can occur due to races with discovery.
                 */
                RMW_QOS_POLICY_DURABILITY_BEST_AVAILABLE
            }

            public enum Liveliness
            {
                /// Implementation specific default
                RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT = 0,

                /// The signal that establishes a Topic is alive comes from the ROS rmw layer.
                RMW_QOS_POLICY_LIVELINESS_AUTOMATIC = 1,

                /// The signal that establishes a Topic is alive is at the Topic level. Only publishing a message
                /// on the Topic or an explicit signal from the application to assert liveliness on the Topic
                /// will mark the Topic as being alive.
                // Using `3` for backwards compatibility.
                RMW_QOS_POLICY_LIVELINESS_MANUAL_BY_TOPIC = 3,

                /// Liveliness policy has not yet been set
                RMW_QOS_POLICY_LIVELINESS_UNKNOWN = 4,

                /// Will match the majority of endpoints and use a manual by topic policy if possible
                /**
                 * A policy will be chosen at the time of creating a subscription or publisher.
                 * A manual by topic policy will by chosen if it matches with all discovered endpoints,
                 * otherwise an automatic policy will be chosen.
                 *
                 * The QoS policy reported by functions like `rmw_subscription_get_actual_qos` or
                 * `rmw_publisher_get_actual_qos` may be best available, automatic, or manual by topic.
                 *
                 * Services and clients are not supported and default to the liveliness value in
                 * `rmw_qos_profile_services_default`.
                 *
                 * The middleware is not expected to update the policy after creating a subscription or
                 * publisher, even if the chosen policy is incompatible with newly discovered endpoints.
                 * Therefore, this policy should be used with care since non-deterministic behavior
                 * can occur due to races with discovery.
                 */
                RMW_QOS_POLICY_LIVELINESS_BEST_AVAILABLE = 5
            }
        }

        public class rmw_time_s
        {
            public ulong sec;
            public ulong nsec;
            public rmw_time_s(ulong s, ulong n)
            {
                sec = s;
                nsec = n;
            }
        }
    }
#endif
}
