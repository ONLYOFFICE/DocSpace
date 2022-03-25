import React from "react";
import { loadComponent, useDynamicScript } from "../components/dynamic";
import {
  STUDIO_SCOPE,
  FILES_SCOPE,
  STUDIO_REMOTE_ENTRY_URL,
  FILES_REMOTE_ENTRY_URL,
} from "../helpers/constants";

function useMfScripts() {
  const [isInitialized, setIsInitialized] = React.useState(false);
  const [isError, setIsError] = React.useState(false);

  const { ready: filesReady, failed: filesFailed } = useDynamicScript({
    id: FILES_SCOPE,
    url: FILES_REMOTE_ENTRY_URL,
  });
  const { ready: studioReady, failed: studioFailed } = useDynamicScript({
    id: STUDIO_SCOPE,
    url: STUDIO_REMOTE_ENTRY_URL,
  });

  React.useEffect(async () => {
    if (filesReady && studioReady) {
      const [toastr, filesUtils, SharingDialog] = await Promise.all([
        loadComponent(STUDIO_SCOPE, "./toastr")(),
        loadComponent(FILES_SCOPE, "./utils")(),
        loadComponent(FILES_SCOPE, "./SharingDialog")(),
      ]);

      window.toastr = toastr.default;
      window.filesUtils = filesUtils;
      window.SharingDialog = SharingDialog.default;

      setIsInitialized(true);
      setIsError(false);
    }

    if (studioFailed || filesFailed) {
      setIsError(true);
      setIsInitialized(false);
    }
  }, [filesReady, studioReady]);

  return [isInitialized, isError];
}

export default useMfScripts;
