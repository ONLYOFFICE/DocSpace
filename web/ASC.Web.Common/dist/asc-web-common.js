'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

function _interopDefault (ex) { return (ex && (typeof ex === 'object') && 'default' in ex) ? ex['default'] : ex; }

var axios = _interopDefault(require('axios'));
var Cookies = _interopDefault(require('universal-cookie'));
var history$1 = require('history');

function _classCallCheck(instance, Constructor) {
  if (!(instance instanceof Constructor)) {
    throw new TypeError("Cannot call a class as a function");
  }
}

function _defineProperties(target, props) {
  for (var i = 0; i < props.length; i++) {
    var descriptor = props[i];
    descriptor.enumerable = descriptor.enumerable || false;
    descriptor.configurable = true;
    if ("value" in descriptor) descriptor.writable = true;
    Object.defineProperty(target, descriptor.key, descriptor);
  }
}

function _createClass(Constructor, protoProps, staticProps) {
  if (protoProps) _defineProperties(Constructor.prototype, protoProps);
  if (staticProps) _defineProperties(Constructor, staticProps);
  return Constructor;
}

function _defineProperty(obj, key, value) {
  if (key in obj) {
    Object.defineProperty(obj, key, {
      value: value,
      enumerable: true,
      configurable: true,
      writable: true
    });
  } else {
    obj[key] = value;
  }

  return obj;
}

function ownKeys(object, enumerableOnly) {
  var keys = Object.keys(object);

  if (Object.getOwnPropertySymbols) {
    var symbols = Object.getOwnPropertySymbols(object);
    if (enumerableOnly) symbols = symbols.filter(function (sym) {
      return Object.getOwnPropertyDescriptor(object, sym).enumerable;
    });
    keys.push.apply(keys, symbols);
  }

  return keys;
}

function _objectSpread2(target) {
  for (var i = 1; i < arguments.length; i++) {
    var source = arguments[i] != null ? arguments[i] : {};

    if (i % 2) {
      ownKeys(source, true).forEach(function (key) {
        _defineProperty(target, key, source[key]);
      });
    } else if (Object.getOwnPropertyDescriptors) {
      Object.defineProperties(target, Object.getOwnPropertyDescriptors(source));
    } else {
      ownKeys(source).forEach(function (key) {
        Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key));
      });
    }
  }

  return target;
}

var toUrlParams = function toUrlParams(obj, skipNull) {
  var str = "";
  for (var key in obj) {
    if (skipNull && !obj[key]) continue;
    if (str !== "") {
      str += "&";
    }
    str += key + "=" + encodeURIComponent(obj[key]);
  }
  return str;
};

var DEFAULT_PAGE = 0;
var DEFAULT_PAGE_COUNT = 25;
var DEFAULT_TOTAL = 0;
var DEFAULT_SORT_BY = "firstname";
var DEFAULT_SORT_ORDER = "ascending";
var DEFAULT_EMPLOYEE_STATUS = null;
var DEFAULT_ACTIVATION_STATUS = null;
var DEFAULT_ROLE = null;
var DEFAULT_SEARCH = null;
var DEFAULT_GROUP = null;
var Filter =
function () {
  _createClass(Filter, null, [{
    key: "getDefault",
    value: function getDefault() {
      var total = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : DEFAULT_TOTAL;
      return new Filter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, total);
    }
  }]);
  function Filter() {
    var _this = this;
    var page = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : DEFAULT_PAGE;
    var _pageCount = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : DEFAULT_PAGE_COUNT;
    var total = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : DEFAULT_TOTAL;
    var _sortBy = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : DEFAULT_SORT_BY;
    var _sortOrder = arguments.length > 4 && arguments[4] !== undefined ? arguments[4] : DEFAULT_SORT_ORDER;
    var _employeeStatus = arguments.length > 5 && arguments[5] !== undefined ? arguments[5] : DEFAULT_EMPLOYEE_STATUS;
    var _activationStatus = arguments.length > 6 && arguments[6] !== undefined ? arguments[6] : DEFAULT_ACTIVATION_STATUS;
    var _role = arguments.length > 7 && arguments[7] !== undefined ? arguments[7] : DEFAULT_ROLE;
    var _search = arguments.length > 8 && arguments[8] !== undefined ? arguments[8] : DEFAULT_SEARCH;
    var _group = arguments.length > 9 && arguments[9] !== undefined ? arguments[9] : DEFAULT_GROUP;
    _classCallCheck(this, Filter);
    this.getStartIndex = function () {
      return _this.page * _this.pageCount;
    };
    this.hasNext = function () {
      return _this.total - _this.getStartIndex() > _this.pageCount;
    };
    this.hasPrev = function () {
      return _this.page > 0;
    };
    this.toDto = function () {
      var pageCount = _this.pageCount,
          sortBy = _this.sortBy,
          sortOrder = _this.sortOrder,
          employeeStatus = _this.employeeStatus,
          activationStatus = _this.activationStatus,
          role = _this.role,
          search = _this.search,
          group = _this.group;
      var dtoFilter = {
        StartIndex: _this.getStartIndex(),
        Count: pageCount,
        sortby: sortBy,
        sortorder: sortOrder,
        employeestatus: employeeStatus,
        activationstatus: activationStatus,
        filtervalue: search,
        groupId: group
      };
      switch (role) {
        case "admin":
          dtoFilter.isadministrator = true;
          break;
        case "user":
          dtoFilter.employeeType = 1;
          break;
        case "guest":
          dtoFilter.employeeType = 2;
          break;
      }
      return dtoFilter;
    };
    this.toUrlParams = function () {
      var dtoFilter = _this.toDto();
      var str = toUrlParams(dtoFilter, true);
      return str;
    };
    this.page = page;
    this.pageCount = _pageCount;
    this.sortBy = _sortBy;
    this.sortOrder = _sortOrder;
    this.employeeStatus = _employeeStatus;
    this.activationStatus = _activationStatus;
    this.role = _role;
    this.search = _search;
    this.total = total;
    this.group = _group;
  }
  _createClass(Filter, [{
    key: "clone",
    value: function clone(onlySorting) {
      return onlySorting ? new Filter(DEFAULT_PAGE, DEFAULT_PAGE_COUNT, DEFAULT_TOTAL, this.sortBy, this.sortOrder) : new Filter(this.page, this.pageCount, this.total, this.sortBy, this.sortOrder, this.employeeStatus, this.activationStatus, this.role, this.search, this.group);
    }
  }, {
    key: "equals",
    value: function equals(filter) {
      var equals = this.employeeStatus === filter.employeeStatus && this.activationStatus === filter.activationStatus && this.role === filter.role && this.group === filter.group && this.search === filter.search && this.sortBy === filter.sortBy && this.sortOrder === filter.sortOrder && this.page === filter.page && this.pageCount === filter.pageCount;
      return equals;
    }
  }]);
  return Filter;
}();

