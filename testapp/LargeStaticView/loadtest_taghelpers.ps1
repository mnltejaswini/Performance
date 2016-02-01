param([int] $iterations = 3000)

$url = "http://localhost:5000/TagHelpers"

& loadtest -k -n $iterations -c 16 --rps 250 $url