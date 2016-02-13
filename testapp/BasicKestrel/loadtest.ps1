param([int] $iterations = 1000000)

$url = "http://127.0.0.1:5000"

& loadtest -n $iterations -c 64  $url