var history = history$1.createBrowserHistory();

var AUTH_KEY = 'asc_auth_key';
var EmployeeActivationStatus = Object.freeze({
  NotActivated: 0,
  Activated: 1,
  Pending: 2,
  AutoGenerated: 4
});
var ConfirmType = Object.freeze({
  EmpInvite: 0,
  LinkInvite: 1,
  PortalSuspend: 2,
  PortalContinue: 3,
  PortalRemove: 4,
  DnsChange: 5,
  PortalOwnerChange: 6,
  Activation: 7,
  EmailChange: 8,
  EmailActivation: 9,
  PasswordChange: 10,
  ProfileRemove: 11,
  PhoneActivation: 12,
  PhoneAuth: 13,
  Auth: 14,
  TfaActivation: 15,
  TfaAuth: 16
});
var ValidationResult = Object.freeze({
  Ok: 0,
  Invalid: 1,
  Expired: 2
});
var EmployeeStatus = Object.freeze({
  Active: 1,
  Disabled: 2
});

var PREFIX = "api";
var VERSION = "2.0";
var baseURL = "".concat(window.location.origin, "/").concat(PREFIX, "/").concat(VERSION);
var client = axios.create({
  baseURL: baseURL,
  responseType: 'json',
  timeout: 30000
});
setAuthorizationToken(localStorage.getItem(AUTH_KEY));
client.interceptors.response.use(function (response) {
  return response;
}, function (error) {
  if (error.response.status === 401) {
    history.push("/login/error=unauthorized");
  }
  if (error.response.status === 502) {
    history.push("/error/".concat(error.response.status));
  }
  return error;
});
function setAuthorizationToken(token) {
  var cookies = new Cookies();
  if (token) {
    client.defaults.headers.common['Authorization'] = token;
    localStorage.setItem(AUTH_KEY, token);
    var current = new Date();
    var nextYear = new Date();
    nextYear.setFullYear(current.getFullYear() + 1);
    cookies.set(AUTH_KEY, token, {
      path: "/",
      expires: nextYear
    });
  } else {
    localStorage.clear();
    delete client.defaults.headers.common['Authorization'];
    cookies.remove(AUTH_KEY, {
      path: "/"
    });
  }
}
var checkResponseError = function checkResponseError(res) {
  if (res && res.data && res.data.error) {
    console.error(res.data.error);
    throw new Error(res.data.error.message);
  }
};
var request = function request(options) {
  var onSuccess = function onSuccess(response) {
    checkResponseError(response);
    return response.data.response;
  };
  var onError = function onError(error) {
    console.error("Request Failed:", error.config);
    if (error.response) {
      console.error("Status:", error.response.status);
      console.error("Data:", error.response.data);
      console.error("Headers:", error.response.headers);
    } else {
      console.error("Error Message:", error.message);
    }
    return Promise.reject(error.response || error.message);
  };
  return client(options).then(onSuccess).catch(onError);
};

function getUserList() {
  var filter = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : Filter.getDefault();
  var params = filter && filter instanceof Filter ? "/filter.json?".concat(filter.toUrlParams()) : "";
  return request({
    method: "get",
    url: "/people".concat(params)
  });
}

var People = /*#__PURE__*/Object.freeze({
  __proto__: null,
  getUserList: getUserList
});

var index = {
  Filter: Filter,
  People: People
};

var api = /*#__PURE__*/Object.freeze({
  __proto__: null,
  'default': index
});

var commonjsGlobal = typeof globalThis !== 'undefined' ? globalThis : typeof window !== 'undefined' ? window : typeof global !== 'undefined' ? global : typeof self !== 'undefined' ? self : {};

function createCommonjsModule(fn, module) {
	return module = { exports: {} }, fn(module, module.exports), module.exports;
}

