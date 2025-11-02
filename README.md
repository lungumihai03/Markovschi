Markovski Hybrid Quasigroup Cipher
==================================

Overview
--------

**Criptare Markovski cu Quasigrup Hibrid** is a Windows Forms application developed in C# that implements a **non-standard stream cipher** based on **quasigroup operations** (Latin squares) with **hybrid left/right division modes**. It uses a user-defined or randomly generated quasigroup (multiplication table) and a numeric key k to encrypt and decrypt text messages over a dynamically generated alphabet.

The cipher combines **left and right division** in a quasigroup to create a reversible transformation, supporting multiple **hybrid modes** (right-only, left-only, alternating, half-and-half). Results, including full step-by-step computation, are saved to results.txt.

> 

* * *

Features
--------

* **Dynamic Alphabet**: Automatically extracts unique letters from the input message and maps them to indices 1..n.
* **Quasigroup Support**:
  * Load a custom n×n Latin square from a .txt file.
  * Or generate a default cyclic quasigroup if none is loaded.
* **Precomputed Division Tables**:
  * Right division / → solves y · a = b
  * Left division \ → solves a · y = b
* **Multiple Operation Modes**:
  1. **Right-only** (/ throughout)
  2. **Left-only** (\ throughout)
  3. **Hybrid (first half right, second left)**
  4. **Hybrid (first half left, second right)**
  5. **Hybrid (alternate right-left)**
  6. **Hybrid (alternate left-right)**
* **Full Traceability**: Every encryption/decryption step is logged with equations.
* **File Output**: Complete results (tables, steps, plaintext/ciphertext) saved to results.txt and opened in Notepad.

* * *

How It Works
------------

### 1. **Alphabet & Mapping**

* Input message → extract unique uppercase letters → sort → assign indices 1..n.
* fMap: char → int, inverseFMap: int → char

### 2. **Quasigroup (·)**

* A Latin square defining a binary operation: a · b = table[a-1, b-1]
* Must contain each number 1..n exactly once per row and column.

### 3. **Division Operations**

* **Right division** /: Find y such that y · a = b → b / a = y
* **Left division** \: Find y such that a · y = b → a \ b = y

### 4. **Encryption (Hybrid Stream)**

text

`   b₀ = k  bᵢ = (rᵢ / prev) or (prev \ rᵢ) depending on mode  prev = bᵢ       `

* rᵢ = plaintext symbol index
* Output: ciphertext sequence b

### 5. **Decryption**

text

`   rᵢ = (bᵢ * prev) or (prev * bᵢ) depending on mode  prev = bᵢ       `

* Reverses the operation using multiplication.

* * *

Usage
-----

1. **Enter Message**:
   * Type or paste text in **"Mesaj"**.
   * Alphabet auto-generates from unique letters.
2. **Set Key**:
   * Enter k (1 to n) in **"Cheie k"**.
3. **Choose Mode**:
   * Select from 6 hybrid modes in the dropdown.
4. **Load Matrix (Optional)**:
   * Click **"Incarca Matrice din Fisier"**.
   * Provide an n×n text file with space-separated numbers (1 to n), one row per line.
5. **Encrypt & Decrypt**:
   * Click **"Calculeaza"**.
   * Full process (encryption + decryption) runs.
   * Results saved to results.txt and opened automatically.
