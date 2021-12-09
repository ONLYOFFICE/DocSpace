const request = require("request");

const proxyURL = "";
const apiPrefixURL = "/api/2.0/";
const apiTimeout = 0; //30000

class RequestManager {
  getBasePath = (req) => "http://localhost:8092";

  makeRequest = (apiUrl, options, socket) => {
    //console.log("socket", socket.client.request.headers);

    return new Promise((resolve, reject) => {
      const basePath = this.getBasePath();
      const url = `${basePath}${apiPrefixURL}${apiUrl}`;
      options.uri = url;

      //console.log("makeRequest options", options);

      request(options, (error, response, body) => {
        if (error) {
          log.error(options.uri, error);
          if (error == 401 && req.session) {
            //req.session.destroy(() => reject(error));
            return reject(error);
          } else {
            return reject(error);
          }
        } else {
          resolve(body);
        }
      });
    });
  };
}

module.exports = new RequestManager();
