import {
  HttpClient,
  HttpClientModule
} from "./chunk-I4YACLXD.js";
import "./chunk-FZH77NA2.js";
import {
  EventEmitter,
  Inject,
  Injectable,
  InjectionToken,
  NgModule,
  Optional,
  setClassMetadata,
  ɵɵdefineInjectable,
  ɵɵdefineInjector,
  ɵɵdefineNgModule,
  ɵɵinject
} from "./chunk-HBIJZBP7.js";
import {
  fromEvent
} from "./chunk-OQLFUR6O.js";
import "./chunk-HL3YN3FC.js";
import {
  debounceTime,
  delay,
  retryWhen,
  startWith,
  switchMap,
  tap,
  timer
} from "./chunk-F6ZKU2BT.js";
import {
  __spreadProps,
  __spreadValues
} from "./chunk-WKYGNSYM.js";

// node_modules/ssr-window/ssr-window.esm.js
function isObject(obj) {
  return obj !== null && typeof obj === "object" && "constructor" in obj && obj.constructor === Object;
}
function extend(target = {}, src = {}) {
  Object.keys(src).forEach((key) => {
    if (typeof target[key] === "undefined")
      target[key] = src[key];
    else if (isObject(src[key]) && isObject(target[key]) && Object.keys(src[key]).length > 0) {
      extend(target[key], src[key]);
    }
  });
}
var ssrDocument = {
  body: {},
  addEventListener() {
  },
  removeEventListener() {
  },
  activeElement: {
    blur() {
    },
    nodeName: ""
  },
  querySelector() {
    return null;
  },
  querySelectorAll() {
    return [];
  },
  getElementById() {
    return null;
  },
  createEvent() {
    return {
      initEvent() {
      }
    };
  },
  createElement() {
    return {
      children: [],
      childNodes: [],
      style: {},
      setAttribute() {
      },
      getElementsByTagName() {
        return [];
      }
    };
  },
  createElementNS() {
    return {};
  },
  importNode() {
    return null;
  },
  location: {
    hash: "",
    host: "",
    hostname: "",
    href: "",
    origin: "",
    pathname: "",
    protocol: "",
    search: ""
  }
};
var ssrWindow = {
  document: ssrDocument,
  navigator: {
    userAgent: ""
  },
  location: {
    hash: "",
    host: "",
    hostname: "",
    href: "",
    origin: "",
    pathname: "",
    protocol: "",
    search: ""
  },
  history: {
    replaceState() {
    },
    pushState() {
    },
    go() {
    },
    back() {
    }
  },
  CustomEvent: function CustomEvent() {
    return this;
  },
  addEventListener() {
  },
  removeEventListener() {
  },
  getComputedStyle() {
    return {
      getPropertyValue() {
        return "";
      }
    };
  },
  Image() {
  },
  Date() {
  },
  screen: {},
  setTimeout() {
  },
  clearTimeout() {
  },
  matchMedia() {
    return {};
  },
  requestAnimationFrame(callback) {
    if (typeof setTimeout === "undefined") {
      callback();
      return null;
    }
    return setTimeout(callback, 0);
  },
  cancelAnimationFrame(id) {
    if (typeof setTimeout === "undefined") {
      return;
    }
    clearTimeout(id);
  }
};
function getWindow() {
  const win = typeof window !== "undefined" ? window : {};
  extend(win, ssrWindow);
  return win;
}

