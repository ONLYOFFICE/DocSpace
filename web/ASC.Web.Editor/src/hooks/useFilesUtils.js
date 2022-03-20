import React from "react";
import { loadComponent } from "../components/dynamic";
import { FILES_SCOPE } from "../helpers/constants";
function useFilesUtils(isReadyFilesRemote) {
  React.useEffect(() => {
    if (isReadyFilesRemote) {
      loadComponent(FILES_SCOPE, "./utils", "filesUtils")();
    }
  }, []);
}

export default useFilesUtils;