var runtime_1 = createCommonjsModule(function (module) {
var runtime = (function (exports) {
  var Op = Object.prototype;
  var hasOwn = Op.hasOwnProperty;
  var undefined$1;
  var $Symbol = typeof Symbol === "function" ? Symbol : {};
  var iteratorSymbol = $Symbol.iterator || "@@iterator";
  var asyncIteratorSymbol = $Symbol.asyncIterator || "@@asyncIterator";
  var toStringTagSymbol = $Symbol.toStringTag || "@@toStringTag";
  function wrap(innerFn, outerFn, self, tryLocsList) {
    var protoGenerator = outerFn && outerFn.prototype instanceof Generator ? outerFn : Generator;
    var generator = Object.create(protoGenerator.prototype);
    var context = new Context(tryLocsList || []);
    generator._invoke = makeInvokeMethod(innerFn, self, context);
    return generator;
  }
  exports.wrap = wrap;
  function tryCatch(fn, obj, arg) {
    try {
      return { type: "normal", arg: fn.call(obj, arg) };
    } catch (err) {
      return { type: "throw", arg: err };
    }
  }
  var GenStateSuspendedStart = "suspendedStart";
  var GenStateSuspendedYield = "suspendedYield";
  var GenStateExecuting = "executing";
  var GenStateCompleted = "completed";
  var ContinueSentinel = {};
  function Generator() {}
  function GeneratorFunction() {}
  function GeneratorFunctionPrototype() {}
  var IteratorPrototype = {};
  IteratorPrototype[iteratorSymbol] = function () {
    return this;
  };
  var getProto = Object.getPrototypeOf;
  var NativeIteratorPrototype = getProto && getProto(getProto(values([])));
  if (NativeIteratorPrototype &&
      NativeIteratorPrototype !== Op &&
      hasOwn.call(NativeIteratorPrototype, iteratorSymbol)) {
    IteratorPrototype = NativeIteratorPrototype;
  }
  var Gp = GeneratorFunctionPrototype.prototype =
    Generator.prototype = Object.create(IteratorPrototype);
  GeneratorFunction.prototype = Gp.constructor = GeneratorFunctionPrototype;
  GeneratorFunctionPrototype.constructor = GeneratorFunction;
  GeneratorFunctionPrototype[toStringTagSymbol] =
    GeneratorFunction.displayName = "GeneratorFunction";
  function defineIteratorMethods(prototype) {
    ["next", "throw", "return"].forEach(function(method) {
      prototype[method] = function(arg) {
        return this._invoke(method, arg);
      };
    });
  }
  exports.isGeneratorFunction = function(genFun) {
    var ctor = typeof genFun === "function" && genFun.constructor;
    return ctor
      ? ctor === GeneratorFunction ||
        (ctor.displayName || ctor.name) === "GeneratorFunction"
      : false;
  };
  exports.mark = function(genFun) {
    if (Object.setPrototypeOf) {
      Object.setPrototypeOf(genFun, GeneratorFunctionPrototype);
    } else {
      genFun.__proto__ = GeneratorFunctionPrototype;
      if (!(toStringTagSymbol in genFun)) {
        genFun[toStringTagSymbol] = "GeneratorFunction";
      }
    }
    genFun.prototype = Object.create(Gp);
    return genFun;
  };
  exports.awrap = function(arg) {
    return { __await: arg };
  };
  function AsyncIterator(generator) {
    function invoke(method, arg, resolve, reject) {
      var record = tryCatch(generator[method], generator, arg);
      if (record.type === "throw") {
        reject(record.arg);
      } else {
        var result = record.arg;
        var value = result.value;
        if (value &&
            typeof value === "object" &&
            hasOwn.call(value, "__await")) {
          return Promise.resolve(value.__await).then(function(value) {
            invoke("next", value, resolve, reject);
          }, function(err) {
            invoke("throw", err, resolve, reject);
          });
        }
        return Promise.resolve(value).then(function(unwrapped) {
          result.value = unwrapped;
          resolve(result);
        }, function(error) {
          return invoke("throw", error, resolve, reject);
        });
      }
    }
    var previousPromise;
    function enqueue(method, arg) {
      function callInvokeWithMethodAndArg() {
        return new Promise(function(resolve, reject) {
          invoke(method, arg, resolve, reject);
        });
      }
      return previousPromise =
        previousPromise ? previousPromise.then(
          callInvokeWithMethodAndArg,
          callInvokeWithMethodAndArg
        ) : callInvokeWithMethodAndArg();
    }
    this._invoke = enqueue;
  }
  defineIteratorMethods(AsyncIterator.prototype);
  AsyncIterator.prototype[asyncIteratorSymbol] = function () {
    return this;
  };
  exports.AsyncIterator = AsyncIterator;
  exports.async = function(innerFn, outerFn, self, tryLocsList) {
    var iter = new AsyncIterator(
      wrap(innerFn, outerFn, self, tryLocsList)
    );
    return exports.isGeneratorFunction(outerFn)
      ? iter
      : iter.next().then(function(result) {
          return result.done ? result.value : iter.next();
        });
  };
  function makeInvokeMethod(innerFn, self, context) {
    var state = GenStateSuspendedStart;
    return function invoke(method, arg) {
      if (state === GenStateExecuting) {
        throw new Error("Generator is already running");
      }
      if (state === GenStateCompleted) {
        if (method === "throw") {
          throw arg;
        }
        return doneResult();
      }
      context.method = method;
      context.arg = arg;
      while (true) {
        var delegate = context.delegate;
        if (delegate) {
          var delegateResult = maybeInvokeDelegate(delegate, context);
          if (delegateResult) {
            if (delegateResult === ContinueSentinel) continue;
            return delegateResult;
          }
        }
        if (context.method === "next") {
          context.sent = context._sent = context.arg;
        } else if (context.method === "throw") {
          if (state === GenStateSuspendedStart) {
            state = GenStateCompleted;
            throw context.arg;
          }
          context.dispatchException(context.arg);
        } else if (context.method === "return") {
          context.abrupt("return", context.arg);
        }
        state = GenStateExecuting;
        var record = tryCatch(innerFn, self, context);
        if (record.type === "normal") {
          state = context.done
            ? GenStateCompleted
            : GenStateSuspendedYield;
          if (record.arg === ContinueSentinel) {
            continue;
          }
          return {
            value: record.arg,
            done: context.done
          };
        } else if (record.type === "throw") {
          state = GenStateCompleted;
          context.method = "throw";
          context.arg = record.arg;
        }
      }
    };
  }
  function maybeInvokeDelegate(delegate, context) {
    var method = delegate.iterator[context.method];
    if (method === undefined$1) {
      context.delegate = null;
      if (context.method === "throw") {
        if (delegate.iterator["return"]) {
          context.method = "return";
          context.arg = undefined$1;
          maybeInvokeDelegate(delegate, context);
          if (context.method === "throw") {
            return ContinueSentinel;
          }
        }
        context.method = "throw";
        context.arg = new TypeError(
          "The iterator does not provide a 'throw' method");
      }
      return ContinueSentinel;
    }
    var record = tryCatch(method, delegate.iterator, context.arg);
    if (record.type === "throw") {
      context.method = "throw";
      context.arg = record.arg;
      context.delegate = null;
      return ContinueSentinel;
    }
    var info = record.arg;
    if (! info) {
      context.method = "throw";
      context.arg = new TypeError("iterator result is not an object");
      context.delegate = null;
      return ContinueSentinel;
    }
    if (info.done) {
      context[delegate.resultName] = info.value;
      context.next = delegate.nextLoc;
      if (context.method !== "return") {
        context.method = "next";
        context.arg = undefined$1;
      }
    } else {
      return info;
    }
    context.delegate = null;
    return ContinueSentinel;
  }
  defineIteratorMethods(Gp);
  Gp[toStringTagSymbol] = "Generator";
  Gp[iteratorSymbol] = function() {
    return this;
  };
  Gp.toString = function() {
    return "[object Generator]";
  };
  function pushTryEntry(locs) {
    var entry = { tryLoc: locs[0] };
    if (1 in locs) {
      entry.catchLoc = locs[1];
    }
    if (2 in locs) {
      entry.finallyLoc = locs[2];
      entry.afterLoc = locs[3];
    }
    this.tryEntries.push(entry);
  }
  function resetTryEntry(entry) {
    var record = entry.completion || {};
    record.type = "normal";
    delete record.arg;
    entry.completion = record;
  }
  function Context(tryLocsList) {
    this.tryEntries = [{ tryLoc: "root" }];
    tryLocsList.forEach(pushTryEntry, this);
    this.reset(true);
  }
  exports.keys = function(object) {
    var keys = [];
    for (var key in object) {
      keys.push(key);
    }
    keys.reverse();
    return function next() {
      while (keys.length) {
        var key = keys.pop();
        if (key in object) {
          next.value = key;
          next.done = false;
          return next;
        }
      }
      next.done = true;
      return next;
    };
  };
  function values(iterable) {
    if (iterable) {
      var iteratorMethod = iterable[iteratorSymbol];
      if (iteratorMethod) {
        return iteratorMethod.call(iterable);
      }
      if (typeof iterable.next === "function") {
        return iterable;
      }
      if (!isNaN(iterable.length)) {
        var i = -1, next = function next() {
          while (++i < iterable.length) {
            if (hasOwn.call(iterable, i)) {
              next.value = iterable[i];
              next.done = false;
              return next;
            }
          }
          next.value = undefined$1;
          next.done = true;
          return next;
        };
        return next.next = next;
      }
    }
    return { next: doneResult };
  }
  exports.values = values;
  function doneResult() {
    return { value: undefined$1, done: true };
  }
  Context.prototype = {
    constructor: Context,
    reset: function(skipTempReset) {
      this.prev = 0;
      this.next = 0;
      this.sent = this._sent = undefined$1;
      this.done = false;
      this.delegate = null;
      this.method = "next";
      this.arg = undefined$1;
      this.tryEntries.forEach(resetTryEntry);
      if (!skipTempReset) {
        for (var name in this) {
          if (name.charAt(0) === "t" &&
              hasOwn.call(this, name) &&
              !isNaN(+name.slice(1))) {
            this[name] = undefined$1;
          }
        }
      }
    },
    stop: function() {
      this.done = true;
      var rootEntry = this.tryEntries[0];
      var rootRecord = rootEntry.completion;
      if (rootRecord.type === "throw") {
        throw rootRecord.arg;
      }
      return this.rval;
    },
    dispatchException: function(exception) {
      if (this.done) {
        throw exception;
      }
      var context = this;
      function handle(loc, caught) {
        record.type = "throw";
        record.arg = exception;
        context.next = loc;
        if (caught) {
          context.method = "next";
          context.arg = undefined$1;
        }
        return !! caught;
      }
      for (var i = this.tryEntries.length - 1; i >= 0; --i) {
        var entry = this.tryEntries[i];
        var record = entry.completion;
        if (entry.tryLoc === "root") {
          return handle("end");
        }
        if (entry.tryLoc <= this.prev) {
          var hasCatch = hasOwn.call(entry, "catchLoc");
          var hasFinally = hasOwn.call(entry, "finallyLoc");
          if (hasCatch && hasFinally) {
            if (this.prev < entry.catchLoc) {
              return handle(entry.catchLoc, true);
            } else if (this.prev < entry.finallyLoc) {
              return handle(entry.finallyLoc);
            }
          } else if (hasCatch) {
            if (this.prev < entry.catchLoc) {
              return handle(entry.catchLoc, true);
            }
          } else if (hasFinally) {
            if (this.prev < entry.finallyLoc) {
              return handle(entry.finallyLoc);
            }
          } else {
            throw new Error("try statement without catch or finally");
          }
        }
      }
    },
    abrupt: function(type, arg) {
      for (var i = this.tryEntries.length - 1; i >= 0; --i) {
        var entry = this.tryEntries[i];
        if (entry.tryLoc <= this.prev &&
            hasOwn.call(entry, "finallyLoc") &&
            this.prev < entry.finallyLoc) {
          var finallyEntry = entry;
          break;
        }
      }
      if (finallyEntry &&
          (type === "break" ||
           type === "continue") &&
          finallyEntry.tryLoc <= arg &&
          arg <= finallyEntry.finallyLoc) {
        finallyEntry = null;
      }
      var record = finallyEntry ? finallyEntry.completion : {};
      record.type = type;
      record.arg = arg;
      if (finallyEntry) {
        this.method = "next";
        this.next = finallyEntry.finallyLoc;
        return ContinueSentinel;
      }
      return this.complete(record);
    },
    complete: function(record, afterLoc) {
      if (record.type === "throw") {
        throw record.arg;
      }
      if (record.type === "break" ||
          record.type === "continue") {
        this.next = record.arg;
      } else if (record.type === "return") {
        this.rval = this.arg = record.arg;
        this.method = "return";
        this.next = "end";
      } else if (record.type === "normal" && afterLoc) {
        this.next = afterLoc;
      }
      return ContinueSentinel;
    },
    finish: function(finallyLoc) {
      for (var i = this.tryEntries.length - 1; i >= 0; --i) {
        var entry = this.tryEntries[i];
        if (entry.finallyLoc === finallyLoc) {
          this.complete(entry.completion, entry.afterLoc);
          resetTryEntry(entry);
          return ContinueSentinel;
        }
      }
    },
    "catch": function(tryLoc) {
      for (var i = this.tryEntries.length - 1; i >= 0; --i) {
        var entry = this.tryEntries[i];
        if (entry.tryLoc === tryLoc) {
          var record = entry.completion;
          if (record.type === "throw") {
            var thrown = record.arg;
            resetTryEntry(entry);
          }
          return thrown;
        }
      }
      throw new Error("illegal catch attempt");
    },
    delegateYield: function(iterable, resultName, nextLoc) {
      this.delegate = {
        iterator: values(iterable),
        resultName: resultName,
        nextLoc: nextLoc
      };
      if (this.method === "next") {
        this.arg = undefined$1;
      }
      return ContinueSentinel;
    }
  };
  return exports;
}(
   module.exports 
));
try {
  regeneratorRuntime = runtime;
} catch (accidentalStrictMode) {
  Function("r", "regeneratorRuntime = r")(runtime);
}
});

