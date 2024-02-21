using System.IO;
using UnityEngine;


public class DirectoryUtil
{
    // �f�[�^�̍폜��ړ�������������


    // ��Ŏ������āACopyFolder() �̃t�@�C���R�s�[�̏��������̊֐��ɒu��������
    public static void CopyFile(string sourceFilePath, string destinationFolderPath, bool recursive = true)
    {

        //// �\�[�X�f�B���N�g���̏����擾
        //var dir = new DirectoryInfo(sourceFilePath);

        //// �\�[�X�f�B���N�gp�����{���ɑ��݂��邩�m�F
        //if (!dir.Exists)
        //    throw new DirectoryNotFoundException($"�R�s�[�������f�B���N�g����������Ȃ�����: {dir.FullName}");

        //// �R�s�[���J�n����O�Ƀf�B���N�g�����L���b�V������
        //DirectoryInfo[] dirs = dir.GetDirectories();

        //// �N���[���̊�Ƃ��Ă̋�f�B���N�g�����쐬
        //Directory.CreateDirectory(destinationFolderPath);

        //// �R�s�[���f�B���N�g���̃t�@�C�����擾���A�R�s�[��f�B���N�g���ɃR�s�[����B
        //foreach (FileInfo file in dir.GetFiles())
        //{
        //    string targetFilePath = Path.Combine(destinationFolderPath, file.Name);
        //    // �w�肵���p�X�̃t�H���_�����ɑ��݂���ꍇ�͖���
        //    if (File.Exists(targetFilePath))
        //    {
        //        Debug.LogWarning($"���L�̃t�@�C���͊��ɑ��݂���̂ŃR�s�[���Ȃ��ǂ��܂�\n{targetFilePath}");
        //        return;
        //    }
        //    // �R�s�[
        //    file.CopyTo(targetFilePath);
        //}

        //// �ċA�I�ɃT�u�f�B���N�g�����R�s�[����ꍇ�́A�ċA�I�ɂ��̃��\�b�h���Ăяo���B
        //if (recursive)
        //{
        //    foreach (DirectoryInfo subDir in dirs)
        //    {
        //        string newDestinationDir = Path.Combine(destinationFolderPath, subDir.Name);
        //        CopyFolder(subDir.FullName, newDestinationDir, true);
        //    }
        //}
    }


    public static void CopyFolder(string sourceFolderPath, string destinationFolderPath, bool recursive = true)
    {
        // �\�[�X�f�B���N�g���̏����擾
        var dir = new DirectoryInfo(sourceFolderPath);

        // �\�[�X�f�B���N�g�����{���ɑ��݂��邩�m�F
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"�R�s�[�������f�B���N�g����������Ȃ�����: {dir.FullName}");

        // �R�s�[���J�n����O�Ƀf�B���N�g�����L���b�V������
        DirectoryInfo[] dirs = dir.GetDirectories();

        // �N���[���̊�Ƃ��Ă̋�f�B���N�g�����쐬
        Directory.CreateDirectory(destinationFolderPath);

        // �R�s�[���f�B���N�g���̃t�@�C�����擾���A�R�s�[��f�B���N�g���ɃR�s�[����B
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationFolderPath, file.Name);
            // �w�肵���p�X�̃t�H���_�����ɑ��݂���ꍇ�͖���
            if (File.Exists(targetFilePath))
            {
                Debug.LogWarning($"���L�̃t�@�C���͊��ɑ��݂���̂ŃR�s�[���Ȃ��ǂ��܂�\n{targetFilePath}");
                return;
            }
            // �R�s�[
            file.CopyTo(targetFilePath);
        }

        // �ċA�I�ɃT�u�f�B���N�g�����R�s�[����ꍇ�́A�ċA�I�ɂ��̃��\�b�h���Ăяo���B
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationFolderPath, subDir.Name);
                CopyFolder(subDir.FullName, newDestinationDir, true);
            }
        }
    }


    // �w�肵���p�X�̂��̂������Ă���e�t�H���_���ۂ��ƃR�s�[
    public static void CopyParentFolder(string sourcePath, string destinationFolderPath, bool recursive = true)
    {
        // �p�X�̃t�@�C���i�܂��̓t�H���_�j���𔲂����
        string sourceName = Path.GetFileName(sourcePath);
        // �e�t�H���_�̃p�X�𔲂����
        string parentFolderPath = sourcePath.Replace(@"\" + sourceName, "");
        // �e�t�H���_�R�s�[
        CopyFolder(parentFolderPath, destinationFolderPath, recursive);
    }
}