// node_modules/ngx-connection-service/fesm2022/ngx-connection-service.mjs
var window2 = getWindow();
var ConnectionServiceOptionsToken = new InjectionToken("ConnectionServiceOptionsToken");
var _ConnectionService = class _ConnectionService {
  /**
   * Current ConnectionService options. Notice that changing values of the returned object has not effect on service execution.
   * You should use "updateOptions" function.
   */
  get options() {
    return __spreadValues({}, this.serviceOptions);
  }
  constructor(http, options) {
    this.http = http;
    this.stateChangeEventEmitter = new EventEmitter();
    this.currentState = {
      hasInternetAccess: false,
      hasNetworkConnection: window2.navigator.onLine
    };
    this.serviceOptions = __spreadValues(__spreadProps(__spreadValues({}, _ConnectionService.DEFAULT_OPTIONS), {
      heartbeatExecutor: () => this.http.request(this.serviceOptions.requestMethod, this.serviceOptions.heartbeatUrl, {
        responseType: "text",
        withCredentials: false
      })
    }), options);
    this.checkNetworkState();
    this.checkInternetState();
  }
  checkInternetState() {
    if (this.httpSubscription) {
      this.httpSubscription.unsubscribe();
      this.httpSubscription = null;
    }
    if (this.serviceOptions.enableHeartbeat) {
      this.httpSubscription = timer(0, this.serviceOptions.heartbeatInterval).pipe(switchMap(() => this.serviceOptions.heartbeatExecutor(this.serviceOptions)), retryWhen((errors) => errors.pipe(
        tap((val) => {
          this.currentState.hasInternetAccess = false;
          this.emitEvent();
        }),
        // restart after 5 seconds
        delay(this.serviceOptions.heartbeatRetryInterval)
      ))).subscribe((result) => {
        this.currentState.hasInternetAccess = true;
        this.emitEvent();
      });
    } else {
      this.currentState.hasInternetAccess = false;
      this.emitEvent();
    }
  }
  checkNetworkState() {
    this.onlineSubscription = fromEvent(window2, "online").subscribe(() => {
      this.currentState.hasNetworkConnection = true;
      this.checkInternetState();
      this.emitEvent();
    });
    this.offlineSubscription = fromEvent(window2, "offline").subscribe(() => {
      this.currentState.hasNetworkConnection = false;
      this.currentState.hasInternetAccess = false;
      this.checkInternetState();
      this.emitEvent();
    });
  }
  emitEvent() {
    this.stateChangeEventEmitter.emit(this.currentState);
  }
  ngOnDestroy() {
    try {
      this.offlineSubscription.unsubscribe();
      this.onlineSubscription.unsubscribe();
      this.httpSubscription.unsubscribe();
    } catch (e) {
    }
  }
  /**
   * Monitor Network & Internet connection status by subscribing to this observer. If you set "reportCurrentState" to "false" then
   * function will not report current status of the connections when initially subscribed.
   * @param reportCurrentState Report current state when initial subscription. Default is "true"
   */
  monitor(reportCurrentState = true) {
    return reportCurrentState ? this.stateChangeEventEmitter.pipe(debounceTime(300), startWith(this.currentState)) : this.stateChangeEventEmitter.pipe(debounceTime(300));
  }
  /**
   * Update options of the service. You could specify partial options object. Values that are not specified will use default / previous
   * option values.
   * @param options Partial option values.
   */
  updateOptions(options) {
    this.serviceOptions = __spreadValues(__spreadValues({}, this.serviceOptions), options);
    this.checkInternetState();
  }
};
_ConnectionService.DEFAULT_OPTIONS = {
  enableHeartbeat: true,
  heartbeatUrl: "https://corsproxy.io?" + encodeURIComponent("https://internethealthtest.org"),
  heartbeatInterval: 3e4,
  heartbeatRetryInterval: 1e3,
  requestMethod: "get"
};
_ConnectionService.ɵfac = function ConnectionService_Factory(t) {
  return new (t || _ConnectionService)(ɵɵinject(HttpClient), ɵɵinject(ConnectionServiceOptionsToken, 8));
};
_ConnectionService.ɵprov = ɵɵdefineInjectable({
  token: _ConnectionService,
  factory: _ConnectionService.ɵfac,
  providedIn: "root"
});
var ConnectionService = _ConnectionService;
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(ConnectionService, [{
    type: Injectable,
    args: [{
      providedIn: "root"
    }]
  }], () => [{
    type: HttpClient
  }, {
    type: void 0,
    decorators: [{
      type: Inject,
      args: [ConnectionServiceOptionsToken]
    }, {
      type: Optional
    }]
  }], null);
})();
var _ConnectionServiceModule = class _ConnectionServiceModule {
};
_ConnectionServiceModule.ɵfac = function ConnectionServiceModule_Factory(t) {
  return new (t || _ConnectionServiceModule)();
};
_ConnectionServiceModule.ɵmod = ɵɵdefineNgModule({
  type: _ConnectionServiceModule,
  imports: [HttpClientModule]
});
_ConnectionServiceModule.ɵinj = ɵɵdefineInjector({
  providers: [ConnectionService],
  imports: [HttpClientModule]
});
var ConnectionServiceModule = _ConnectionServiceModule;
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(ConnectionServiceModule, [{
    type: NgModule,
    args: [{
      imports: [HttpClientModule],
      providers: [ConnectionService]
    }]
  }], null, null);
})();
export {
  ConnectionService,
  ConnectionServiceModule,
  ConnectionServiceOptionsToken
};
//# sourceMappingURL=ngx-connection-service.js.map
