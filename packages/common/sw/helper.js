import registerSW from "./register";
import unregisterSW from "./unregister";

window.SW = {
  register: registerSW,
  unregister: unregisterSW,
};

export { unregisterSW as registerSW, unregisterSW };
// TODO: Replace 'unregisterSW as registerSW' to 'registerSW' when sw.js is needed
//export { registerSW, unregisterSW };
