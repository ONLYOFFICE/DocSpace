export const frames = (fn) => {
  let previousArgs;
  let frameId = -1;
  let lock = false;

  return {
    next(...args) {
      previousArgs = args;
      if (!lock) {
        lock = true;
        frameId = requestAnimationFrame(() => {
          fn(...previousArgs);
          lock = false;
        });
      }
    },
    cancel() {
      cancelAnimationFrame(frameId);
      lock = false;
    },
  };
};