var regenerator = runtime_1;

var LOGIN_POST = "LOGIN_POST";
var SET_CURRENT_USER = "SET_CURRENT_USER";
var SET_MODULE = "SET_MODULE";
var SET_MODULES = "SET_MODULES";
var SET_SETTINGS = "SET_SETTINGS";
var SET_IS_LOADED = "SET_IS_LOADED";
var LOGOUT = "LOGOUT";
function setModule(module) {
  return {
    type: SET_MODULE,
    module: module
  };
}
function setCurrentUser(user) {
  return {
    type: SET_CURRENT_USER,
    user: user
  };
}
function setModules(modules) {
  return {
    type: SET_MODULES,
    modules: modules
  };
}
function setSettings(settings) {
  return {
    type: SET_SETTINGS,
    settings: settings
  };
}
function setIsLoaded(isLoaded) {
  return {
    type: SET_IS_LOADED,
    isLoaded: isLoaded
  };
}
function setLogout() {
  return {
    type: LOGOUT
  };
}
function getUserInfo(dispatch, additionalAction) {
  var _ref, user, modules, settings, newSettings, inviteLinks;
  return regenerator.async(function getUserInfo$(_context) {
    while (1) {
      switch (_context.prev = _context.next) {
        case 0:
          _context.next = 2;
          return regenerator.awrap(undefined());
        case 2:
          _ref = _context.sent;
          user = _ref.user;
          modules = _ref.modules;
          settings = _ref.settings;
          newSettings = settings;
          if (!user.isAdmin) {
            _context.next = 12;
            break;
          }
          _context.next = 10;
          return regenerator.awrap(undefined());
        case 10:
          inviteLinks = _context.sent;
          newSettings = Object.assign(newSettings, inviteLinks);
        case 12:
          dispatch(setCurrentUser(user));
          dispatch(setModules(modules));
          dispatch(setSettings(newSettings));
          additionalAction && additionalAction();
          return _context.abrupt("return", dispatch(setIsLoaded(true)));
        case 17:
        case "end":
          return _context.stop();
      }
    }
  });
}
function logout() {
  return function (dispatch) {
    setAuthorizationToken();
    return dispatch(setLogout());
  };
}

