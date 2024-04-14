namespace Logistics
{
    public class Merger : Machine
    {
        public override void OnConstructed()
        {
            foreach (var inventory in InputInventories) inventory.maxItems = 1;

            OutputInventories[0].maxItems = 1;
        }

        private int iterator;
        private int Iterator
        {
            get { return iterator; }
            set { iterator = value % 3; }
        }

        public override void OnTick()
        {
            for (int i = 0; i < 3; i++)
            {
                Iterator++;
                if (TransferAnythingRandom(InputInventories[Iterator], OutputInventories[0]))
                { 
                    break;
                }
            }

            TryOutputAnything(0);
        }
    }

    public class Splitter : Machine
    {
        public override void OnConstructed()
        {
            foreach (var inventory in OutputInventories) inventory.maxItems = 1;

            InputInventories[0].maxItems = 1;
        }

        private int iterator;
        private int Iterator
        {
            get { return iterator; }
            set { iterator = value % 3; }
        }

        public override void OnTick()
        { 
            for (int i = 0; i < 3; i++)
            {
                TransferAnythingRandom(InputInventories[0], OutputInventories[Iterator]);
                Iterator++;

                if (TryOutputAnything(Iterator))
                { 
                    Iterator++;
                    break;
                }
            }
        }
    } 
}