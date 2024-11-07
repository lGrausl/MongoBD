using Microsoft.Identity.Client;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

/*
char select = char.Parse(Console.ReadLine());
if (select == y) managerCollections.createColections();
else if (select == n)
{
    Console.Clear();
    Main();
}
*/

namespace TetsBD
{
    class Program
    {
        static void Main()
        {
            MenuManager menuCollections = new MenuManager();
            menuCollections.menuCollections();
        }

    }

    public class MenuManager
    {
        public void menuCollections()//меню самой базы
        {
            ManagingCollections managerCollections = new ManagingCollections();

            Console.WriteLine("Список колекций:");
            managerCollections.showAllcollections();
            Console.WriteLine("\n");
            Console.WriteLine("Нажмите 1 - добавление колекции");
            Console.WriteLine("Нажмите 2 - удаление колекции");
            Console.WriteLine("Нажмите 3 - выбрать колекцию");
            Console.WriteLine("Нажмите 4 - очистить консоль");

            char switcher = Convert.ToChar(Console.ReadLine());
            switch (switcher)
            {
                case '1':
                    managerCollections.createColections();
                    break;
                case '2':
                    managerCollections.deleteColections();
                    break;
                case '3':
                    menuTable();
                    break;
                case '4':
                    Console.Clear();
                    break;
                default:
                    menuCollections();
                    break;
            }

            menuCollections();
        }

        public void menuTable()//Меню таблицы 
        {


            ManagerTable table = new ManagerTable();
            ConnectBD tables = new ConnectBD();

            Console.WriteLine("Введите имя коллекции");
            string nameTable = Console.ReadLine();
            var users = tables.tableConnect(nameTable);


            Console.WriteLine("Нажмите 1 - просмотреть записи");
            Console.WriteLine("Нажмите 2 - создать запись");
            Console.WriteLine("Нажмите 3 - удалить запись");
            Console.WriteLine("Нажмите 4 - поиск по фильтру");
            Console.WriteLine("Нажмите 5 - очистить консоль");

            char switcher = Convert.ToChar(Console.ReadLine());

            switch (switcher)
            {
                case '1':
                    table.ShowAllCollections(users);
                    break;
                case '2':
                    table.CreateRecord(users);
                    break;
                case '3':
                    table.DeleteRecord(users);
                    break;
                case '4':
                    table.filteredData(users);
                    break;
                case '5':
                    Console.Clear();
                    break;
                default:
                    menuCollections();
                    break;
            }

            menuCollections();
        }

    }

    public class ManagingCollections//методы работы с базой
    {
        string nameColections;

        ConnectBD db = new ConnectBD(); //подключение к самой базе

        public void createColections()
        {
            Console.WriteLine("Введите название таблици:");

            nameColections = Console.ReadLine();
            db.connctBd().CreateCollection(nameColections);

            Console.WriteLine("Таблица создана");
        }


        public void deleteColections()
        {
            Console.WriteLine("Введите название таблици:");

            nameColections = Console.ReadLine();
            db.connctBd().DropCollection(nameColections);

            Console.WriteLine("Таблица Удалена");
        }


        public void showAllcollections()
        {
            var collections = db.connctBd().ListCollectionNames();
            foreach (var collection in collections.ToList())
            {
                Console.WriteLine(collection);
            }
        }
    }

    public class ManagerTable//меторы работы с колекцией в базе
    {
        ConnectBD table = new ConnectBD();

        public void ShowAllCollections(IMongoCollection<BsonDocument> users)//показать все записи
        {
            List<BsonDocument> tables = users.Find(new BsonDocument()).ToList();

            foreach (var table in tables)
            {
                Console.WriteLine(table);
            }
        }

        public void filteredData(IMongoCollection<BsonDocument> users)//показать данные только с определённым параметрам
        {
            Console.WriteLine("По кому фильту выбрать:");
            Console.WriteLine("1.имя");
            Console.WriteLine("2.фамелия");
            Console.WriteLine("3.отчество");
            Console.WriteLine("4.возраст");
            char switcher = Convert.ToChar(Console.ReadLine());

            Console.WriteLine("Введите значание по каторому фильровать");

            string filterArg = Console.ReadLine();
            if (switcher == 1)
            {
                var filter = new BsonDocument { { "First name", filterArg } };
                List<BsonDocument> filterTable = users.Find(filter).ToList();
                foreach (var str in filterTable) { Console.WriteLine(str); }
            }
            else if (switcher == 2)
            {
                var filter = new BsonDocument { { "Family name", filterArg } };
                List<BsonDocument> filterTable = users.Find(filter).ToList();
                foreach (var str in filterTable) { Console.WriteLine(str); }
            }
            else if (switcher == 3)
            {
                var filter = new BsonDocument { { "Patronymic", filterArg } };
                List<BsonDocument> filterTable = users.Find(filter).ToList();
                foreach (var str in filterTable) { Console.WriteLine(str); }
            }
            else if (switcher == 4)
            {
                var filter = new BsonDocument { { "old", filterArg } };
                List<BsonDocument> filterTable = users.Find(filter).ToList();
                foreach (var str in filterTable) { Console.WriteLine(str); }
            }
            else
            {
                Console.WriteLine("Введёно не известное значение");
            }

        }

        public void CreateRecord(IMongoCollection<BsonDocument> users)//Добавление записи
        {
            Console.WriteLine("Введите имя");
            string name = Console.ReadLine();
            Console.WriteLine("Введите фамелию");
            string family = Console.ReadLine();
            Console.WriteLine("Введите отчество");
            string patronymic = Console.ReadLine();
            Console.WriteLine("Введите возраст");
            int old = Convert.ToInt32(Console.ReadLine());

            BsonDocument recordStructure = new BsonDocument//структура таблицы потом можно под 
        {
            {"First name", name},
            {"Family name", family},
            {"Patronymic ", patronymic},
            {"old", old}
        };

            users.InsertMany(new List<BsonDocument> { recordStructure });

        }

        public void DeleteRecord(IMongoCollection<BsonDocument> users)//Добавление записи
        {
            Console.WriteLine("Удалить");
            Console.WriteLine("1.Все записи по одному фильтру");
            Console.WriteLine("2.Одну запись по параметрам");

            char switcher = Convert.ToChar(Console.ReadLine());
            if (switcher == '1')
            {
                Console.WriteLine("Введите названи фильтра по каторому хотите удалить:\n\"First name\" \n\"Family name\"\n\"Patronymic\"\n\"old\"");
                string filtres = Console.ReadLine();
                Console.WriteLine("Введите сам параметр");
                string paramets = Console.ReadLine();
                var result = users.DeleteMany(new BsonDocument(filtres, paramets));
            }
            else{ Console.WriteLine("Тут я уже не успел"); }
        }

    }

    public class ConnectBD//Подключение к базе 
    {
        public IMongoDatabase connctBd()//полключение к самой безе 
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("test");
            return db;
        }

        public IMongoCollection<BsonDocument> tableConnect(string nameTable)//получене самой таблицы
        {
            var table = connctBd().GetCollection<BsonDocument>(nameTable);
            return table;
        }
    }   


}
