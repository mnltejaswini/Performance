param([int] $iterations = 1000000)

$url = "http://127.0.0.1:5000"

& loadtest -k -n $iterations -c 128  $url