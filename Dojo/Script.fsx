#r "System.Xml.Linq.dll"
#r "packages/FSharp.Data.2.0.4/lib/net40/FSharp.Data.dll"
open FSharp.Data
open System.Runtime
open System.Text.RegularExpressions

// ------------------------------------------------------------------
// WORD #1
//
let wb = WorldBankData.GetDataContext()

let allCountries = 
    wb
        .Regions.``North America``
        .Countries

let first = 
    allCountries
    |> Seq.minBy (fun x-> x.Indicators.``Life expectancy at birth, total (years)``.[2000])
    |> fun x -> x.Code.Substring(0, 2) //United States

// ------------------------------------------------------------------
// WORD #2
//
type Sample = XmlProvider<"data/bbc.xml">

let doc = Sample.GetSample()

let getNWord (word: string)(n) =
    word.Split(' ')
    |> Array.get <| n

let getNWordBackwards (word: string)(n) =
    getNWord word (word.Split(' ').Length - 1 - n)

let getLastWord (word: string) =
    word.Split(' ')
    |> Array.last
    |> fun x -> Regex.Replace(x, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled)

let second = 
    doc.Channel.Items
    |> Seq.where (fun x -> x.PubDate.Hour = 9 && x.PubDate.Minute = 5)
    |> Seq.last
    |> fun x -> getLastWord x.Title //all
    
// ------------------------------------------------------------------
// WORD #3
//
let key = "<api_key>"
let name = "Haugerud"
let data = 
  Http.RequestString
    ( "http://api.themoviedb.org/3/search/person",
      query = [ ("query", name); ("api_key", key) ],
      headers = [ HttpRequestHeaders.Accept HttpContentTypes.Json ] )

type PersonSearch = JsonProvider<"data/personsearch.json">
let sample = PersonSearch.Parse(data)

let directorId = 
    sample.Results
    |> Seq.last
    |> fun x -> x.Id

open System
let movieQuery = String.Format("http://api.themoviedb.org/3/person/{0}/movie_credits?api_key={1}", directorId, key)

let moviedata = 
  Http.RequestString
    ( movieQuery,
      headers = [ HttpRequestHeaders.Accept HttpContentTypes.Json ] )

type MovieList = JsonProvider<"data/moviecredits.json">
let movie = MovieList.Parse(moviedata)

let third = 
    movie.Crew
    |> Seq.where ( fun x -> x.Department = "Directing")
    |> Seq.head
    |> fun x -> getNWordBackwards x.OriginalTitle 2 //du

// ------------------------------------------------------------------
// WORD #4
//

type LibProvider = CsvProvider<"data/librarycalls.csv", ";">
let library = new LibProvider()
let fourth = 
    library.Rows
    |> Seq.where ( fun x -> x.``params`` = 2 && x.count = 1 && x.name.Length > 6 )
    |> Seq.head
    |> fun x -> x.name.Split('_')
    |> Array.last
    |> fun x -> String.Format("{0}s", x) //types
    
// ------------------------------------------------------------------
// WORD #5
//

let fb = FreebaseData.GetDataContext()

let fifth = 
    query { for e in fb.``Science and Technology``.Chemistry.``Chemical Elements`` do 
            where (e.``Atomic number``.Value = 36)
            select (e.Name) } 
    |> Seq.head
    |> fun x -> x.Substring(4,2) //to

// ------------------------------------------------------------------
// WORD #6
//
type TitanicProvider = CsvProvider<"data/titanic.csv">
let titanic = new TitanicProvider()
let sixth = 
    titanic.Rows
    |> Seq.where ( fun x -> x.Sex = "female" && x.Age = 19.0 && x.Embarked = "Q" )
    |> Seq.head
    |> fun x -> x.Name.Substring(19,3) //are

// ------------------------------------------------------------------
// WORD #7
//
let seventh =
    doc.Channel.Items
    |> Seq.where (fun x -> x.Title.Contains "Duran Duran")
    |> Seq.head
    |> fun x -> x.Description.Split ' ' |> fun y -> y.[13] //your

printf "%s %s %s %s %s %s %s\n" first second third fourth fifth sixth seventh