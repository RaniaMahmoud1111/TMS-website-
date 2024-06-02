<?php

   setcookie('product','camera', time() -3600);
?>
<html>
<body>

<?php
if(!isset($_COOKIE["product"])) {
  echo "Cookie named '" . $cookie_name . "' is not set!";
} else {
  echo "Cookie '" . $cookie_name . "' is set!<br>";
  echo "Value is: " . $_COOKIE["product"];
}
?>

</body>
</html>