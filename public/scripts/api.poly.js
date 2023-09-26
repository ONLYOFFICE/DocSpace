"use strict";

function _typeof(o) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (o) { return typeof o; } : function (o) { return o && "function" == typeof Symbol && o.constructor === Symbol && o !== Symbol.prototype ? "symbol" : typeof o; }, _typeof(o); }
function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }
function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, _toPropertyKey(descriptor.key), descriptor); } }
function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); Object.defineProperty(Constructor, "prototype", { writable: false }); return Constructor; }
function _classPrivateFieldInitSpec(obj, privateMap, value) { _checkPrivateRedeclaration(obj, privateMap); privateMap.set(obj, value); }
function _checkPrivateRedeclaration(obj, privateCollection) { if (privateCollection.has(obj)) { throw new TypeError("Cannot initialize the same private elements twice on an object"); } }
function _classPrivateFieldSet(receiver, privateMap, value) { var descriptor = _classExtractFieldDescriptor(receiver, privateMap, "set"); _classApplyDescriptorSet(receiver, descriptor, value); return value; }
function _classApplyDescriptorSet(receiver, descriptor, value) { if (descriptor.set) { descriptor.set.call(receiver, value); } else { if (!descriptor.writable) { throw new TypeError("attempted to set read only private field"); } descriptor.value = value; } }
function _classPrivateFieldGet(receiver, privateMap) { var descriptor = _classExtractFieldDescriptor(receiver, privateMap, "get"); return _classApplyDescriptorGet(receiver, descriptor); }
function _classExtractFieldDescriptor(receiver, privateMap, action) { if (!privateMap.has(receiver)) { throw new TypeError("attempted to " + action + " private field on non-instance"); } return privateMap.get(receiver); }
function _classApplyDescriptorGet(receiver, descriptor) { if (descriptor.get) { return descriptor.get.call(receiver); } return descriptor.value; }
function ownKeys(e, r) { var t = Object.keys(e); if (Object.getOwnPropertySymbols) { var o = Object.getOwnPropertySymbols(e); r && (o = o.filter(function (r) { return Object.getOwnPropertyDescriptor(e, r).enumerable; })), t.push.apply(t, o); } return t; }
function _objectSpread(e) { for (var r = 1; r < arguments.length; r++) { var t = null != arguments[r] ? arguments[r] : {}; r % 2 ? ownKeys(Object(t), !0).forEach(function (r) { _defineProperty(e, r, t[r]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(e, Object.getOwnPropertyDescriptors(t)) : ownKeys(Object(t)).forEach(function (r) { Object.defineProperty(e, r, Object.getOwnPropertyDescriptor(t, r)); }); } return e; }
function _defineProperty(obj, key, value) { key = _toPropertyKey(key); if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }
function _toPropertyKey(arg) { var key = _toPrimitive(arg, "string"); return _typeof(key) === "symbol" ? key : String(key); }
function _toPrimitive(input, hint) { if (_typeof(input) !== "object" || input === null) return input; var prim = input[Symbol.toPrimitive]; if (prim !== undefined) { var res = prim.call(input, hint || "default"); if (_typeof(res) !== "object") return res; throw new TypeError("@@toPrimitive must return a primitive value."); } return (hint === "string" ? String : Number)(input); }
(function () {
  var defaultConfig = {
    src: new URL(document.currentScript.src).origin,
    rootPath: "/rooms/shared/",
    width: "100%",
    height: "100%",
    name: "frameDocSpace",
    type: "desktop",
    // TODO: ["desktop", "mobile"]
    frameId: "ds-frame",
    mode: "manager",
    //TODO: ["manager", "editor", "viewer","room-selector", "file-selector", "system"]
    id: null,
    locale: "en-US",
    theme: "Base",
    editorType: "embedded",
    //TODO: ["desktop", "embedded"]
    editorGoBack: true,
    selectorType: "exceptPrivacyTrashArchiveFolders",
    //TODO: ["roomsOnly", "userFolderOnly", "exceptPrivacyTrashArchiveFolders", "exceptSortedByTagsFolders"]
    showHeader: false,
    showTitle: true,
    showMenu: false,
    showFilter: false,
    destroyText: "",
    viewAs: "row",
    //TODO: ["row", "table", "tile"]
    filter: {
      count: 100,
      page: 1,
      sortorder: "descending",
      //TODO: ["descending", "ascending"]
      sortby: "DateAndTime",
      //TODO: ["DateAndTime", "AZ", "Type", "Size", "DateAndTimeCreation", "Author"]
      search: "",
      withSubfolders: true
    },
    keysForReload: ["src", "rootPath", "width", "height", "name", "frameId", "id", "type", "editorType", "mode"],
    events: {
      onSelectCallback: null,
      onCloseCallback: null,
      onAppReady: null,
      onAppError: null,
      onEditorCloseCallback: null,
      onAuthSuccess: null
    }
  };
  var getConfigFromParams = function getConfigFromParams() {
    var src = document.currentScript.src;
    if (!src || !src.length) return null;
    var searchUrl = src.split("?")[1];
    var object = {};
    if (searchUrl && searchUrl.length) {
      object = JSON.parse("{\"".concat(searchUrl.replace(/&/g, '","').replace(/=/g, '":"'), "\"}"), function (k, v) {
        return v === "true" ? true : v === "false" ? false : v;
      });
      object.filter = defaultConfig.filter;
      for (prop in object) {
        if (prop in defaultConfig.filter) {
          object.filter[prop] = object[prop];
          delete object[prop];
        }
      }
    }
    return _objectSpread(_objectSpread({}, defaultConfig), object);
  };
  var _iframe = /*#__PURE__*/new WeakMap();
  var _isConnected = /*#__PURE__*/new WeakMap();
  var _callbacks = /*#__PURE__*/new WeakMap();
  var _tasks = /*#__PURE__*/new WeakMap();
  var _classNames = /*#__PURE__*/new WeakMap();
  var _oneOfExistInObject = /*#__PURE__*/new WeakMap();
  var _createIframe = /*#__PURE__*/new WeakMap();
  var _sendMessage = /*#__PURE__*/new WeakMap();
  var _onMessage = /*#__PURE__*/new WeakMap();
  var _executeMethod = /*#__PURE__*/new WeakMap();
  var _getMethodPromise = /*#__PURE__*/new WeakMap();
  var DocSpace = /*#__PURE__*/function () {
    function DocSpace(_config) {
      var _this = this;
      _classCallCheck(this, DocSpace);
      _classPrivateFieldInitSpec(this, _iframe, {
        writable: true,
        value: void 0
      });
      _classPrivateFieldInitSpec(this, _isConnected, {
        writable: true,
        value: false
      });
      _classPrivateFieldInitSpec(this, _callbacks, {
        writable: true,
        value: []
      });
      _classPrivateFieldInitSpec(this, _tasks, {
        writable: true,
        value: []
      });
      _classPrivateFieldInitSpec(this, _classNames, {
        writable: true,
        value: ""
      });
      _classPrivateFieldInitSpec(this, _oneOfExistInObject, {
        writable: true,
        value: function value(array, object) {
          return Object.keys(object).some(function (k) {
            return array.includes(k);
          });
        }
      });
      _classPrivateFieldInitSpec(this, _createIframe, {
        writable: true,
        value: function value(config) {
          var iframe = document.createElement("iframe");
          var path = "";
          switch (config.mode) {
            case "manager":
              {
                if (config.filter) {
                  if (config.id) config.filter.folder = config.id;
                  var filterString = new URLSearchParams(config.filter).toString();
                  path = "".concat(config.rootPath).concat(config.id ? config.id + "/" : "", "filter?").concat(filterString);
                }
                break;
              }
            case "room-selector":
              {
                path = "/sdk/room-selector";
                break;
              }
            case "file-selector":
              {
                path = "/sdk/file-selector?selectorType=".concat(config.selectorType);
                break;
              }
            case "system":
              {
                path = "/sdk/system";
                break;
              }
            case "editor":
              {
                var goBack = config.editorGoBack;
                if (config.events.onEditorCloseCallback && typeof config.events.onEditorCloseCallback === "function") {
                  goBack = "event";
                }
                path = "/doceditor/?fileId=".concat(config.id, "&type=").concat(config.editorType, "&editorGoBack=").concat(goBack);
                break;
              }
            case "viewer":
              {
                var _goBack = config.editorGoBack;
                if (config.events.onEditorCloseCallback && typeof config.events.onEditorCloseCallback === "function") {
                  _goBack = "event";
                }
                path = "/doceditor/?fileId=".concat(config.id, "&type=").concat(config.editorType, "&action=view&editorGoBack=").concat(_goBack);
                break;
              }
            default:
              path = config.rootPath;
          }
          iframe.src = config.src + path;
          iframe.width = config.width;
          iframe.height = config.height;
          iframe.name = config.name;
          iframe.id = config.frameId;
          iframe.align = "top";
          iframe.frameBorder = 0;
          iframe.allowFullscreen = true;
          iframe.setAttribute("allowfullscreen", "");
          iframe.setAttribute("onmousewheel", "");
          iframe.setAttribute("allow", "autoplay");
          if (config.type == "mobile") {
            iframe.style.position = "fixed";
            iframe.style.overflow = "hidden";
            document.body.style.overscrollBehaviorY = "contain";
          }
          return iframe;
        }
      });
      _classPrivateFieldInitSpec(this, _sendMessage, {
        writable: true,
        value: function value(message) {
          var mes = {
            frameId: _this.config.frameId,
            type: "",
            data: message
          };
          if (!!_classPrivateFieldGet(_this, _iframe).contentWindow) {
            _classPrivateFieldGet(_this, _iframe).contentWindow.postMessage(JSON.stringify(mes, function (key, value) {
              return typeof value === "function" ? value.toString() : value;
            }), _this.config.src);
          }
        }
      });
      _classPrivateFieldInitSpec(this, _onMessage, {
        writable: true,
        value: function value(e) {
          if (typeof e.data == "string") {
            var data = {};
            try {
              data = JSON.parse(e.data);
            } catch (err) {
              data = {};
            }
            switch (data.type) {
              case "onMethodReturn":
                {
                  if (_classPrivateFieldGet(_this, _callbacks).length > 0) {
                    var callback = _classPrivateFieldGet(_this, _callbacks).shift();
                    callback && callback(data.methodReturnData);
                  }
                  if (_classPrivateFieldGet(_this, _tasks).length > 0) {
                    _classPrivateFieldGet(_this, _sendMessage).call(_this, _classPrivateFieldGet(_this, _tasks).shift());
                  }
                  break;
                }
              case "onEventReturn":
                {
                  var _data, _data2;
                  if (((_data = data) === null || _data === void 0 || (_data = _data.eventReturnData) === null || _data === void 0 ? void 0 : _data.event) in _this.config.events && typeof _this.config.events[(_data2 = data) === null || _data2 === void 0 ? void 0 : _data2.eventReturnData.event] === "function") {
                    var _data3, _data4;
                    _this.config.events[(_data3 = data) === null || _data3 === void 0 ? void 0 : _data3.eventReturnData.event]((_data4 = data) === null || _data4 === void 0 || (_data4 = _data4.eventReturnData) === null || _data4 === void 0 ? void 0 : _data4.data);
                  }
                  break;
                }
              case "onCallCommand":
                {
                  _this[data.commandName].call(_this, data.commandData);
                  break;
                }
              default:
                break;
            }
          }
        }
      });
      _classPrivateFieldInitSpec(this, _executeMethod, {
        writable: true,
        value: function value(methodName, params, callback) {
          if (!_classPrivateFieldGet(_this, _isConnected)) {
            console.log("Message bus is not connected with frame");
            return;
          }
          _classPrivateFieldGet(_this, _callbacks).push(callback);
          var message = {
            type: "method",
            methodName: methodName,
            data: params
          };
          if (_classPrivateFieldGet(_this, _callbacks).length !== 1) {
            _classPrivateFieldGet(_this, _tasks).push(message);
            return;
          }
          _classPrivateFieldGet(_this, _sendMessage).call(_this, message);
        }
      });
      _classPrivateFieldInitSpec(this, _getMethodPromise, {
        writable: true,
        value: function value(methodName) {
          var params = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : null;
          var withReload = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : false;
          return new Promise(function (resolve) {
            if (withReload) {
              _this.initFrame(_this.config);
              resolve(_this.config);
            } else {
              _classPrivateFieldGet(_this, _executeMethod).call(_this, methodName, params, function (data) {
                return resolve(data);
              });
            }
          });
        }
      });
      this.config = _config;
    }
    _createClass(DocSpace, [{
      key: "initFrame",
      value: function initFrame(config) {
        var configFull = _objectSpread(_objectSpread({}, defaultConfig), config);
        this.config = _objectSpread(_objectSpread({}, this.config), configFull);
        var target = document.getElementById(this.config.frameId);
        if (target) {
          _classPrivateFieldSet(this, _iframe, _classPrivateFieldGet(this, _createIframe).call(this, this.config));
          _classPrivateFieldSet(this, _classNames, target.className);
          target.parentNode && target.parentNode.replaceChild(_classPrivateFieldGet(this, _iframe), target);
          window.addEventListener("message", _classPrivateFieldGet(this, _onMessage), false);
          _classPrivateFieldSet(this, _isConnected, true);
        }
        window.DocSpace.SDK.frames = window.DocSpace.SDK.frames || [];
        window.DocSpace.SDK.frames[this.config.frameId] = this;
        return _classPrivateFieldGet(this, _iframe);
      }
    }, {
      key: "initManager",
      value: function initManager() {
        var config = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        config.mode = "manager";
        return this.initFrame(config);
      }
    }, {
      key: "initEditor",
      value: function initEditor() {
        var config = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        config.mode = "editor";
        return this.initFrame(config);
      }
    }, {
      key: "initViewer",
      value: function initViewer() {
        var config = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        config.mode = "viewer";
        return this.initFrame(config);
      }
    }, {
      key: "initRoomSelector",
      value: function initRoomSelector() {
        var config = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        config.mode = "room-selector";
        return this.initFrame(config);
      }
    }, {
      key: "initFileSelector",
      value: function initFileSelector() {
        var config = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        config.mode = "file-selector";
        return this.initFrame(config);
      }
    }, {
      key: "initSystem",
      value: function initSystem() {
        var config = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        config.mode = "system";
        return this.initFrame(config);
      }
    }, {
      key: "destroyFrame",
      value: function destroyFrame() {
        var target = document.createElement("div");
        target.setAttribute("id", this.config.frameId);
        target.innerHTML = this.config.destroyText;
        target.className = _classPrivateFieldGet(this, _classNames);
        if (_classPrivateFieldGet(this, _iframe)) {
          window.removeEventListener("message", _classPrivateFieldGet(this, _onMessage), false);
          _classPrivateFieldSet(this, _isConnected, false);
          delete window.DocSpace.SDK.frames[this.config.frameId];
          _classPrivateFieldGet(this, _iframe).parentNode && _classPrivateFieldGet(this, _iframe).parentNode.replaceChild(target, _classPrivateFieldGet(this, _iframe));
        }
        this.config = {};
      }
    }, {
      key: "getFolderInfo",
      value: function getFolderInfo() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getFolderInfo");
      }
    }, {
      key: "getSelection",
      value: function getSelection() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getSelection");
      }
    }, {
      key: "getFiles",
      value: function getFiles() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getFiles");
      }
    }, {
      key: "getFolders",
      value: function getFolders() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getFolders");
      }
    }, {
      key: "getList",
      value: function getList() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getList");
      }
    }, {
      key: "getRooms",
      value: function getRooms(filter) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getRooms", filter);
      }
    }, {
      key: "getUserInfo",
      value: function getUserInfo() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getUserInfo");
      }
    }, {
      key: "getConfig",
      value: function getConfig() {
        return this.config;
      }
    }, {
      key: "getHashSettings",
      value: function getHashSettings() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "getHashSettings");
      }
    }, {
      key: "setConfig",
      value: function setConfig() {
        var newConfig = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
        var reload = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : false;
        if (_classPrivateFieldGet(this, _oneOfExistInObject).call(this, this.config.keysForReload, newConfig)) reload = true;
        this.config = _objectSpread(_objectSpread({}, this.config), newConfig);
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "setConfig", this.config, reload);
      }
    }, {
      key: "openModal",
      value: function openModal(type, options) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "openModal", {
          type: type,
          options: options
        });
      }
    }, {
      key: "createFile",
      value: function createFile(folderId, title, templateId, formId) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "createFile", {
          folderId: folderId,
          title: title,
          templateId: templateId,
          formId: formId
        });
      }
    }, {
      key: "createFolder",
      value: function createFolder(parentFolderId, title) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "createFolder", {
          parentFolderId: parentFolderId,
          title: title
        });
      }
    }, {
      key: "createRoom",
      value: function createRoom(title, roomType) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "createRoom", {
          title: title,
          roomType: roomType
        });
      }
    }, {
      key: "setListView",
      value: function setListView(type) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "setItemsView", type);
      }
    }, {
      key: "createHash",
      value: function createHash(password, hashSettings) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "createHash", {
          password: password,
          hashSettings: hashSettings
        });
      }
    }, {
      key: "login",
      value: function login(email, passwordHash) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "login", {
          email: email,
          passwordHash: passwordHash
        });
      }
    }, {
      key: "logout",
      value: function logout() {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "logout");
      }
    }, {
      key: "createTag",
      value: function createTag(name) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "createTag", name);
      }
    }, {
      key: "addTagsToRoom",
      value: function addTagsToRoom(roomId, tags) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "addTagsToRoom", {
          roomId: roomId,
          tags: tags
        });
      }
    }, {
      key: "removeTagsFromRoom",
      value: function removeTagsFromRoom(roomId, tags) {
        return _classPrivateFieldGet(this, _getMethodPromise).call(this, "removeTagsFromRoom", {
          roomId: roomId,
          tags: tags
        });
      }
    }]);
    return DocSpace;
  }();
  var config = getConfigFromParams();
  window.DocSpace = window.DocSpace || {};
  window.DocSpace.SDK = new DocSpace(config);
  if (config.init) {
    window.DocSpace.SDK.initFrame(config);
  }
})();