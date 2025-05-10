# Auth-Service

## HACK: Gernerate good token

## Dev-stuff:
<code>
    https://localhost:5205/swagger/index.html

    GET:
    https://localhost:5205/AuthService/
    -> 200 if Service OK

    POST:
    https://localhost:5205/AuthService?username=test&password=test
    -> token (string)

    GET:
    https://localhost:5205/AuthService/check?username=test&token=dGVzdGFzZHNdfgfdghZA%3D%3D
    -> 200 if OK, 400 if not OK
</code>