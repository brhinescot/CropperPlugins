# SetVersion.ps1
#
# Set the version in all the AssemblyInfo.cs or AssemblyInfo.vb files in any subdirectory.
#
# usage:
#  from cmd.exe:
#     powershell.exe SetVersion.ps1  2.8.3.0
#
#  from powershell.exe prompt:
#     .\SetVersion.ps1  2.8.3.0
#
# last saved Time-stamp: <2010-December-22 15:59:58>
#


function Usage
{
  echo "Usage: ";
  echo "  from cmd.exe: ";
  echo "     powershell.exe SetVersion.ps1  2.8.3.0";
  echo " ";
  echo "  from powershell.exe prompt: ";
  echo "     .\SetVersion.ps1  2.8.3.0";
  echo " ";
}


function Update-SourceVersion
{
    Param ([string]$Version)

    $NewVersion = 'AssemblyVersion("' + $Version + '")';
    $NewFileVersion = 'AssemblyFileVersion("' + $Version + '")';

    foreach ($o in $input)
    {
        $av = select-string AssemblyVersion $o
        $fv = select-string AssemblyVersion $o

        if ($av -ne $null -or $fv -ne $null)
        {
            if ($o.Attributes -band [System.IO.FileAttributes]::ReadOnly)
            {
                # checkout the file for edit, using the tf.exe tool, and
                # passing the CodePlex authn info on cmd line.
                & $tf edit $o.FullName $env:cplogin
            }
            Write-output $o.FullName
            $TmpFile = $o.FullName + ".tmp"

            get-content $o.FullName |
                %{$_ -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewVersion } |
                %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewFileVersion }  > $TmpFile

            move-item $TmpFile $o.FullName -force
        }
    }
}


function Update-AllAssemblyInfoFiles ( $version )
{
  foreach ($file in "SolutionInfo.cs", "AssemblyInfo.cs", "AssemblyInfo.vb" )
  {
       Write-output $file.FullName
  }

  foreach ($file in "SolutionInfo.cs", "AssemblyInfo.cs", "AssemblyInfo.vb" )
  {
    get-childitem -recurse |? {$_.Name -eq $file} | Update-SourceVersion $version ;
  }
}

#$tf = "c:\vs2008\common7\IDE\tf.exe"
$tf = "c:\vs2010\common7\IDE\tf.exe"

# validate arguments
$r= [System.Text.RegularExpressions.Regex]::Match($args[0], "^[0-9]+(\.[0-9]+){1,3}$");

if ($r.Success)
{
  Update-AllAssemblyInfoFiles $args[0];
}
else
{
  echo " ";
  echo "Bad Input!"
  echo " ";
  Usage ;
}
