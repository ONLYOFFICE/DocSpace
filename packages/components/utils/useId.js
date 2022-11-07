import React from "react";

let ID = 0;
const genId = () => ID++;
let serverHandoffComplete = false;

const usePassiveLayoutEffect =
  React[
    typeof document !== "undefined" && document.createElement !== void 0
      ? "useLayoutEffect"
      : "useEffect"
  ];

const useId = (fallbackId, prefix = "prefix") => {
  const [id, setId] = React.useState(serverHandoffComplete ? genId : void 0);

  usePassiveLayoutEffect(() => {
    if (id === void 0) {
      setId(ID++);
    }

    serverHandoffComplete = true;
  }, []);

  return fallbackId ? fallbackId : id === void 0 ? id : prefix + id;
};

export default useId;
