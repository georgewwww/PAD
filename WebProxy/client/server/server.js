var express = require('express');
var app = express();


app.use('/public', express.static(__dirname + './../public'));

// parse incoming body
var bodyParser = require('body-parser');
app.use(bodyParser.json());

// routes
var routes = require('./routes');
routes.routes(app);

app.listen(4000, function () {
    console.log('Start the client server on port: 4000');
});
