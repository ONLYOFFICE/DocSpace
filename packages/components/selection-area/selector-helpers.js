export const frames = (fn) => {
  let frameId = -1;
  let lock = false;

  const next = () => {
    if (!lock) {
      lock = true;
      frameId = requestAnimationFrame(() => {
        fn();
        lock = false;
      });
    }
  };

  const cancel = () => {
    cancelAnimationFrame(frameId);
    lock = false;
  };

  return { next, cancel };
};
