import React from "react";
import {
  loadComponent,
  useDynamicScript,
} from "../components/DynamicComponent";
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

  React.useEffect(() => {
    if (filesReady && studioReady) {
      initMfScripts();
    }

    if (studioFailed || filesFailed) {
      setIsError(true);
      setIsInitialized(false);
    }
  }, [filesReady, studioReady]);

  const initMfScripts = async () => {
    const SharingDialog = await loadComponent(FILES_SCOPE, "./SharingDialog")();
    const toastr = await loadComponent(STUDIO_SCOPE, "./toastr")();
    const filesUtils = await loadComponent(FILES_SCOPE, "./utils")();
    const authStore = await loadComponent(STUDIO_SCOPE, "./store")();

    window.toastr = toastr.default;
    window.filesUtils = filesUtils;
    window.SharingDialog = SharingDialog.default;
    window.authStore = authStore.default;

    setIsInitialized(true);
    setIsError(false);
  };

  return [isInitialized, isError];
}

export default useMfScripts;
