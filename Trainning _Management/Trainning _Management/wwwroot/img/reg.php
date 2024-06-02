<?php
$c=mysqli_connect("localhost","root","rootroot");
mysqli_select_db($c,"batata");
$n=$_POST['name'];
$p=$_POST['pass'];
$a=$_POST['age'];
$errors=array();
if(trim(strlen($n))==0){
    $errors['name']= 'name reqierd';
}

if(trim(strlen($p))==0){
    $errors['pass']= 'password reqierd';
}

if(trim(strlen($a))==0){
    $errors['age']= 'age reqierd';
}

if(count($errors)>0){

?>
<form method='post' action='reg.php'>
<table>

<tr>
<td>username:</td>
<td><input type='text' name='name'></td>
<td style="color:red"><?php if(isset($errors['name'])) print $errors['name']?></td>
</tr>

<tr>
<td>password:</td>
<td><input type='password' name='pass'></td>
<td style="color:red"><?php if(isset($errors['pass'])) print $errors['pass']?></td>
</tr>
<tr>
<td>age:</td>
<td><input type='text' name='age'></td>
<td style="color:red"><?php if(isset($errors['age'])) print $errors['age']?></td>
</tr>
<tr>
<td><input type='submit' value='submit' ></td>
</tr>
</table>
</form>
<?php
}
else{
    mysqli_query($c,"insert into user values('','$n','$p','$a')");
}

?>

<form method="post" action="forml.php">
<input type="submit" value="login">
</form>