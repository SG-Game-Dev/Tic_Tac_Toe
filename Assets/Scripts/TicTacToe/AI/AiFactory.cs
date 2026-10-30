namespace TicTacToe
{
    // AiMode के हिसाब से सही AI बनाकर देता है (नया level जोड़ना हो तो बस यहाँ case जोड़ो)
    public static class AiFactory
    {
        internal static IAiStrategy Create(AiMode mode, AiContext ctx)
        {
            switch (mode)
            {
                case AiMode.SmartAi: return new SmartAi(ctx);
                case AiMode.Minimax: return new MinimaxAi(ctx);
                case AiMode.Easy:
                default: return new EasyAi(ctx);
            }
        }
    }
}
