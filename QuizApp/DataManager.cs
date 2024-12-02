using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Xml;

public class DataManager
    {
    private const string UserFile = "users.json";
    private const string QuizFile = "quizzes.json";
 


    public List<User> Users { get; private set; }
    public List<Quiz> Quizzes { get; private set; }
    

    public DataManager()
        {
        Users = LoadData<User>(UserFile);
        Quizzes = LoadData<Quiz>(QuizFile);
        }

    public void SaveData<T>( string fileName, List<T> data )
        {
        File.WriteAllText(fileName, JsonConvert.SerializeObject(data));
        }

    private List<T> LoadData<T>( string fileName )
        {
        if (File.Exists(fileName))
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(fileName));
        return new List<T>();
        }

    public User GetUser( string username ) => Users.FirstOrDefault(u => u.UserName == username);

    public void AddUser( User user )
        {
        Users.Add(user);
        SaveData(UserFile, Users);
        }

    public void AddQuiz( Quiz quiz )
        {
        Quizzes.Add(quiz);
        SaveData(QuizFile, Quizzes);
        }

    public void DeleteQuiz( Quiz quiz )
        {
        Quizzes.Remove(quiz);
        SaveData(QuizFile, Quizzes);
        }
    }
