namespace E621TelegramBot.Conversations
{
    public class ConversationMessage
    {
        //Telegram Id from
        public long From { get; set; }

        //Telegram Id to
        public long To { get; set; }

        public string Text { get; set; } = "";

    }
}