var actions = /*#__PURE__*/Object.freeze({
  __proto__: null,
  LOGIN_POST: LOGIN_POST,
  SET_CURRENT_USER: SET_CURRENT_USER,
  SET_MODULE: SET_MODULE,
  SET_MODULES: SET_MODULES,
  SET_SETTINGS: SET_SETTINGS,
  SET_IS_LOADED: SET_IS_LOADED,
  LOGOUT: LOGOUT,
  setModule: setModule,
  setCurrentUser: setCurrentUser,
  setModules: setModules,
  setSettings: setSettings,
  setIsLoaded: setIsLoaded,
  setLogout: setLogout,
  getUserInfo: getUserInfo,
  logout: logout
});

var objectProto = Object.prototype;
function isPrototype(value) {
  var Ctor = value && value.constructor,
      proto = (typeof Ctor == 'function' && Ctor.prototype) || objectProto;
  return value === proto;
}
var _isPrototype = isPrototype;

function overArg(func, transform) {
  return function(arg) {
    return func(transform(arg));
  };
}
var _overArg = overArg;

var nativeKeys = _overArg(Object.keys, Object);
var _nativeKeys = nativeKeys;

var objectProto$1 = Object.prototype;
var hasOwnProperty = objectProto$1.hasOwnProperty;
function baseKeys(object) {
  if (!_isPrototype(object)) {
    return _nativeKeys(object);
  }
  var result = [];
  for (var key in Object(object)) {
    if (hasOwnProperty.call(object, key) && key != 'constructor') {
      result.push(key);
    }
  }
  return result;
}
var _baseKeys = baseKeys;

