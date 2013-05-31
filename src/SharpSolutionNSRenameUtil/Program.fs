// Learn more about F# at http://fsharp.net
open System.IO;
   
   
let replaceNSInFile (fileInfo : FileInfo) (oldNS : string) (newNS : string) =
    let filename = fileInfo.FullName;
    let fileOriginalContent = File.ReadAllText filename
    let modifiedContent = fileOriginalContent.Replace(oldNS, newNS)
    File.WriteAllText(filename, modifiedContent)

let processFile (fileInfo : FileInfo) oldNS newNS =
    let ext = fileInfo.Extension;
    let filename = fileInfo.FullName;
    match ext with
    | ".cs" -> replaceNSInFile fileInfo oldNS newNS
    | ".sln" -> replaceNSInFile fileInfo oldNS newNS
    | ".csproj" -> replaceNSInFile fileInfo oldNS newNS
    | a -> ()

    if (filename.Contains oldNS) then
        let newFileName = filename.Replace(oldNS, newNS);
        fileInfo.MoveTo(newFileName); 

 

let rec renameNSForDirectoryRec (dirInfo : DirectoryInfo) oldNS newNS =
        
    let dirName = dirInfo.Name;

    if dirName.Contains oldNS then
        dirInfo.MoveTo <| dirInfo.FullName.Replace(oldNS, newNS)

    let files = dirInfo.GetFiles();
    let dirs = dirInfo.GetDirectories();

    if (not (dirName = ".git" || dirName = ".nuget" || dirName.StartsWith ".")) then
        if dirName = "bin" || dirName = "obj" then
            dirInfo.Delete(true);
        else
            Array.iter (fun fi -> processFile fi oldNS newNS) files
            Array.iter (fun d -> renameNSForDirectoryRec d oldNS newNS) dirs
     

[<EntryPoint>]
let main args = // Usage ***.exe -SolutionDirPath #dirPath# -OldNS #oldNS# -NewNS #newNS#
    let dirPath : string = Array.get args 1;
    let oldNS : string = Array.get args 3;
    let newNS : string = Array.get args 5;
    let dirInfo = new DirectoryInfo(dirPath);
    renameNSForDirectoryRec dirInfo oldNS newNS;
    0   
    
    