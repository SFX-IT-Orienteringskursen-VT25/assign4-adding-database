# assign3-addition-api

Work:
- Created a endpoint `/storage/:key`, user can provide a key as url param and values in body as a json payload

`http://localhost:3000/storage/enteredNumbers`

```
{
  "value":["10",23]
}
```

- `POST` method will create a new key if the key is not exisit, or append the provided values if the key exisit, converted to number. response will provide the values, sum of the values and appended values from the request.

- status code `201` used if created and `200` is successful 

```
{
    "key": "enteredNumbers",
    "values": [
        35,
        10,
        23
    ],
    "sum": 68,
    "appended": [
        10,
        23
    ]
}
```

- `GET` method will provide the values and sum of the values of the provided key
- status code `200` if key found and `404` used if key is not found

```
{
    "key": "enteredNumbers",
    "values": [
        35,
        10,
        23
    ],
    "sum": 68
}
```
```
{
    "error": "Not found",
    "key": "enteredNumbers_3"
}
```

- another `GET` endpoint `/storage` created which gives all the stored keys, and count of how many keys stored.

```
{
    "count": 2,
    "data": {
        "enteredNumbers": {
            "values": [
                35,
                10,
                23
            ],
            "sum": 68
        },
        "enteredNumbers_2": {
            "values": [
                1,
                2,
                4,
                10,
                -3
            ],
            "sum": 14
        }
    }
}
```

AI disclosure
- wrote unit test with the help of chatgpt 