var freeGlobal = typeof commonjsGlobal == 'object' && commonjsGlobal && commonjsGlobal.Object === Object && commonjsGlobal;
var _freeGlobal = freeGlobal;

var freeSelf = typeof self == 'object' && self && self.Object === Object && self;
var root = _freeGlobal || freeSelf || Function('return this')();
var _root = root;

var Symbol$1 = _root.Symbol;
var _Symbol = Symbol$1;

var objectProto$2 = Object.prototype;
var hasOwnProperty$1 = objectProto$2.hasOwnProperty;
var nativeObjectToString = objectProto$2.toString;
var symToStringTag = _Symbol ? _Symbol.toStringTag : undefined;
function getRawTag(value) {
  var isOwn = hasOwnProperty$1.call(value, symToStringTag),
      tag = value[symToStringTag];
  try {
    value[symToStringTag] = undefined;
    var unmasked = true;
  } catch (e) {}
  var result = nativeObjectToString.call(value);
  if (unmasked) {
    if (isOwn) {
      value[symToStringTag] = tag;
    } else {
      delete value[symToStringTag];
    }
  }
  return result;
}
var _getRawTag = getRawTag;

var objectProto$3 = Object.prototype;
var nativeObjectToString$1 = objectProto$3.toString;
function objectToString(value) {
  return nativeObjectToString$1.call(value);
}
var _objectToString = objectToString;

var nullTag = '[object Null]',
    undefinedTag = '[object Undefined]';
var symToStringTag$1 = _Symbol ? _Symbol.toStringTag : undefined;
function baseGetTag(value) {
  if (value == null) {
    return value === undefined ? undefinedTag : nullTag;
  }
  return (symToStringTag$1 && symToStringTag$1 in Object(value))
    ? _getRawTag(value)
    : _objectToString(value);
}
var _baseGetTag = baseGetTag;

function isObject(value) {
  var type = typeof value;
  return value != null && (type == 'object' || type == 'function');
}
var isObject_1 = isObject;

var asyncTag = '[object AsyncFunction]',
    funcTag = '[object Function]',
    genTag = '[object GeneratorFunction]',
    proxyTag = '[object Proxy]';
function isFunction(value) {
  if (!isObject_1(value)) {
    return false;
  }
  var tag = _baseGetTag(value);
  return tag == funcTag || tag == genTag || tag == asyncTag || tag == proxyTag;
}
var isFunction_1 = isFunction;

var coreJsData = _root['__core-js_shared__'];
var _coreJsData = coreJsData;

var maskSrcKey = (function() {
  var uid = /[^.]+$/.exec(_coreJsData && _coreJsData.keys && _coreJsData.keys.IE_PROTO || '');
  return uid ? ('Symbol(src)_1.' + uid) : '';
}());
function isMasked(func) {
  return !!maskSrcKey && (maskSrcKey in func);
}
var _isMasked = isMasked;

var funcProto = Function.prototype;
var funcToString = funcProto.toString;
function toSource(func) {
  if (func != null) {
    try {
      return funcToString.call(func);
    } catch (e) {}
    try {
      return (func + '');
    } catch (e) {}
  }
  return '';
}
var _toSource = toSource;

var reRegExpChar = /[\\^$.*+?()[\]{}|]/g;
var reIsHostCtor = /^\[object .+?Constructor\]$/;
var funcProto$1 = Function.prototype,
    objectProto$4 = Object.prototype;
var funcToString$1 = funcProto$1.toString;
var hasOwnProperty$2 = objectProto$4.hasOwnProperty;
var reIsNative = RegExp('^' +
  funcToString$1.call(hasOwnProperty$2).replace(reRegExpChar, '\\$&')
  .replace(/hasOwnProperty|(function).*?(?=\\\()| for .+?(?=\\\])/g, '$1.*?') + '$'
);
function baseIsNative(value) {
  if (!isObject_1(value) || _isMasked(value)) {
    return false;
  }
  var pattern = isFunction_1(value) ? reIsNative : reIsHostCtor;
  return pattern.test(_toSource(value));
}
var _baseIsNative = baseIsNative;

