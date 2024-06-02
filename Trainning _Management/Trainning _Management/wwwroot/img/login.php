<?php
$c=mysqli_connect("localhost","root","rootroot");
mysqli_select_db($c,"batata");

$n=$_POST['name'];
$p=$_POST['pass'];

   $q=mysqli_query($c,"select * from user  ");

   while($row=mysqli_fetch_assoc($q)){

    if($row['name']==$n && $row['pass']==$p){
$nn= "welcome to this website <br>
<form method='post' action='view.php'>
<input type='submit' value='go to page'>
<form>";
break;
    }else{
$nn="not valid username or password";

    }
   }
   echo $nn;
    


?>