import React from "react";
import { loadComponent } from "../components/dynamic";
import { FILES_SCOPE } from "../helpers/constants";

function useFilesUtils() {
  React.useEffect(() => {
    loadComponent(FILES_SCOPE, "./utils", "filesUtils")();
  }, []);
}

export default useFilesUtils;
