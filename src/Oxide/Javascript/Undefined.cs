namespace Oxide.JavaScript
{
    public class Undefined
    {
        public static readonly Undefined Value = new Undefined();

        private Undefined()
        {
        }

        public override string ToString() => @"[undefined]";
    }
}
