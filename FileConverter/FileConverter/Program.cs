using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

Person p;
FamilyData f;
PhoneData t; ;
AdressData a;

string? line;
string[] chars = new string[] {};
var personList = new PersonList {
    People = new List<PersonData>()
};

try
{
    var sr = new StreamReader(@"C:\FileConverter\FileConverter\input2.txt");
    line = sr.ReadLine();

    while (line != null)
    {
        if(personList.People.Count == 0)
        {
            chars = GetWords(line);
        }
        
        if(chars != null && chars.Length > 0) 
        {
            var ch = chars[0];

            if(ch == "P")
            {
                p = new Person();
                SetPerson(p, chars);

                line = GetLine(sr, personList, p);
                if (line == null) break;
                chars = GetWords(line);
                if (chars == null || chars.Length == 0) break;

                ch = chars[0];

                while(ch == "T" || ch == "A")
                {
                    if(ch == "T")
                    {
                        t = new PhoneData();
                        SetPhone(t, chars);
                        p.Phone = t;

                        line = GetLine(sr, personList, p);
                        if (line == null) break;
                        chars = GetWords(line);
                        if (chars == null || chars.Length == 0) break;

                        ch = chars[0];
                    }
                    if(ch == "A")
                    {
                        a = new AdressData();
                        SetAdress(a, chars);
                        p.Adress = a;

                        line = GetLine(sr, personList, p);
                        if (line == null) break;
                        chars = GetWords(line);
                        if (chars == null || chars.Length == 0) break;
                        
                        ch = chars[0];
                    }
                }
                if(ch == "F")
                {
                    while(chars != null && ch != "P")
                    {
                        f = new FamilyData();
                        SetFamily(p, f, chars);

                        line = GetLine(sr, personList, p);
                        if (line == null) break;
                        chars = GetWords(line);
                        if (chars == null || chars.Length == 0) break;
                        
                        ch = chars[0];
                            
                        if(chars != null && chars.Length > 0)
                        {
                            ch = chars[0];

                            while (ch == "A" || ch == "T")
                            {
                                if(chars != null && ch == "A")
                                {
                                    a = new AdressData();
                                    SetAdress(a, chars);
                                    f.Adress = a;

                                    line = GetLine(sr, personList, p);
                                    if (line == null) break;
                                    chars = GetWords(line);
                                    if (chars == null || chars.Length == 0) break;
                                    
                                    ch = chars[0];
                                }
                                else if(chars != null && ch == "T")
                                {
                                    t = new PhoneData();
                                    SetPhone(t, chars);
                                    f.Phone = t;

                                    line = GetLine(sr, personList, p);
                                    if (line == null) break;
                                    chars = GetWords(line);
                                    if (chars == null || chars.Length == 0) break;

                                    ch = chars[0];
                                }

                            }
                            if(p.Family != null){
                                p.Family.Add(f);
                            }
                        }
                    }
                }
                
                if(line != null)
                {
                    personList.People.Add(new PersonData { Person = p });
                }  
            }
        }
    }
    sr.Close();

}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    await using FileStream createStream = File.Create(@"C:\FileConverter\FileConverter\output.json");
    await JsonSerializer.SerializeAsync(createStream, personList);
    Console.WriteLine("Done.");
}



void SetPerson(Person p, string[] chars){
    p.FirstName = chars[1] ?? "";
    p.LastName = chars[2] ?? "";
    p.Family = new List<FamilyData>();
}

void SetPhone(PhoneData t, string[] chars){
    t.Mobile = chars[1] ?? "";
    t.LandLine = "";

    if(chars != null && chars.Length > 2) {
        t.LandLine = chars[2];
    }
}

void SetAdress(AdressData a, string[] chars)
{
    a.Street = chars[1] ?? "";
    a.City = chars[2] ?? "";
    a.Zip = "";

    if(chars != null && chars.Length > 3)
    {
        a.Zip = chars[3] ?? ""; 
    }
}

void SetFamily(Person p, FamilyData f, string[] chars){
    f.Name = chars[1];
    f.Born = "";

    if(chars != null && chars.Length > 2)
    {
        f.Born = chars[2];
    }
}

string? GetLine(StreamReader sr, PersonList personList, Person p){
    var line = sr.ReadLine();
    if(line == null ) {
        var pd = new PersonData() {
            Person = p
        };
        if(personList != null && personList.People != null)
        {
            personList.People.Add(pd);
        }
    }
    return line;
}

string[] GetWords(string? line)
{
    string[] chars;
    if(line != null){
        chars = line.Split('|');

        if(chars != null && chars.Length > 0)
        {
            Console.WriteLine(line);
            return chars;
        }
    }
    return new string [] {};  
}

public class PersonList {
    public List<PersonData>? People { get; set; }
}

public class PersonData {
    public Person? Person { get; set; }
}

public class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public AdressData? Adress { get; set; }
    public PhoneData? Phone { get; set; }
    public List<FamilyData>? Family { get; set; }

}

public class AdressData {
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Zip { get; set; }
}

public class PhoneData {
    public string? Mobile { get; set; }
    public string? LandLine { get; set; }
}

public class FamilyData {
    public string? Name { get; set; }
    public string? Born { get; set; }
    public AdressData? Adress { get; set; }
    public PhoneData? Phone { get; set; }
}
