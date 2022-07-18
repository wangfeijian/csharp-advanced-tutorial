using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using BankInterface;

namespace MEFDemoForDll
{
    class Program
    {
        [ImportMany(typeof(ICard))]
        public IEnumerable<ICard> cards { get; set; }
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Compose();

            foreach (var card in p.cards)
            {
                Console.WriteLine(card.GetCountInfo());
            }

            Console.ReadKey();
        }

        private void Compose()
        {
            var catalog = new DirectoryCatalog("Cards");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }
}
