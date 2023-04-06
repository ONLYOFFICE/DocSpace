(function () {
  class BrowserDetector {
    constructor() {
      this.browser = {};
      this.unsupportedBrowsers = {
        Chrome: 102,
        Firefox: 102,
        IE: 11,
        Edge: 102,
        Opera: 90,
        Safari: 14,
        SafariMobile: 13,
        AscDesktopEditor: 6,
        SamsungBrowser: 4,
        UCBrowser: 12,
      };

      this.detectBrowser();
    }

    detectBrowser() {
      this.browser = (function () {
        const agent = navigator.userAgent;
        let temp = [];
        let match =
          agent.match(
            /(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i
          ) || [];

        if (/trident/i.test(match[1])) {
          temp = /\brv[ :]+(\d+)/g.exec(ua) || [];
          return { name: "IE", version: temp[1] || "" };
        }

        if (match[1] === "Chrome") {
          temp = agent.match(
            /\b(OPR|Edge|AscDesktopEditor|SamsungBrowser|UCBrowser)\/(\d+)/
          );
          if (temp != null) {
            return { name: temp[1].replace("OPR", "Opera"), version: temp[2] };
          }
        }

        match = match[2]
          ? [match[1], match[2]]
          : [navigator.appName, navigator.appVersion, "-?"];

        if ((temp = agent.match(/version\/(\d+)/i)) != null) {
          match.splice(1, 1, temp[1]);
        }

        if ((temp = agent.match(/mobile\/(\d+)/i)) != null) {
          match[0] += "Mobile";
        }

        if ((temp = agent.match(/mobile/i)) != null) {
          if ((temp = agent.match(/chrome\/?\s*(\d+)/i))) {
            match[1] = temp[1];
          }
        }

        return { name: match[0], version: match[1] };
      })();
    }

    isSupported() {
      if (this.unsupportedBrowsers.hasOwnProperty(this.browser.name)) {
        if (
          +this.browser.version < this.unsupportedBrowsers[this.browser.name]
        ) {
          return false;
        }
      }

      return true;
    }
  }

  const isSupported = new BrowserDetector().isSupported();

  if (!isSupported) {
    const styles = `<style>
    *, *::after, *::before {box-sizing: border-box;}
    html { font-size: 16px; }
    body { background: #F6F8F9; width: 100%; height: 100%; position: fixed; display: table; text-align: center; padding: 0px; margin: 0px; min-width: 320px; }
    .wrapper { display: table-cell; vertical-align: middle; height: 350px; width: 100%; max-width: 520px; text-align: center; padding: 10px;}
    h1 {font-weight: bold; font-size: 1.5rem; line-height: 2rem; padding: 0px; margin: 20px auto;}
    p {font-size: 1.125rem; line-height: 1.5rem; padding-bottom: 40px; color: #8797A1;}
    button {border: 1px solid #199CF3; border-radius: 4px; display: block; height: 50px; font-size: 1rem; padding: 15px; margin: 0 auto 50px; color: #199cf3; background-color: transparent; cursor: pointer;}
    span {display: block; font-size: 0.875rem; line-height: 1.5rem; text-align: center; color: #8797A1;}
    .old-list { list-style: none; display: block; margin: 0 auto; width: 100%; max-width: 640px; padding: 0; font-size: 0;}
    .old-item { display: inline-block; width: 25%; text-align: center;}
    .old-link { display: block; text-align: center; text-decoration: none; width: 100%; height: 100%; padding-top: 10px; padding-bottom: 10px; transition: 0.3s all ease; }
    .old-link:hover { background: rgba(25, 156, 243, 0.1); border-radius: 6px;}
    .old-title { margin-top: 20px; color: #199cf3; font-size: 1.125rem; }
    @media screen and (max-width: 600px) { .br-item { width: 50%; margin-bottom: 40px; } }
  </style>`;

    const body = `<div class="wrapper">
    <h1>Browser needs to be updated</h1>
    <p>You are using an outdated browser version</p>
    <ul class="old-list">
      <li class="old-item">
        <a href="https://www.google.com/chrome/" class="old-link">
          <img src="/static/images/browsers/chrome.svg" class="old-img" alt="Chrome">
          <span class="old-title">Chrome</span>
        </a>
      </li>
      <li class="old-item">
        <a href="https://www.mozilla.org/firefox/new/" class="old-link">
          <img src="/static/images/browsers/firefox.svg" class="old-img" alt="Firefox">
          <span class="old-title">Firefox</span>
        </a>
      </li>
      <li class="old-item">
        <a href="https://www.opera.com/" class="old-link">
          <img src="/static/images/browsers/opera.svg" class="old-img" alt="Opera">
          <span class="old-title">Opera</span>
        </a>
      </li>
      <li class="old-item">
        <a href="https://www.microsoft.com/edge/download" class="old-link">
          <img src="/static/images/browsers/edge.svg" class="old-img" alt="Edge">
          <span class="old-title">Edge</span>
        </a>
      </li>
    </ul>
  </div>`;

    document.head.innerHTML += styles;
    document.title = "You are using an outdated browser version";
    document.body.innerHTML = body;
  }
})();
