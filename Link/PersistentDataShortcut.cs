#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;



public static class PersistentDataShortcut_WIN
{
    const string LinkPath = "Assets/Link_Persistent";

    [MenuItem("Maku/Create Shortcut (Windows)/PersistentDataPath �� Assets", priority = 1000)]
    public static void CreatePersistentDataShortcut()
    {
#if UNITY_EDITOR_WIN
        UniTask.Void(async () =>
        {
            string targ = Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
            string link = Path.GetFullPath(LinkPath);

            // persistentDataPath �����쐬�ł������Ȃ��悤�Ɋm��
            try { Directory.CreateDirectory(targ); } catch { }

            // ���������N/�t�H���_������
            try
            {
                // PowerShell�ł̈��S�폜�i�����N/���̂�����Ēǂ�Ȃ��j
                string rm = $"if (Test-Path -LiteralPath '{link}') {{ Remove-Item -LiteralPath '{link}' -Force }}";
                await PowerShellAPI.Command(rm);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"�����̃����N�폜�Ɏ��s: {e.Message}");
            }

            bool ok = false;
            string madeType = "?";

            // �܂� SymbolicLink �������i�J���҃��[�h or �Ǘ��҂Ő������₷���j
            try
            {
                string cmdSym = $"New-Item -ItemType SymbolicLink -Path '{link}' -Target '{targ}'";
                await PowerShellAPI.Command(cmdSym);
                ok = true;
                madeType = "SymbolicLink";
            }
            catch
            {
                // �_���Ȃ� Junction �Ƀt�H�[���o�b�N�i�Ǘ��ҕs�v�Œʂ邱�Ƃ������j
                try
                {
                    string cmdJunc = $"New-Item -ItemType Junction -Path '{link}' -Target '{targ}'";
                    await PowerShellAPI.Command(cmdJunc);
                    ok = true;
                    madeType = "Junction";
                }
                catch (System.Exception e2)
                {
                    Debug.LogError(
                        "�����N�쐬�Ɏ��s���܂����iSymbolicLink / Junction �Ƃ��Ɏ��s�j�B\n" +
                        $"�^�[�Q�b�g: {targ}\n�����N: {link}\n" +
                        $"�ڍ�: {e2.Message}\n" +
                        "�� SymbolicLink �͊J���҃��[�h or �Ǘ��Ҍ������K�v�ȏꍇ������܂��B"
                    );
                }
            }

            AssetDatabase.Refresh();

            if (ok)
            {
                Debug.Log($"�쐬����: [{madeType}] {LinkPath} �� {targ}");
            }
        });
#else
        Debug.LogError("���̃R�}���h�� Windows ��p�ł��B");
#endif
    }
}
#endif
