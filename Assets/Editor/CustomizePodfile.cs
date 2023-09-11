#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class CustomizePodfile
{

    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            foreach (string podFilePath in PodFilePaths)
            {
                CopyAndReplaceFile(podFilePath, Path.Combine(path, Path.GetFileName(podFilePath)));
            }

           
            StartPodsProcess (path); 
        }
    }
        static void CopyAndReplaceFile (string srcPath, string dstPath)
        {
            if (File.Exists (dstPath))
                File.Delete (dstPath);

            File.Copy (srcPath, dstPath);
        }

        static void StartPodsProcess (string path)
        {
            var proc = new System.Diagnostics.Process ();
            var fileName = Path.Combine (path, OpenPodsFileName);
            proc.StartInfo.FileName = fileName;
            proc.Start ();
        }

        static string UnityProjectRootFolder {
            get {
                return "Assets";
            }
        }

        static string XcodeFilesFolderPath {
            get {
                return Path.Combine (UnityProjectRootFolder, "XcodeFiles");
            }
        }

        static string PodFolderPath {
            get {
                return Path.Combine (XcodeFilesFolderPath, "Pods/");
            }
        }

        static string[] PodFilePaths {
            get {
                return new [] {
                    Path.Combine (PodFolderPath, "Podfile"),
                    Path.Combine (PodFolderPath, "pods.command"),
                    Path.Combine (PodFolderPath, OpenPodsFileName)
                };
            }
        }

        static string OpenPodsFileName {
            get {
                return "open_pods.command";
            }
        } 
}
#endif