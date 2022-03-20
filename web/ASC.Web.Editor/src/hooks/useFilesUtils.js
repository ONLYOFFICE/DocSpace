import React from "react";
import { loadComponent } from "../components/dynamic";
import { FILES_SCOPE } from "../helpers/constants";
function useFilesUtils() {
  React.useEffect(() => {
    if (document.getElementById(FILES_SCOPE)) {
      loadComponent(FILES_SCOPE, "./utils", "filesUtils")();
    }
  }, []);
}

export default useFilesUtils;
