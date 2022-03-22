import React from "react";
import { loadComponent } from "../components/dynamic";
import { FILES_SCOPE } from "../helpers/constants";

function useFilesUtils() {
  const [isInitialized, setIsInitialized] = React.useState(false);

  React.useEffect(() => {
    if (!isInitialized) {
      const exists = document.getElementById(FILES_SCOPE);
      if (exists) {
        loadComponent(FILES_SCOPE, "./utils", "filesUtils")();
        setIsInitialized(true);
      }
    }
  });
}

export default useFilesUtils;
