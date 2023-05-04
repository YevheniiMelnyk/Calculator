<h1>Calculator Application using Reverse Polish Notation</h1>
<p>This application is designed to perform mathematical operations in two modes - console and file mode. 
The operations are executed using Reverse Polish Notation (RPN) without the use of third party libraries or components.
</p>

<h3>Console Mode</h3>
<p>
In this mode, users can perform simple operations without brackets in a console application. 
Operations are executed based on math priority - * and / have higher priority than + and -.
</p>
<h4>Examples</h4>
Input: 2+2*3 </br>
Output: 8 </br>

Input: 2/0 </br>
Output: Exception. Divide by zero.

<h3>File Mode</h3>
<p>In this mode, the application reads data from a file line by line and performs calculations with brackets. 
Each line is calculated and the result is written to a different file.</p>

<h4>Example</h4>
Input file:</br>

1+2*(3+2)</br>
1+x+4</br>
2+15/3+4*2</br>

Output file:</br>
1+2*(3+2) = 11</br>
1+x+4 = Exception. Wrong input.</br>
2+15/3+4*2 = 15</br>

<h5>Notes</h5>
<i>The application implements parsing, calculation, and math operation priorities without using third party libraries or components.</i>