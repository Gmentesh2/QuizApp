using System;
using System.Linq;

class Program
    {
    private static readonly DataManager _dataManager = new DataManager();

    static void Main( string[] args )
        {
        ShowTopUsers();
        while (true)
            {
            Console.WriteLine("\nChoose: [Register] [Login] [Exit]");
            var input = Console.ReadLine()?.ToLower();
            if (input == "register")
                Register();
            else if (input == "login")
                Login();
            else if (input == "exit")
                break;
            }
        }

    static void Register()
        {
        Console.Write("Enter Username: ");
        var username = Console.ReadLine();
        Console.Write("Enter Password: ");
        var password = Console.ReadLine();

        if (_dataManager.GetUser(username) != null)
            {
            Console.WriteLine("User already exists!");
            return;
            }

        var user = new User { UserName = username, Password = password, Coins = 0 };
        _dataManager.AddUser(user);
        Console.WriteLine("Registration successful!");
        }

    static void Login()
        {
        Console.Write("Enter Username: ");
        var username = Console.ReadLine();
        Console.Write("Enter Password: ");
        var password = Console.ReadLine();

        var user = _dataManager.GetUser(username);
        if (user != null && user.Password == password)
            {
            Console.WriteLine("Login successful!");
            UserMenu(user);
            }
        else
            {
            Console.WriteLine("Invalid credentials.");
            }
        }

    static void UserMenu( User user )
        {
        while (true)
            {
            Console.WriteLine("\nChoose: [CreateQuiz] [SolveQuiz] [EditQuiz] [DeleteQuiz] [Logout]  ");
            var input = Console.ReadLine()?.ToLower();
            if (input == "createquiz")
                CreateQuiz(user);
            else if (input == "solvequiz")
                SolveQuiz(user);
            else if (input == "deletequiz")
                DeleteQuiz(user);
            else if (input == "editquiz")
                EditQuiz(user);
            else if (input == "logout")
                break;
            }
        }

    static void CreateQuiz( User user )
        {
        Console.Write("Enter Quiz Title: ");
        var title = Console.ReadLine();

        var quiz = new Quiz { Title = title, CreatorUserName = user.UserName, QuizId = _dataManager.Quizzes.Count + 1, Questions = new List<Question>() };

        for (int i = 1; i <= 5; i++)
            {
            var question = new Question();
            Console.Write($"Enter Question {i}: ");
            question.Text = Console.ReadLine();
            question.Options = new List<string>();

            for (int j = 1; j <= 4; j++)
                {
                Console.Write($"Option {j}: ");
                question.Options.Add(Console.ReadLine());
                }

            int correctOption;
            do
                {
                Console.Write("Correct Option (1-4): ");
                while (!int.TryParse(Console.ReadLine(), out correctOption))
                    {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                    Console.Write("Correct Option (1-4): ");
                    }

                if (correctOption < 1 || correctOption > 4)
                    {
                    Console.WriteLine("Invalid choice. The correct option must be between 1 and 4.");
                    }

                } while (correctOption < 1 || correctOption > 4);

            question.CorrectAnswerIndex = correctOption - 1;
            quiz.Questions.Add(question);
            }
        }

    static void SolveQuiz( User user )
        {
        var availableQuizzes = _dataManager.Quizzes.Where(q => q.CreatorUserName != user.UserName).ToList();
        if (!availableQuizzes.Any())
            {
            Console.WriteLine("No quizzes available.");
            return;
            }

        Console.WriteLine("Available Quizzes:");
        foreach (var quiz in availableQuizzes)
            Console.WriteLine($"- {quiz.Title}");

        Console.Write("Enter Quiz Title to solve: ");
        var title = Console.ReadLine();
        var selectedQuiz = availableQuizzes.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (selectedQuiz == null)
            {
            Console.WriteLine("Quiz not found.");
            return;
            }

        int totalScore = 0;

        foreach (var question in selectedQuiz.Questions)
            {
            Console.WriteLine($"\n{question.Text}");
            for (int i = 0; i < question.Options.Count; i++)
                Console.WriteLine($"{i + 1}. {question.Options[i]}");

            Console.Write("Your answer (1-4): ");
            int userAnswer;
            while (!int.TryParse(Console.ReadLine(), out userAnswer) || userAnswer < 1 || userAnswer > 4)
                {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                Console.Write("Your answer (1-4): ");
                }

            userAnswer--;

            if (userAnswer == question.CorrectAnswerIndex)
                {
                totalScore += 20;
                Console.WriteLine("Correct! +20 coins.");
                }
            else
                {
                totalScore -= 20;
                Console.WriteLine("Incorrect! -20 coins.");
                }
            }

        user.Coins += totalScore;
        Console.WriteLine($"\nQuiz complete! Your total score: {totalScore} Coins: {user.Coins}");
        _dataManager.SaveData("users.json", _dataManager.Users);
        }



    static void ShowTopUsers()
        {
        var topUsers = _dataManager.Users.OrderByDescending(u => u.Coins).Take(10).ToList();
        Console.WriteLine("\nTop 10 Users:");
        foreach (var user in topUsers)
            Console.WriteLine($"{user.UserName}: {user.Coins} coins");
        }


    //

    public static void EditQuiz( User user )
        {
        Console.WriteLine("Your Quizzes:");
        var userQuizzes = _dataManager.Quizzes.Where(q => q.CreatorUserName == user.UserName).ToList();

        if (!userQuizzes.Any())
            {
            Console.WriteLine("You have no quizzes to edit.");
            return;
            }

        foreach (var quiz in userQuizzes)
            Console.WriteLine($"- {quiz.Title}");

        Console.Write("Enter the title of the quiz you want to edit: ");
        var title = Console.ReadLine();
        var quizToEdit = userQuizzes.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (quizToEdit == null)
            {
            Console.WriteLine("Quiz not found.");
            return;
            }

        Console.Write("Enter new title (press enter to keep current title): ");
        var newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle))
            quizToEdit.Title = newTitle;

        for (int i = 0; i < quizToEdit.Questions.Count; i++)
            {
            var question = quizToEdit.Questions[i];
            Console.WriteLine($"\nEditing Question {i + 1}: {question.Text}");
            Console.Write("New question text (press enter to keep current): ");
            var newQuestionText = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newQuestionText))
                question.Text = newQuestionText;

            for (int j = 0; j < question.Options.Count; j++)
                {
                Console.Write($"Option {j + 1}: {question.Options[j]} ");
                Console.Write("New option text (press enter to keep current): ");
                var newOptionText = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newOptionText))
                    question.Options[j] = newOptionText;
                }

            int correctOption;
            do
                {
                Console.Write("Enter new correct option (1-4, press enter to keep current): ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;

                while (!int.TryParse(input, out correctOption) || correctOption < 1 || correctOption > 4)
                    {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                    Console.Write("Enter new correct option (1-4): ");
                    input = Console.ReadLine();
                    }

                question.CorrectAnswerIndex = correctOption - 1;

                } while (false);
            }

        _dataManager.SaveData("quizzes.json", _dataManager.Quizzes);
        Console.WriteLine("Quiz updated successfully!");
        }

    public static void DeleteQuiz( User user )
        {
        Console.WriteLine("Your Quizzes:");
        var userQuizzes = _dataManager.Quizzes.Where(q => q.CreatorUserName == user.UserName).ToList();

        if (!userQuizzes.Any())
            {
            Console.WriteLine("You have no quizzes to delete.");
            return;
            }

        foreach (var quiz in userQuizzes)
            Console.WriteLine($"- {quiz.Title}");

        Console.Write("Enter the title of the quiz you want to delete: ");
        var title = Console.ReadLine();
        var quizToDelete = userQuizzes.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (quizToDelete == null)
            {
            Console.WriteLine("Quiz not found.");
            return;
            }

        _dataManager.Quizzes.Remove(quizToDelete);
        _dataManager.SaveData("quizzes.json", _dataManager.Quizzes);
        Console.WriteLine("Quiz deleted successfully!");
        }




    }
