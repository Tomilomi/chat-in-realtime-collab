using ErrorOr;

namespace Domain.Errors
{
    public static partial class DomainErrors
    {
        public static class Message
        {
            public static class Validation
            {
                public static Error Blank => Error.Validation(
                    code: "Message.Blank",
                    description: "The message content cannot be blank."
                );

                public static Error TooLong => Error.Validation(
                    code: "Message.TooLong",
                    description: "The message content exceeds the maximum allowed length."
                );
            }
        }
    }
}