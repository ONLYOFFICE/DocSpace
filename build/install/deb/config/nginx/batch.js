function batchJSON(request) {
	batch(request, { serialize: JSON.stringify, contentType: "application/json; charset=utf-8"});
}

function batchXML(request) {
	batch(request, { serialize: OBJtoXML, contentType: "application/xml; charset=utf-8"});
}

function batch(request, serialiazer) {
    var body = JSON.parse(request.requestBody).batch;
    var n = 0;
    var result = {
        count: body.length,
        status: 0,
        statusCode: 200,
        response: []
    };

    function done(i, reply) {
        result.response[i] = {
            status: reply.status,
            data: reply.responseBody
        };

        if (++n == body.length) {
            var response = serialiazer.serialize(result);
            request.status = 200;
            request.headersOut['Content-Type'] = serialiazer.contentType;
            request.headersOut['Content-Length'] = response.length;
            request.sendHeader();
            request.send(response);
            request.finish();
        }
    }


    for(var i in body) {
        request.subrequest(body[i].RelativeUrl, { method : body[i].RelativeMethod }, done.bind(null, i));
    }
}

function OBJtoXML(obj) {
  var xml = '';
  for (var prop in obj) {
    xml += obj[prop] instanceof Array ? '' : "<" + prop + ">";
    if (obj[prop] instanceof Array) {
      for (var array in obj[prop]) {
        xml += "<" + prop + ">";
        xml += OBJtoXML(new Object(obj[prop][array]));
        xml += "</" + prop + ">";
      }
    } else if (typeof obj[prop] == "object") {
      xml += OBJtoXML(new Object(obj[prop]));
    } else {
      xml += obj[prop];
    }
    xml += obj[prop] instanceof Array ? '' : "</" + prop + ">";
  }
  var xml = xml.replace(/<\/?[0-9]{1,}>/g, '');
  return xml
}