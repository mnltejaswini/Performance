param([int] $iterations = 3000)

# & needs escaping for CMD
$url = "http://127.0.0.1:5000/TagHelpers/Index/?name=Joey^^^&age=15^^^&birthdate=9-9-1985"

& loadtest -k -n $iterations -c 16 --rps 100 $url