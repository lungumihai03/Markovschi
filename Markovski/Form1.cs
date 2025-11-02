using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Markovski
{
    public class Form1 : Form
    {
        private int n;
        private int[,] multiplicationTable;
        private int[,] rightDivisionTable;
        private int[,] leftDivisionTable;
        private Dictionary<char, int> fMap;
        private Dictionary<int, char> inverseFMap;
        private TextBox txtMessage;
        private TextBox txtKey;
        private ComboBox cmbMode;
        private Button btnCalculate;
        private Button btnLoadMatrix;
        private Label lblMatrixStatus;
        private Label lblAlphabet;
        private bool matrixLoaded = false;

        public Form1()
        {
            SetupUI();
        }

        private void SetupUI()
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Criptare Markovski cu Quasigrup Hibrid";
            this.Size = new Size(600, 400);

            Label lblMessage = new Label { Text = "Mesaj:", Location = new Point(10, 10) };
            txtMessage = new TextBox { Location = new Point(110, 10), Width = 400 };
            txtMessage.TextChanged += TxtMessage_TextChanged;

            Label lblKey = new Label { Text = "Cheie k:", Location = new Point(10, 40) };
            txtKey = new TextBox { Location = new Point(110, 40), Width = 100 };

            Label lblMode = new Label { Text = "Mod:", Location = new Point(10, 70) };
            cmbMode = new ComboBox { Location = new Point(110, 70), Width = 250 };
            cmbMode.Items.Add("Dreapta");
            cmbMode.Items.Add("Stanga");
            cmbMode.Items.Add("Hibrid (prima jumătate dreapta, a doua stanga)");
            cmbMode.Items.Add("Hibrid (prima jumătate stanga, a doua dreapta)");
            cmbMode.Items.Add("Hibrid (alternare dreapta-stanga)");
            cmbMode.Items.Add("Hibrid (alternare stanga-dreapta)");
            cmbMode.SelectedIndex = 0;

            btnLoadMatrix = new Button { Text = "Incarca Matrice din Fisier", Location = new Point(10, 100), Width = 200 };
            btnLoadMatrix.Click += BtnLoadMatrix_Click;

            lblMatrixStatus = new Label { Text = "Matrice neincarcata. Va fi generata aleator daca nu incarcati.", Location = new Point(220, 100), Width = 300 };

            lblAlphabet = new Label { Text = "Alfabet: (se genereaza din mesaj)", Location = new Point(10, 130), Width = 500 };

            btnCalculate = new Button { Text = "Calculeaza", Location = new Point(10, 160), Width = 200 };
            btnCalculate.Click += BtnCalculate_Click;

            this.Controls.Add(lblMessage);
            this.Controls.Add(txtMessage);
            this.Controls.Add(lblKey);
            this.Controls.Add(txtKey);
            this.Controls.Add(lblMode);
            this.Controls.Add(cmbMode);
            this.Controls.Add(btnLoadMatrix);
            this.Controls.Add(lblMatrixStatus);
            this.Controls.Add(lblAlphabet);
            this.Controls.Add(btnCalculate);
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {
            GenerateAlphabet();
        }

        private void GenerateAlphabet()
        {
            string message = txtMessage.Text.ToUpper();
            HashSet<char> uniqueChars = new HashSet<char>();
            foreach (char c in message)
            {
                if (char.IsLetter(c))
                {
                    uniqueChars.Add(c);
                }
            }

            if (uniqueChars.Count == 0)
            {
                lblAlphabet.Text = "Alfabet: (nimic)";
                n = 0;
                return;
            }

            List<char> alphabet = uniqueChars.OrderBy(c => c).ToList();
            n = alphabet.Count;
            fMap = new Dictionary<char, int>();
            inverseFMap = new Dictionary<int, char>();
            for (int i = 0; i < n; i++)
            {
                fMap[alphabet[i]] = i + 1;
                inverseFMap[i + 1] = alphabet[i];
            }

            lblAlphabet.Text = "Alfabet: " + string.Join(", ", alphabet) + $" (n={n})";
            matrixLoaded = false;
            lblMatrixStatus.Text = "Matrice neincarcata. Va fi generata aleator daca nu incarcati.";
        }

        private void BtnLoadMatrix_Click(object sender, EventArgs e)
        {
            if (n == 0)
            {
                MessageBox.Show("Introduceti un mesaj valid pentru a genera alfabetul.");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Incarca Fisier cu Matricea Quasigrup"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length != n)
                        throw new Exception($"Fisierul trebuie sa aiba exact {n} linii.");

                    multiplicationTable = new int[n, n];

                    for (int i = 0; i < n; i++)
                    {
                        string[] nums = lines[i].Trim().Split(' ');
                        if (nums.Length != n)
                            throw new Exception($"Fiecare linie trebuie sa aiba exact {n} numere.");

                        for (int j = 0; j < n; j++)
                        {
                            if (!int.TryParse(nums[j], out int val) || val < 1 || val > n)
                                throw new Exception($"Numerele trebuie sa fie intre 1 si {n}.");
                            multiplicationTable[i, j] = val;
                        }
                    }

                    if (!IsLatinSquare(multiplicationTable))
                        throw new Exception("Matricea nu este un Latin square valid (quasigrup).");

                    CalculateDivisionTables();
                    matrixLoaded = true;
                    lblMatrixStatus.Text = "Matrice incarcata cu succes.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcare: " + ex.Message);
                }
            }
        }

        private bool IsLatinSquare(int[,] table)
        {
            if (table == null) return false;
            for (int i = 0; i < n; i++)
            {
                HashSet<int> row = new HashSet<int>();
                HashSet<int> col = new HashSet<int>();
                for (int j = 0; j < n; j++)
                {
                    int rowVal = table[i, j];
                    int colVal = table[j, i];
                    if (!row.Add(rowVal) || !col.Add(colVal))
                        return false;
                }
            }
            return true;
        }

        private void GenerateRandomMatrix()
        {
            if (n == 0)
            {
                MessageBox.Show("Introduceti un mesaj valid pentru a genera alfabetul.");
                return;
            }

            multiplicationTable = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    multiplicationTable[i, j] = ((i + j) % n) + 1;
                }
            }

            CalculateDivisionTables();
            matrixLoaded = true;
            lblMatrixStatus.Text = "Matrice generata aleator.";
        }

        private void CalculateDivisionTables()
        {
            rightDivisionTable = new int[n, n];
            leftDivisionTable = new int[n, n];

            for (int b = 1; b <= n; b++)
            {
                for (int a = 1; a <= n; a++)
                {
                    for (int y = 1; y <= n; y++)
                    {
                        if (Multiply(y, a) == b)
                        {
                            rightDivisionTable[b - 1, a - 1] = y;
                            break;
                        }
                    }
                }
            }

            for (int a = 1; a <= n; a++)
            {
                for (int b = 1; b <= n; b++)
                {
                    for (int y = 1; y <= n; y++)
                    {
                        if (Multiply(a, y) == b)
                        {
                            leftDivisionTable[a - 1, b - 1] = y;
                            break;
                        }
                    }
                }
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            PerformOperation();
        }

        private void PerformOperation()
        {
            if (n == 0)
            {
                MessageBox.Show("Introduceti un mesaj valid cu litere.");
                return;
            }

            if (!matrixLoaded)
            {
                GenerateRandomMatrix();
            }

            string message = txtMessage.Text.ToUpper();
            if (!int.TryParse(txtKey.Text, out int k) || k < 1 || k > n)
            {
                MessageBox.Show($"Cheia k trebuie sa fie intre 1 si {n}.");
                return;
            }

            string mode = cmbMode.SelectedItem.ToString();
            bool isRightOnly = mode == "Dreapta";
            bool isLeftOnly = mode == "Stanga";
            bool isHybridHalfRightLeft = mode == "Hibrid (prima jumătate dreapta, a doua stanga)";
            bool isHybridHalfLeftRight = mode == "Hibrid (prima jumătate stanga, a doua dreapta)";
            bool isHybridAlternateRightLeft = mode == "Hibrid (alternare dreapta-stanga)";
            bool isHybridAlternateLeftRight = mode == "Hibrid (alternare stanga-dreapta)";

            string modeType = mode;

            List<int> sequence = new List<int>();
            foreach (char c in message)
            {
                if (fMap.ContainsKey(c))
                {
                    sequence.Add(fMap[c]);
                }
            }

            if (sequence.Count == 0)
            {
                MessageBox.Show("Mesajul nu contine litere valide.");
                return;
            }

            (List<int> encryptedSequence, string encryptSteps) = EncryptHybrid(sequence, k, isRightOnly, isLeftOnly, isHybridHalfRightLeft, isHybridHalfLeftRight, isHybridAlternateRightLeft, isHybridAlternateLeftRight);
            (List<int> decryptedSequence, string decryptSteps) = DecryptHybrid(encryptedSequence, k, isRightOnly, isLeftOnly, isHybridHalfRightLeft, isHybridHalfLeftRight, isHybridAlternateRightLeft, isHybridAlternateLeftRight);

            string encryptedResult = string.Join("", encryptedSequence.Select(num => inverseFMap[num]));
            string decryptedResult = string.Join("", decryptedSequence.Select(num => inverseFMap[num]));

            string fileContent = $"Operatie: Criptare si Decriptare in mod {modeType}\r\n";
            fileContent += $"Mesaj initial: {message}\r\n";
            fileContent += $"Cheie k: {k}\r\n";
            fileContent += $"Alfabet: {string.Join(", ", fMap.Keys)}\r\n";
            fileContent += $"Dimensiune n: {n}\r\n";
            fileContent += "Matricea Quasigrup de baza (.):\r\n";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    fileContent += multiplicationTable[i, j] + " ";
                }
                fileContent += "\r\n";
            }
            fileContent += "\r\nMatricea de Diviziune Dreapta (/):\r\n";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    fileContent += rightDivisionTable[i, j] + " ";
                }
                fileContent += "\r\n";
            }
            fileContent += "\r\nMatricea de Diviziune Stanga (\\):\r\n";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    fileContent += leftDivisionTable[i, j] + " ";
                }
                fileContent += "\r\n";
            }
            fileContent += "\r\nPasi Criptare:\r\n" + encryptSteps;
            fileContent += $"\r\nRezultat Criptare: {encryptedResult}\r\n";
            fileContent += "\r\nPasi Decriptare:\r\n" + decryptSteps;
            fileContent += $"\r\nRezultat Decriptare: {decryptedResult}\r\n";

            string filePath = "results.txt";
            File.WriteAllText(filePath, fileContent);
            MessageBox.Show($"Rezultatele au fost salvate in {filePath}.");
            System.Diagnostics.Process.Start("notepad.exe", filePath);
        }

        private (List<int>, string) EncryptHybrid(List<int> r, int k, bool isRightOnly, bool isLeftOnly, bool isHybridHalfRightLeft, bool isHybridHalfLeftRight, bool isHybridAlternateRightLeft, bool isHybridAlternateLeftRight)
        {
            List<int> b = new List<int>();
            string steps = $"Criptare in mod selectat:\r\n";

            int prev = k;
            int m = r.Count;
            for (int i = 0; i < m; i++)
            {
                bool useRight = DetermineIfRight(i, m, isRightOnly, isLeftOnly, isHybridHalfRightLeft, isHybridHalfLeftRight, isHybridAlternateRightLeft, isHybridAlternateLeftRight);
                int bi;
                string op;
                if (useRight)
                {
                    bi = RightDivide(r[i], prev);
                    op = "/ (dreapta)";
                }
                else
                {
                    bi = LeftDivide(prev, r[i]);
                    op = "\\ (stanga)";
                }
                b.Add(bi);
                steps += $"b{i + 1} = {(useRight ? $"{r[i]} {op} {prev}" : $"{prev} {op} {r[i]}")} = {bi}\r\n";
                prev = bi;
            }

            return (b, steps);
        }

        private (List<int>, string) DecryptHybrid(List<int> b, int k, bool isRightOnly, bool isLeftOnly, bool isHybridHalfRightLeft, bool isHybridHalfLeftRight, bool isHybridAlternateRightLeft, bool isHybridAlternateLeftRight)
        {
            List<int> r = new List<int>();
            string steps = $"Decriptare in mod selectat:\r\n";

            int prev = k;
            int m = b.Count;
            for (int i = 0; i < m; i++)
            {
                bool useRight = DetermineIfRight(i, m, isRightOnly, isLeftOnly, isHybridHalfRightLeft, isHybridHalfLeftRight, isHybridAlternateRightLeft, isHybridAlternateLeftRight);
                int ri;
                string op;
                if (useRight)
                {
                    ri = Multiply(b[i], prev);
                    op = "* (dreapta)";
                }
                else
                {
                    ri = Multiply(prev, b[i]);
                    op = "* (stanga)";
                }
                r.Add(ri);
                steps += $"r{i + 1} = {(useRight ? $"{b[i]} {op} {prev}" : $"{prev} {op} {b[i]}")} = {ri}\r\n";
                prev = b[i];
            }

            return (r, steps);
        }

        private bool DetermineIfRight(int index, int length, bool isRightOnly, bool isLeftOnly, bool isHybridHalfRightLeft, bool isHybridHalfLeftRight, bool isHybridAlternateRightLeft, bool isHybridAlternateLeftRight)
        {
            if (isRightOnly) return true;
            if (isLeftOnly) return false;
            if (isHybridHalfRightLeft)
            {
                return index < length / 2;
            }
            if (isHybridHalfLeftRight)
            {
                return index >= length / 2;
            }
            if (isHybridAlternateRightLeft)
            {
                return index % 2 == 0;
            }
            if (isHybridAlternateLeftRight)
            {
                return index % 2 == 1;
            }
            return true;
        }

        private int Multiply(int x, int y)
        {
            return multiplicationTable[x - 1, y - 1];
        }

        private int RightDivide(int b, int a)
        {
            return rightDivisionTable[b - 1, a - 1];
        }

        private int LeftDivide(int a, int b)
        {
            return leftDivisionTable[a - 1, b - 1];
        }
    }
}