function getValue(object, key) {
  return object == null ? undefined : object[key];
}
var _getValue = getValue;

function getNative(object, key) {
  var value = _getValue(object, key);
  return _baseIsNative(value) ? value : undefined;
}
var _getNative = getNative;

var DataView = _getNative(_root, 'DataView');
var _DataView = DataView;

var Map = _getNative(_root, 'Map');
var _Map = Map;

var Promise$1 = _getNative(_root, 'Promise');
var _Promise = Promise$1;

var Set = _getNative(_root, 'Set');
var _Set = Set;

var WeakMap = _getNative(_root, 'WeakMap');
var _WeakMap = WeakMap;

var mapTag = '[object Map]',
    objectTag = '[object Object]',
    promiseTag = '[object Promise]',
    setTag = '[object Set]',
    weakMapTag = '[object WeakMap]';
var dataViewTag = '[object DataView]';
var dataViewCtorString = _toSource(_DataView),
    mapCtorString = _toSource(_Map),
    promiseCtorString = _toSource(_Promise),
    setCtorString = _toSource(_Set),
    weakMapCtorString = _toSource(_WeakMap);
var getTag = _baseGetTag;
if ((_DataView && getTag(new _DataView(new ArrayBuffer(1))) != dataViewTag) ||
    (_Map && getTag(new _Map) != mapTag) ||
    (_Promise && getTag(_Promise.resolve()) != promiseTag) ||
    (_Set && getTag(new _Set) != setTag) ||
    (_WeakMap && getTag(new _WeakMap) != weakMapTag)) {
  getTag = function(value) {
    var result = _baseGetTag(value),
        Ctor = result == objectTag ? value.constructor : undefined,
        ctorString = Ctor ? _toSource(Ctor) : '';
    if (ctorString) {
      switch (ctorString) {
        case dataViewCtorString: return dataViewTag;
        case mapCtorString: return mapTag;
        case promiseCtorString: return promiseTag;
        case setCtorString: return setTag;
        case weakMapCtorString: return weakMapTag;
      }
    }
    return result;
  };
}
var _getTag = getTag;

function isObjectLike(value) {
  return value != null && typeof value == 'object';
}
var isObjectLike_1 = isObjectLike;

var argsTag = '[object Arguments]';
function baseIsArguments(value) {
  return isObjectLike_1(value) && _baseGetTag(value) == argsTag;
}
var _baseIsArguments = baseIsArguments;

var objectProto$5 = Object.prototype;
var hasOwnProperty$3 = objectProto$5.hasOwnProperty;
var propertyIsEnumerable = objectProto$5.propertyIsEnumerable;
var isArguments = _baseIsArguments(function() { return arguments; }()) ? _baseIsArguments : function(value) {
  return isObjectLike_1(value) && hasOwnProperty$3.call(value, 'callee') &&
    !propertyIsEnumerable.call(value, 'callee');
};
var isArguments_1 = isArguments;

var isArray = Array.isArray;
var isArray_1 = isArray;

var MAX_SAFE_INTEGER = 9007199254740991;
function isLength(value) {
  return typeof value == 'number' &&
    value > -1 && value % 1 == 0 && value <= MAX_SAFE_INTEGER;
}
var isLength_1 = isLength;

function isArrayLike(value) {
  return value != null && isLength_1(value.length) && !isFunction_1(value);
}
var isArrayLike_1 = isArrayLike;

function stubFalse() {
  return false;
}
var stubFalse_1 = stubFalse;

var isBuffer_1 = createCommonjsModule(function (module, exports) {
var freeExports =  exports && !exports.nodeType && exports;
var freeModule = freeExports && 'object' == 'object' && module && !module.nodeType && module;
var moduleExports = freeModule && freeModule.exports === freeExports;
var Buffer = moduleExports ? _root.Buffer : undefined;
var nativeIsBuffer = Buffer ? Buffer.isBuffer : undefined;
var isBuffer = nativeIsBuffer || stubFalse_1;
module.exports = isBuffer;
});

var argsTag$1 = '[object Arguments]',
    arrayTag = '[object Array]',
    boolTag = '[object Boolean]',
    dateTag = '[object Date]',
    errorTag = '[object Error]',
    funcTag$1 = '[object Function]',
    mapTag$1 = '[object Map]',
    numberTag = '[object Number]',
    objectTag$1 = '[object Object]',
    regexpTag = '[object RegExp]',
    setTag$1 = '[object Set]',
    stringTag = '[object String]',
    weakMapTag$1 = '[object WeakMap]';
var arrayBufferTag = '[object ArrayBuffer]',
    dataViewTag$1 = '[object DataView]',
    float32Tag = '[object Float32Array]',
    float64Tag = '[object Float64Array]',
    int8Tag = '[object Int8Array]',
    int16Tag = '[object Int16Array]',
    int32Tag = '[object Int32Array]',
    uint8Tag = '[object Uint8Array]',
    uint8ClampedTag = '[object Uint8ClampedArray]',
    uint16Tag = '[object Uint16Array]',
    uint32Tag = '[object Uint32Array]';
