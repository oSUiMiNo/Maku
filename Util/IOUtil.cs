using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class IOUtil
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


    //===========================================
    // ����t�H���_�̓��e���T�u�f�B���N�g���܂߂ĕʃt�H���_�ɃR�s�[
    //===========================================
    public static void CopyAll(string sourceDir, string destDir, string[] ignoreFileName = null, string[] ignoreDirName = null)
    {
        if (ignoreFileName == null) ignoreFileName = new string[0];
        if (ignoreDirName == null) ignoreDirName = new string[0];

        // �R�s�[��f�B���N�g����������΍쐬
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // �S�t�@�C�����R�s�[
        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(filePath);
            string destFilePath = $"{destDir}/{fileName}";

            // �����Ώۂ͖���
            if (ignoreFileName.Contains(fileName))
            {
                Debug.Log($"�����F{filePath}");
                continue;
            }
            // �R�s�[��ɓ����f�B���N�g������������x��(��������@�\�ǉ�����)
            if (Directory.Exists(destFilePath))
            {
                Debug.LogAssertion($"�t�@�C���Ɠ����̃f�B���N�g����������");
                continue;
            }
            // �R�s�[���s(�����t�@�C������������㏑��)
            File.Copy(filePath, destFilePath, true);
        }

        // �T�u�f�B���N�g�����ċA�I�ɃR�s�[
        foreach (string dirPath in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dirPath);
            string destSubDir = $"{destDir}/{dirName}";

            // �����Ώۂ͖���
            if (ignoreDirName.Contains(dirName))
            {
                Debug.Log($"�����F{dirPath}");
                continue;
            }
            // �ċA����
            CopyAll(dirPath, destSubDir, ignoreFileName, ignoreDirName);
        }
        Debug.Log($"�ړ������F{sourceDir}");
    }


    //===========================================
    // ����t�H���_�̓��e���T�u�f�B���N�g���܂߂ĕʃt�H���_�Ɉړ�
    //===========================================
    public static void MoveAll(string sourceDir, string destDir, string[] ignoreFileName = null, string[] ignoreDirName = null)
    {
        if (ignoreFileName == null) ignoreFileName = new string[0];
        if (ignoreDirName == null) ignoreDirName = new string[0];

        // �ړ���f�B���N�g����������΍쐬
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // �S�t�@�C�����ړ�
        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(filePath);
            string destFilePath = $"{destDir}/{fileName}";

            // �����Ώۂ͖���
            if (ignoreFileName.Contains(fileName))
            {
                Debug.Log($"�����F{filePath}");
                continue;
            }
            // �ړ���ɓ����f�B���N�g������������x��(��������@�\�ǉ�����)
            if (Directory.Exists(destFilePath))
            {
                Debug.LogAssertion($"�t�@�C���Ɠ����̃f�B���N�g����������");
                continue;
            }
            // �ړ���ɓ����t�@�C������������폜
            if (File.Exists(destFilePath)) File.Delete(destFilePath);
            // �ړ����s
            File.Move(filePath, destFilePath);
        }

        // �T�u�f�B���N�g�����ċA�I�Ɉړ�
        foreach (string dirPath in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dirPath);
            string destSubDir = $"{destDir}/{dirName}";

            // �����Ώۂ͖���
            if (ignoreDirName.Contains(dirName))
            {
                Debug.Log($"�����F{dirPath}");
                continue;
            }
            // �ċA����
            MoveAll(dirPath, destSubDir, ignoreFileName, ignoreDirName);
        }
        Debug.Log($"�ړ������F{sourceDir}");
    }
}
