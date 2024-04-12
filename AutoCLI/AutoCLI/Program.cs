using CLI;

Console.WriteLine("Welcome to AutoCLI");


await CLI.GitHubAuther.SetAuthLink();

while (CLI.GitHubAuther.AccessToken == null)
{

}

List<string> repos = await ReviewAPI.GetUserReposAsync(CLI.GitHubAuther.AccessToken);

for (int i = 0; i < repos.Count; i++)
{
    Console.WriteLine(i.ToString() + ". " + repos[i]);
}

int input= -1;
bool isValidInput = false;

while (!isValidInput)
{
    Console.Write($"Please enter an integer less than {repos.Count}: ");
    string userInput = Console.ReadLine();

    if (int.TryParse(userInput, out input))
    {
        if (input < repos.Count)
        {
            isValidInput = true;
        }
        else
        {
            Console.WriteLine("Error: Please enter a valid index for repos");
        }
    }
    else
    {
        Console.WriteLine("Error: Invalid input. Please enter an integer.");
    }
}

string userChosenRepo = repos[input];

Console.WriteLine("Repo set to " +  userChosenRepo);

List<string> branches = await CLI.GitHubAPI.GetRepoBanches(
    userChosenRepo.Split("/")[0],
    userChosenRepo.Split("/")[1]
    ); ;

for (int i = 0; i < branches.Count; i++)
{
    Console.WriteLine(i.ToString() + ". " + branches[i]);
}

int branchesInput = -1;
bool isValidInputBranches = false;

while (!isValidInputBranches)
{
    Console.Write($"Please enter an integer less than {branches.Count}: ");
    string userInput = Console.ReadLine();

    if (int.TryParse(userInput, out branchesInput))
    {
        if (branchesInput < branches.Count)
        {
            isValidInputBranches = true;
        }
        else
        {
            Console.WriteLine("Error: Please enter a valid index for branches");
        }
    }
    else
    {
        Console.WriteLine("Error: Invalid input. Please enter an integer.");
    }
}

string userChosenBranch = branches[branchesInput];

Console.WriteLine("Branch set to " + userChosenBranch);

List<string> prTypes = new List<string> { "Backend", "Database", "Frontend", "Testing", "UI" };

for (int i = 0; i < prTypes.Count; i++)
{
    Console.WriteLine(i.ToString() + ". " + prTypes[i]);
}

int prInput = -1;
bool isValidInputPR = false;

while (!isValidInputPR)
{
    Console.Write($"Please enter an integer less than {prTypes.Count}: ");
    string userInput = Console.ReadLine();

    if (int.TryParse(userInput, out prInput))
    {
        if (prInput < prTypes.Count)
        {
            isValidInputPR = true;
        }
        else
        {
            Console.WriteLine("Error: Please enter a valid index for pr type");
        }
    }
    else
    {
        Console.WriteLine("Error: Invalid input. Please enter an integer.");
    }
}

string userChosenPRType = prTypes[prInput];

Console.WriteLine("PR Type set to " + userChosenPRType);

Console.WriteLine("\n");

Console.WriteLine("Please specify a name for your PR: ");
string prName = Console.ReadLine();


await CLI.GitHubAPI.CreatePR(
    userChosenRepo.Split("/")[0],
    userChosenRepo.Split("/")[1],
    prName,
    userChosenBranch,
    userChosenPRType
    );
