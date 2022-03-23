import React from "react";
import { loadComponent } from "../components/dynamic";
import { STUDIO_SCOPE } from "../helpers/constants";

function useFilesUtils() {
  const [isInitialized, setIsInitialized] = React.useState(false);

  React.useEffect(() => {
    if (!isInitialized) {
      const exists = document.getElementById(STUDIO_SCOPE);
      if (exists) {
        loadComponent(STUDIO_SCOPE, "./toastr", "toastr")();
        setIsInitialized(true);
      }
    }
  });
}

export default useFilesUtils;