var typedArrayTags = {};
typedArrayTags[float32Tag] = typedArrayTags[float64Tag] =
typedArrayTags[int8Tag] = typedArrayTags[int16Tag] =
typedArrayTags[int32Tag] = typedArrayTags[uint8Tag] =
typedArrayTags[uint8ClampedTag] = typedArrayTags[uint16Tag] =
typedArrayTags[uint32Tag] = true;
typedArrayTags[argsTag$1] = typedArrayTags[arrayTag] =
typedArrayTags[arrayBufferTag] = typedArrayTags[boolTag] =
typedArrayTags[dataViewTag$1] = typedArrayTags[dateTag] =
typedArrayTags[errorTag] = typedArrayTags[funcTag$1] =
typedArrayTags[mapTag$1] = typedArrayTags[numberTag] =
typedArrayTags[objectTag$1] = typedArrayTags[regexpTag] =
typedArrayTags[setTag$1] = typedArrayTags[stringTag] =
typedArrayTags[weakMapTag$1] = false;
function baseIsTypedArray(value) {
  return isObjectLike_1(value) &&
    isLength_1(value.length) && !!typedArrayTags[_baseGetTag(value)];
}
var _baseIsTypedArray = baseIsTypedArray;

function baseUnary(func) {
  return function(value) {
    return func(value);
  };
}
var _baseUnary = baseUnary;

var _nodeUtil = createCommonjsModule(function (module, exports) {
var freeExports =  exports && !exports.nodeType && exports;
var freeModule = freeExports && 'object' == 'object' && module && !module.nodeType && module;
var moduleExports = freeModule && freeModule.exports === freeExports;
var freeProcess = moduleExports && _freeGlobal.process;
var nodeUtil = (function() {
  try {
    var types = freeModule && freeModule.require && freeModule.require('util').types;
    if (types) {
      return types;
    }
    return freeProcess && freeProcess.binding && freeProcess.binding('util');
  } catch (e) {}
}());
module.exports = nodeUtil;
});

var nodeIsTypedArray = _nodeUtil && _nodeUtil.isTypedArray;
var isTypedArray = nodeIsTypedArray ? _baseUnary(nodeIsTypedArray) : _baseIsTypedArray;
var isTypedArray_1 = isTypedArray;

var mapTag$2 = '[object Map]',
    setTag$2 = '[object Set]';
var objectProto$6 = Object.prototype;
var hasOwnProperty$4 = objectProto$6.hasOwnProperty;
function isEmpty(value) {
  if (value == null) {
    return true;
  }
  if (isArrayLike_1(value) &&
      (isArray_1(value) || typeof value == 'string' || typeof value.splice == 'function' ||
        isBuffer_1(value) || isTypedArray_1(value) || isArguments_1(value))) {
    return !value.length;
  }
  var tag = _getTag(value);
  if (tag == mapTag$2 || tag == setTag$2) {
    return !value.size;
  }
  if (_isPrototype(value)) {
    return !_baseKeys(value).length;
  }
  for (var key in value) {
    if (hasOwnProperty$4.call(value, key)) {
      return false;
    }
  }
  return true;
}
var isEmpty_1 = isEmpty;

var initialState = {
  isAuthenticated: false,
  isLoaded: false,
  user: {},
  modules: [],
  settings: {
    currentProductId: null,
    culture: "en-US",
    trustedDomains: [],
    trustedDomainsType: 1,
    timezone: "UTC",
    utcOffset: "00:00:00",
    utcHoursOffset: 0,
    homepage: "",
    datePattern: "M/d/yyyy",
    datePatternJQ: "00/00/0000",
    dateTimePattern: "dddd, MMMM d, yyyy h:mm:ss tt",
    datepicker: {
      datePattern: "mm/dd/yy",
      dateTimePattern: "DD, mm dd, yy h:mm:ss tt",
      timePattern: "h:mm tt"
    },
    hasShortenService: false
  }
};
var authReducer = function authReducer() {
  var state = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : initialState;
  var action = arguments.length > 1 ? arguments[1] : undefined;
  switch (action.type) {
    case SET_CURRENT_USER:
      return Object.assign({}, state, {
        isAuthenticated: !isEmpty_1(action.user),
        user: action.user
      });
    case SET_MODULE:
      return Object.assign({}, state, {
        settings: _objectSpread2({}, state.settings, {
          currentProductId: action.module.id,
          homepage: action.module.homepage
        })
      });
    case SET_MODULES:
      return Object.assign({}, state, {
        modules: action.modules
      });
    case SET_SETTINGS:
      return Object.assign({}, state, {
        settings: _objectSpread2({}, state.settings, {}, action.settings)
      });
    case SET_IS_LOADED:
      return Object.assign({}, state, {
        isLoaded: action.isLoaded
      });
    case LOGOUT:
      return Object.assign({}, initialState, {
        isLoaded: true
      });
    default:
      return state;
  }
};

var reducers = /*#__PURE__*/Object.freeze({
  __proto__: null,
  'default': authReducer
});

function isAdmin(user) {
  var isPeopleAdmin = user.listAdminModules ? user.listAdminModules.includes('people') : false;
  return user.isAdmin || user.isOwner || isPeopleAdmin;
}
function isMe(user, userName) {
  return userName === "@self" || userName === user.userName;
}
function getCurrentModule(modules, currentModuleId) {
  return modules.find(function (module) {
    return module.id === currentModuleId;
  });
}

var selectors = /*#__PURE__*/Object.freeze({
  __proto__: null,
  isAdmin: isAdmin,
  isMe: isMe,
  getCurrentModule: getCurrentModule
});

var index$1 = {
  actions: actions,
  reducers: reducers,
  selectors: selectors
};

exports.Api = index;
exports.Auth = index$1;
//# sourceMappingURL=asc-web-common.js.map
