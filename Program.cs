using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace m9_2
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        // Поле для хранения подключения к базе данных SQLite.
        private static SQLiteConnection connection;

        static void Main()
        {
            connection = new SQLiteConnection("Data Source=management.db;Version=3;");
            connection.Open();
            CreateTables(); // Создание таблиц, если они еще не существуют.
            while (true)
            {
                ShowMainMenu();
            }
        }

        // Метод для отображения главного меню с вариантами управления проектами, задачами и сотрудниками.
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Управление задачами проекта:");
            Console.WriteLine("1. Добавить проект");
            Console.WriteLine("2. Удалить проект");
            Console.WriteLine("3. Редактировать проект");
            Console.WriteLine("4. Добавить задачу");
            Console.WriteLine("5. Удалить задачу");
            Console.WriteLine("6. Редактировать задачу");
            Console.WriteLine("7. Добавить сотрудника");
            Console.WriteLine("8. Удалить сотрудника");
            Console.WriteLine("9. Показать отчеты");
            Console.WriteLine("10. Выход");
            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine();
            ExecuteChoice(choice); // Обработка выбранного действия.
        }

        // Метод для выполнения действия, выбранного пользователем.
        static void ExecuteChoice(string choice)
        {
            switch (choice)
            {
                case "1": AddProject(); break;
                case "2": RemoveProject(); break;
                case "3": EditProject(); break;
                case "4": AddTask(); break;
                case "5": RemoveTask(); break;
                case "6": EditTask(); break;
                case "7": AddEmployee(); break;
                case "8": RemoveEmployee(); break;
                case "9": ShowReports(); break;
                case "10": CloseConnection(); return;
                default: Console.WriteLine("Неверный выбор, попробуйте снова."); break;
            }
        }

        // Метод для создания таблиц Projects, Tasks и Employees, если они еще не существуют в базе данных.
        static void CreateTables()
        {
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Projects (Id INTEGER PRIMARY KEY, Name TEXT, Status TEXT)";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (Id INTEGER PRIMARY KEY, Title TEXT, ProjectId INTEGER, AssignedTo TEXT, Status TEXT)";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Employees (Id INTEGER PRIMARY KEY, Name TEXT)";
                command.ExecuteNonQuery();
            }
        }

        // Метод для добавления нового проекта в таблицу Projects.
        static void AddProject()
        {
            var name = Prompt("Введите имя проекта: ");
            var status = Prompt("Введите статус проекта: ");
            using (var command = new SQLiteCommand("INSERT INTO Projects (Name, Status) VALUES (@name, @status)", connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@status", status);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Проект добавлен.");
            PromptContinue();
        }

        // Метод для удаления проекта по ID.
        static void RemoveProject()
        {
            var id = int.Parse(Prompt("Введите ID проекта для удаления: "));
            using (var command = new SQLiteCommand("DELETE FROM Projects WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Проект удален.");
            PromptContinue();
        }

        // Метод для редактирования информации о проекте по ID, включая его имя и статус.
        static void EditProject()
        {
            var id = int.Parse(Prompt("Введите ID проекта для редактирования: "));
            var name = Prompt("Новое имя проекта (оставьте пустым для пропуска): ");
            var status = Prompt("Новый статус проекта (оставьте пустым для пропуска): ");
            using (var command = new SQLiteCommand("UPDATE Projects SET Name = COALESCE(NULLIF(@name, ''), Name), Status = COALESCE(NULLIF(@status, ''), Status) WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@status", status);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Проект обновлен.");
            PromptContinue();
        }

        // Метод для добавления новой задачи в таблицу Tasks, привязанной к проекту.
        static void AddTask()
        {
            var title = Prompt("Введите заголовок задачи: ");
            var projectId = int.Parse(Prompt("Введите ID проекта: "));
            var assignedTo = Prompt("Введите имя сотрудника: ");
            var status = Prompt("Введите статус задачи: ");
            using (var command = new SQLiteCommand("INSERT INTO Tasks (Title, ProjectId, AssignedTo, Status) VALUES (@title, @projectId, @assignedTo, @status)", connection))
            {
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@assignedTo", assignedTo);
                command.Parameters.AddWithValue("@status", status);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Задача добавлена.");
            PromptContinue();
        }

        // Метод для удаления задачи по ID.
        static void RemoveTask()
        {
            var id = int.Parse(Prompt("Введите ID задачи для удаления: "));
            using (var command = new SQLiteCommand("DELETE FROM Tasks WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Задача удалена.");
            PromptContinue();
        }

        // Метод для редактирования задачи по ID, с возможностью изменения заголовка, сотрудника и статуса.
        static void EditTask()
        {
            var id = int.Parse(Prompt("Введите ID задачи для редактирования: "));
            var title = Prompt("Новый заголовок задачи (оставьте пустым для пропуска): ");
            var assignedTo = Prompt("Новое имя сотрудника (оставьте пустым для пропуска): ");
            var status = Prompt("Новый статус задачи (оставьте пустым для пропуска): ");
            using (var command = new SQLiteCommand("UPDATE Tasks SET Title = COALESCE(NULLIF(@title, ''), Title), AssignedTo = COALESCE(NULLIF(@assignedTo, ''), AssignedTo), Status = COALESCE(NULLIF(@status, ''), Status) WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@assignedTo", assignedTo);
                command.Parameters.AddWithValue("@status", status);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Задача обновлена.");
            PromptContinue();
        }

        // Метод для добавления нового сотрудника в таблицу Employees.
        static void AddEmployee()
        {
            var name = Prompt("Введите имя сотрудника: ");
            using (var command = new SQLiteCommand("INSERT INTO Employees (Name) VALUES (@name)", connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Сотрудник добавлен.");
            PromptContinue();
        }

        // Метод для удаления сотрудника по ID.
        static void RemoveEmployee()
        {
            var id = int.Parse(Prompt("Введите ID сотрудника для удаления: "));
            using (var command = new SQLiteCommand("DELETE FROM Employees WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Сотрудник удален.");
            PromptContinue();
        }

        // Метод ShowReports служит для отображения отчетов по проектам и задачам (можно дополнить по необходимости).
        // Метод ShowReports для отображения краткого отчета по проектам и задачам.
        static void ShowReports()
        {
            Console.Clear();
            Console.WriteLine("Отчеты по проектам и задачам:");

            // Запрос для выборки информации о проектах и задачах.
            string query = @"
        SELECT p.Name AS ProjectName, p.Status AS ProjectStatus,
               t.Title AS TaskTitle, t.AssignedTo AS TaskAssignedTo, t.Status AS TaskStatus
        FROM Projects p
        LEFT JOIN Tasks t ON p.Id = t.ProjectId
        ORDER BY p.Name";

            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Выводим название проекта, если он содержит хотя бы одну строку.
                    Console.WriteLine($"\nПроект: {reader["ProjectName"]} | Статус: {reader["ProjectStatus"]}");

                    // Проверяем наличие задачи у проекта.
                    string taskTitle = reader["TaskTitle"].ToString();
                    if (!string.IsNullOrEmpty(taskTitle))
                    {
                        Console.WriteLine($"   Задача: {taskTitle}, Исполнитель: {reader["TaskAssignedTo"]}, Статус: {reader["TaskStatus"]}");
                    }
                    else
                    {
                        Console.WriteLine("   Нет задач для этого проекта.");
                    }
                }
            }

            PromptContinue();
        }
 

        // Вспомогательный метод Prompt для ввода данных пользователем.
        static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        // Метод для приостановки программы перед продолжением.
        static void PromptContinue()
        {
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        // Метод для закрытия соединения с базой данных перед завершением программы.
        static void CloseConnection()
        {
            connection.Close();
            Console.WriteLine("Программа завершена.");
        }
    }
}
