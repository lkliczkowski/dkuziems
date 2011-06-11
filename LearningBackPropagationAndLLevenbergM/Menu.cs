using System;
using System.Collections.Generic;

namespace LearningBPandLM
{
    delegate void VoidMenuEmpty();

    class Menu
    {
        private class MenuItem
        {
            public VoidMenuEmpty voidMenuCallback;
            public string info;

            public MenuItem(VoidMenuEmpty menuCallback, string info)
            {
                this.voidMenuCallback = menuCallback;
                this.info = info;
            }
        }



        private List<MenuItem> MenuList = new List<MenuItem>();

        public void Add(string info, VoidMenuEmpty menuOption)
        {
            MenuList.Add(new MenuItem(menuOption, info));
        }

        public void Remove(string info, VoidMenuEmpty menuOption)
        {
            for (int i = 0; i < MenuList.Count; i++)
                if (MenuList[i].voidMenuCallback.Equals(menuOption))
                {
                    MenuList.RemoveAt(i);
                }
        }

        public void Show()
        {
            Console.WriteLine();
            for (int i = 0; i < MenuList.Count; i++)
            {
                MenuItem mi = MenuList[i] as MenuItem;
                if(i == MenuList.Count - 1)
                    Console.WriteLine("\n\t[{0}] {1}\t", i + 1, mi.info);
                else
                    Console.WriteLine("\t[{0}] {1}\t", i + 1, mi.info);
            }

            int option = 0;
            try
            {
                option = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                option = 0;
            }

            if (checkOption(option, MenuList.Count))
            {
                MenuItem o = MenuList[option - 1] as MenuItem;
                VoidMenuEmpty menuCallback = o.voidMenuCallback;
                menuCallback();
            }
            else
                Console.WriteLine("Nieprawidłowa opcja");

        }
        
        private static bool checkOption(int op, int c)
        {
            if (op < 1 || c < op)
                return false;
            return true;
        }
    }
}

